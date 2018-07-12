using KumoDictionary.Serialization;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace KumoDictionary.AzureStorageTable.Internal
{
    internal static class DictionaryTableEntityResolver<TKey, TValue>
    {
        public static EntityResolver<IDictionaryTableEntity<TKey, TValue>> GetResolver(KumoDictionaryValueSerialization valueSerialization)
        {
            if (valueSerialization == KumoDictionaryValueSerialization.Binary)
            {
                return Resolve_Binary;
            }
            else
            {
                return Resolve_String;
            }
        }

        private static IDictionaryTableEntity<TKey, TValue> Resolve_Binary(string partitionKey, string rowKey, DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag)
        {
            var entity = DictionaryTableEntity.CreateEmpty<TKey, TValue>(KumoDictionaryValueSerialization.Binary);
            entity.PartitionKey = partitionKey;
            entity.RowKey = rowKey;
            entity.Timestamp = timestamp;
            entity.ETag = etag;
            entity.ReadEntity(properties, null);
            return entity;
        }

        private static IDictionaryTableEntity<TKey, TValue> Resolve_String(string partitionKey, string rowKey, DateTimeOffset timestamp, IDictionary<string, EntityProperty> properties, string etag)
        {
            var entity = DictionaryTableEntity.CreateEmpty<TKey, TValue>(KumoDictionaryValueSerialization.String);
            entity.PartitionKey = partitionKey;
            entity.RowKey = rowKey;
            entity.Timestamp = timestamp;
            entity.ETag = etag;
            entity.ReadEntity(properties, null);
            return entity;
        }
    }
}
