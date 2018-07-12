using Amazon;
using Amazon.DynamoDBv2;
using KumoDictionary.AmazonDynamoDB;
using KumoDictionary.AmazonDynamoDB.Internal;
using KumoDictionary.Serialization;
using System;

namespace KumoDictionary.Provider
{
    public class KumoDictionaryAmazonDynamoDBProvider : IKumoDictionaryBackendProvider
    {
        public KumoDictionaryAmazonDynamoDBSettings Settings { get; }
        public AmazonDynamoDBClient DynamoDBClient { get; }

        public KumoDictionaryAmazonDynamoDBProvider(string tableName, AmazonDynamoDBClient dynamoDBClient, IKumoDictionaryValueSerializer serializer)
        {
            DynamoDBClient = dynamoDBClient;
            Settings = new KumoDictionaryAmazonDynamoDBSettings(tableName, serializer);
        }

        public static void UseAsDefault(string tableName, AmazonDynamoDBClient dynamoDBClient, Action<KumoDictionaryAmazonDynamoDBProvider> configure = null)
        {
            if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentNullException(tableName);
            if (dynamoDBClient == null) throw new ArgumentNullException(nameof(dynamoDBClient));

            var provider = new KumoDictionaryAmazonDynamoDBProvider(
                tableName,
                dynamoDBClient,
                KumoDictionaryValueSerializer.Default
            );

            KumoDictionaryBackendProvider.Default = provider;

            configure?.Invoke(provider);
        }

        public IKumoDictionaryBackend Create(string dictionaryName)
        {
            if (String.IsNullOrWhiteSpace(dictionaryName)) throw new ArgumentNullException(nameof(dictionaryName));

            return new DynamoDBDictionaryBackend(dictionaryName, Settings, DynamoDBClient);
        }
    }
}
