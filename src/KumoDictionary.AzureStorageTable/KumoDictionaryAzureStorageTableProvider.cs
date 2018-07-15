using KumoDictionary.AzureStorageTable;
using KumoDictionary.AzureStorageTable.Internal;
using KumoDictionary.Serialization;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace KumoDictionary.Provider
{
    public class KumoDictionaryAzureStorageTableProvider : IKumoDictionaryBackendProvider
    {
        public CloudTable Table { get; }
        public KumoDictionaryAzureStorageTableSettings Settings { get; }

        public KumoDictionaryAzureStorageTableProvider(string connectionString, string tableName, IKumoDictionaryValueSerializer serializer = null)
            : this(GetCloudTable(connectionString, tableName), serializer)
        {
        }

        public KumoDictionaryAzureStorageTableProvider(CloudTable table, IKumoDictionaryValueSerializer serializer = null)
        {
            Table = table;
            Settings = new KumoDictionaryAzureStorageTableSettings(serializer ?? KumoDictionaryValueSerializer.Default);
        }

        private static CloudTable GetCloudTable(string connectionString, string tableName)
        {
            var account = CloudStorageAccount.Parse(connectionString);

            var tableClient = account.CreateCloudTableClient();
            var tableRef = tableClient.GetTableReference(tableName);

            return tableRef;
        }

        public IKumoDictionaryBackend Create(string dictionaryName)
        {
            return new AzureTableDictionaryBackend(Table, dictionaryName, Settings);
        }

        public static void UseAsDefault(string connectionString, string tableName, Action<KumoDictionaryAzureStorageTableProvider> configure = null)
        {
            var provider = new KumoDictionaryAzureStorageTableProvider(
                connectionString,
                tableName,
                KumoDictionaryValueSerializer.Default
            );

            configure?.Invoke(provider);

            KumoDictionaryBackendProvider.Default = provider;
        }
        public static void UseAsDefault(CloudTable table, Action<KumoDictionaryAzureStorageTableProvider> configure = null)
        {
            var provider = new KumoDictionaryAzureStorageTableProvider(
                table,
                KumoDictionaryValueSerializer.Default
            );

            configure?.Invoke(provider);

            KumoDictionaryBackendProvider.Default = provider;
        }
    }
}
