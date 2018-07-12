using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using KumoDictionary.Provider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KumoDictionary.AmazonDynamoDB.Internal
{
    internal class DynamoDBDictionaryBackend : IKumoDictionaryBackend
    {
        private AmazonDynamoDBClient _dynamoDBClient;
        private KumoDictionaryAmazonDynamoDBSettings _settings;
        private string _tableName;

        public string DictionaryName { get; }

        public DynamoDBDictionaryBackend(string dictionaryName, KumoDictionaryAmazonDynamoDBSettings settings, AmazonDynamoDBClient dynamoDBClient)
        {
            if (String.IsNullOrWhiteSpace(dictionaryName)) throw new ArgumentNullException(nameof(dictionaryName));
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (dynamoDBClient == null) throw new ArgumentNullException(nameof(dynamoDBClient));

            DictionaryName = dictionaryName;

            _settings = settings;
            _tableName = settings.TableName;
            _dynamoDBClient = dynamoDBClient;
        }

        private Dictionary<string, AttributeValue> SetKeyItems<TKey>(TKey key, Dictionary<string, AttributeValue> item = null)
        {
            item = item ?? new Dictionary<string, AttributeValue>();

            item["Dictionary"] = new AttributeValue(DictionaryName);
            item["Key"] = new AttributeValue(_settings.KeySerializer.SerializeAsKey(_settings.Serializer, key));

            return item;
        }

        private TKey GetKey<TKey>(Dictionary<string, AttributeValue> item)
        {
            return _settings.KeySerializer.DeserializeFromKey<TKey>(_settings.Serializer, item["Key"].S);
        }

        private TValue GetValue<TValue>(Dictionary<string, AttributeValue> item)
        {
            return AttributeValueTypeAccessor<TValue>.GetValue(item["Value"], _settings.Serializer);
        }

        public async Task AddAsync<TKey, TValue>(TKey key, TValue value)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = SetKeyItems(key, new Dictionary<string, AttributeValue>
                {
                    { "Value", AttributeValueTypeAccessor<TValue>.CreateValue(value, _settings.Serializer) },
                }),
                Expected = new Dictionary<string, ExpectedAttributeValue>
                {
                    { "Dictionary", new ExpectedAttributeValue(false) },
                    { "Key", new ExpectedAttributeValue(false) },
                }
            };

            try
            {
                var response = await _dynamoDBClient.PutItemAsync(request).ConfigureAwait(false);
            }
            catch (ConditionalCheckFailedException ex)
            {
                throw new ArgumentException("An item with the same key has already been added.", ex);
            }
        }

        public async Task AddOrUpdateAsync<TKey, TValue>(TKey key, TValue value)
        {
            var request = new PutItemRequest
            {
                TableName = _tableName,
                Item = SetKeyItems(key, new Dictionary<string, AttributeValue>
                {
                    { "Value", AttributeValueTypeAccessor<TValue>.CreateValue(value, _settings.Serializer) },
                }),
            };

            var response = await _dynamoDBClient.PutItemAsync(request).ConfigureAwait(false);
        }

        public async Task ClearAsync(CancellationToken cancellationToken)
        {
            Dictionary<string, AttributeValue> lastKeyEvaluated = null;

            do
            {
                var request = new QueryRequest
                {
                    TableName = _tableName,
                    KeyConditionExpression = "Dictionary = :v_Dictionary",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":v_Dictionary", new AttributeValue(DictionaryName) },
                    },
                    ExpressionAttributeNames = new Dictionary<string, string>()
                    {
                        { "#K", "Key" } // MEMO: 'Key' is reserved keyword in DynamoDB Query.
                    },
                    ProjectionExpression = "Dictionary, #K",
                };

                var response = await _dynamoDBClient.QueryAsync(request, cancellationToken).ConfigureAwait(false);
                if (cancellationToken.IsCancellationRequested) return;

                if (!response.Items.Any()) return;

                var batchRequest = new BatchWriteItemRequest
                {
                    RequestItems = new Dictionary<string, List<WriteRequest>>
                    {
                        {
                            _tableName,
                            response.Items.Select(x => new WriteRequest
                            {
                                DeleteRequest = new DeleteRequest
                                {
                                    Key = new Dictionary<string, AttributeValue>
                                    {
                                        { "Dictionary", x["Dictionary"] },
                                        { "Key", x["Key"] },
                                    }
                                }
                            }).ToList()
                        }
                    }
                };

                await _dynamoDBClient.BatchWriteItemAsync(batchRequest, cancellationToken).ConfigureAwait(false);

                if (cancellationToken.IsCancellationRequested) return;

                lastKeyEvaluated = response.LastEvaluatedKey;

            } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);
        }

        public async Task<bool> ContainsKeyAsync<TKey>(TKey key)
        {
            var request = new QueryRequest
            {
                TableName = _tableName,
                KeyConditionExpression = "Dictionary = :v_Dictionary and #K = :v_Key",
                ExpressionAttributeNames = new Dictionary<string, string>()
                {
                    { "#K", "Key" } // MEMO: 'Key' is reserved keyword in DynamoDB Query.
                },
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":v_Dictionary", new AttributeValue(DictionaryName) },
                    { ":v_Key", new AttributeValue(_settings.KeySerializer.SerializeAsKey(_settings.Serializer, key)) },
                },
                ProjectionExpression = "Dictionary, #K",
            };

            var response = await _dynamoDBClient.QueryAsync(request);
            return (response.Count != 0);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator<TKey, TValue>()
        {
            Dictionary<string, AttributeValue> lastKeyEvaluated = null;

            do
            {
                var request = new QueryRequest
                {
                    TableName = _tableName,
                    KeyConditionExpression = "Dictionary = :v_Dictionary",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":v_Dictionary", new AttributeValue(DictionaryName) },
                    }
                };

                var response = _dynamoDBClient.QueryAsync(request).GetAwaiter().GetResult();

                foreach (var item in response.Items)
                {
                    yield return new KeyValuePair<TKey, TValue>(GetKey<TKey>(item), GetValue<TValue>(item));
                }

                lastKeyEvaluated = response.LastEvaluatedKey;

            } while (lastKeyEvaluated != null && lastKeyEvaluated.Count != 0);
        }

        public async Task<bool> RemoveAsync<TKey>(TKey key)
        {
            var request = new DeleteItemRequest
            {
                TableName = _tableName,
                Key = SetKeyItems(key),
                ReturnValues = ReturnValue.ALL_OLD,
            };

            var response = await _dynamoDBClient.DeleteItemAsync(request).ConfigureAwait(false);

            return response.Attributes.Any();
        }

        public async Task<TryGetAsyncResult<TValue>> TryGetValueAsync<TKey, TValue>(TKey key)
        {
            var request = new GetItemRequest
            {
                TableName = _tableName,
                Key = SetKeyItems(key),
            };

            var response = await _dynamoDBClient.GetItemAsync(request).ConfigureAwait(false);

            if (response.Item.Count == 0) return TryGetAsyncResult<TValue>.NotFound;

            return new TryGetAsyncResult<TValue>(GetValue<TValue>(response.Item));
        }
    }
}
