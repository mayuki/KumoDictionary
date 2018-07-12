using KumoDictionary.Provider;
using KumoDictionary.Serialization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KumoDictionary.AzureStorageTable.Internal
{
    internal class AzureTableDictionaryBackend : IKumoDictionaryBackend
    {
        private bool _tableCreateIfNotExistsOnce;
        private KumoDictionaryAzureStorageTableSettings _settings;

        public string DictionaryName { get; }

        public CloudTable Table { get; }

        public AzureTableDictionaryBackend(CloudTable table, string dictionaryName, KumoDictionaryAzureStorageTableSettings settings)
        {
            Table = table;
            DictionaryName = dictionaryName;
            _settings = settings;
        }

        private async Task CreateTableIfNotExistsAsync()
        {
            if (_settings.DisableCreateTableIfNotExists) return;

            if (!_tableCreateIfNotExistsOnce)
            {
                await Table.CreateIfNotExistsAsync().ConfigureAwait(false);
                _tableCreateIfNotExistsOnce = true;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator<TKey, TValue>()
        {
            var query = new TableQuery()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, DictionaryName));

            var entityResolver = DictionaryTableEntityResolver<TKey, TValue>.GetResolver(_settings.Serializer.ValueSerialization);
            TableContinuationToken token = null;
            do
            {
                var results = Table.ExecuteQuerySegmentedAsync(query, entityResolver, token).Result;
                foreach (var result in results)
                {
                    yield return new KeyValuePair<TKey, TValue>(result.GetKey(_settings.Serializer, _settings.KeySerializer), result.GetValue(_settings.Serializer));
                }

                token = results.ContinuationToken;
            }
            while (token != null);
        }

        public async Task ClearAsync(CancellationToken cancellationToken)
        {
            var query = new TableQuery() { SelectColumns = new List<string>(), }
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, DictionaryName));

            TableContinuationToken token = null;
            do
            {
                var results = await Table.ExecuteQuerySegmentedAsync(query, token, null, null, cancellationToken).ConfigureAwait(false);
                if (!results.Any()) return;

                var tableBatchOperation = new TableBatchOperation();
                foreach (var x in results) tableBatchOperation.Add(TableOperation.Delete(x));

                await Table.ExecuteBatchAsync(tableBatchOperation, null, null, cancellationToken).ConfigureAwait(false);
                token = results.ContinuationToken;
            }
            while (token != null && !cancellationToken.IsCancellationRequested);
        }

        public async Task<bool> RemoveAsync<TKey>(TKey key)
        {
            await CreateTableIfNotExistsAsync().ConfigureAwait(false);

            try
            {
                var serializedKey = _settings.KeySerializer.SerializeAsKey(_settings.Serializer, key);

                var tempEntity = DictionaryTableEntity.Create(DictionaryName, key, 0, _settings.Serializer, _settings.KeySerializer);
                tempEntity.ETag = "*";

                await Table.ExecuteAsync(TableOperation.Delete(tempEntity)).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == 404)
                {
                    return false;
                }
                throw;
            }

            return true;
        }

        public async Task<bool> ContainsKeyAsync<TKey>(TKey key)
        {
            await CreateTableIfNotExistsAsync().ConfigureAwait(false);

            var tableOperation = TableOperation.Retrieve(DictionaryName, _settings.KeySerializer.SerializeAsKey(_settings.Serializer, key), DictionaryTableEntityResolver<TKey, object>.GetResolver(KumoDictionaryValueSerialization.Binary));
            var result = await Table.ExecuteAsync(tableOperation).ConfigureAwait(false);
            var entity = (IDictionaryTableEntity<TKey, object>)result.Result;

            return (entity != null);
        }

        public async Task AddOrUpdateAsync<TKey, TValue>(TKey key, TValue value)
        {
            await CreateTableIfNotExistsAsync().ConfigureAwait(false);

            var entity = DictionaryTableEntity.Create(DictionaryName, key, value, _settings.Serializer, _settings.KeySerializer);
            await Table.ExecuteAsync(TableOperation.InsertOrReplace(entity)).ConfigureAwait(false);
        }

        public async Task AddAsync<TKey, TValue>(TKey key, TValue value)
        {
            await CreateTableIfNotExistsAsync().ConfigureAwait(false);

            try
            {
                var entity = DictionaryTableEntity.Create(DictionaryName, key, value, _settings.Serializer, _settings.KeySerializer);
                var result = await Table.ExecuteAsync(TableOperation.Insert(entity)).ConfigureAwait(false);
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode == TableErrorCodeStrings.EntityAlreadyExists)
                {
                    throw new ArgumentException("An item with the same key has already been added.", ex);
                }
                throw;
            }
        }

        public async Task<TryGetAsyncResult<TValue>> TryGetValueAsync<TKey, TValue>(TKey key)
        {
            await CreateTableIfNotExistsAsync().ConfigureAwait(false);

            var serializedKey = _settings.KeySerializer.SerializeAsKey(_settings.Serializer, key);
            var tableOperation = TableOperation.Retrieve(DictionaryName, serializedKey, DictionaryTableEntityResolver<TKey, TValue>.GetResolver(_settings.Serializer.ValueSerialization));
            var result = await Table.ExecuteAsync(tableOperation).ConfigureAwait(false);
            var entity = (IDictionaryTableEntity<TKey, TValue>)result.Result;

            return (entity == null) ? TryGetAsyncResult<TValue>.NotFound : new TryGetAsyncResult<TValue>(entity.GetValue(_settings.Serializer));
        }
    }
}
