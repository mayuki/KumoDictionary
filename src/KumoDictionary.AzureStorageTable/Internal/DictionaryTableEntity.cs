using KumoDictionary.AzureStorageTable;
using KumoDictionary.Serialization;
using KumoDictionary.Serialization.KeySerializer;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;

namespace KumoDictionary.AzureStorageTable.Internal
{
    internal interface IDictionaryTableEntity<TKey, TValue> : ITableEntity
    {
        TKey GetKey(IKumoDictionaryValueSerializer serializer, StringKeySerializer keySerializer);
        TValue GetValue(IKumoDictionaryValueSerializer serializer);
    }

    internal static class DictionaryTableEntity
    {
        public static class EntityTypeResolver<TKey, TValue>
        {
            public static readonly Type Type;
            public static readonly bool IsValueEdmType;

            static EntityTypeResolver()
            {
                var t = typeof(TValue);

                if (t == typeof(string) ||
                    t == typeof(bool) ||
                    t == typeof(int) ||
                    t == typeof(long) ||
                    t == typeof(double) ||
                    t == typeof(DateTime) ||
                    t == typeof(Guid) ||
                    t == typeof(byte[]))
                {
                    Type = typeof(EdmDictionaryTableEntity<TKey, TValue>);
                    IsValueEdmType = true;
                }
                else
                {
                    Type = typeof(BinaryDictionaryTableEntity<TKey, TValue>);
                    IsValueEdmType = false;
                }
            }
        }

        public static Type GetType<TKey, TValue>()
        {
            return EntityTypeResolver<TKey, TValue>.Type;
        }

        public static IDictionaryTableEntity<TKey, TValue> Create<TKey, TValue>(string partitionKey, TKey rowKey, TValue value, IKumoDictionaryValueSerializer serializer, StringKeySerializer keySerializer)
        {
            if (EntityTypeResolver<TKey, TValue>.IsValueEdmType)
            {
                return new EdmDictionaryTableEntity<TKey, TValue>(partitionKey, rowKey, value, serializer, keySerializer);
            }
            else
            {
                if (serializer.ValueSerialization == KumoDictionaryValueSerialization.Binary)
                {
                    return new BinaryDictionaryTableEntity<TKey, TValue>(partitionKey, rowKey, value, serializer, keySerializer);
                }
                else
                {
                    return new StringDictionaryTableEntity<TKey, TValue>(partitionKey, rowKey, value, serializer, keySerializer);
                }
            }
        }

        public static IDictionaryTableEntity<TKey, TValue> CreateEmpty<TKey, TValue>(KumoDictionaryValueSerialization valueSerialization)
        {
            if (EntityTypeResolver<TKey, TValue>.IsValueEdmType)
            {
                return new EdmDictionaryTableEntity<TKey, TValue>();
            }
            else
            {
                if (valueSerialization == KumoDictionaryValueSerialization.Binary)
                {
                    return new BinaryDictionaryTableEntity<TKey, TValue>();
                }
                else
                {
                    return new StringDictionaryTableEntity<TKey, TValue>();
                }
            }
        }
    }

    internal class BinaryDictionaryTableEntity<TKey, TValue> : TableEntity, IDictionaryTableEntity<TKey, TValue>
    {
        public BinaryDictionaryTableEntity(string partitionKey, TKey rowKey, TValue value, IKumoDictionaryValueSerializer serializer, StringKeySerializer keySerializer)
        {
            PartitionKey = partitionKey;
            RowKey = keySerializer.SerializeAsKey(serializer, rowKey);
            RawValue = serializer.Serialize(value);
        }

        public BinaryDictionaryTableEntity() { }

        public byte[] RawValue { get; set; }

        public TKey GetKey(IKumoDictionaryValueSerializer serializer, StringKeySerializer keySerializer)
        {
            return keySerializer.DeserializeFromKey<TKey>(serializer, RowKey);
        }

        public TValue GetValue(IKumoDictionaryValueSerializer serializer)
        {
            return serializer.Deserialize<TValue>((byte[])(object)RawValue);
        }
    }

    internal class EdmDictionaryTableEntity<TKey, TValue> : TableEntity, IDictionaryTableEntity<TKey, TValue>
    {
        public EdmDictionaryTableEntity(string partitionKey, TKey rowKey, TValue value, IKumoDictionaryValueSerializer serializer, StringKeySerializer keySerializer)
        {
            PartitionKey = partitionKey;
            RowKey = keySerializer.SerializeAsKey(serializer, rowKey);
            RawValue = value;
        }

        public EdmDictionaryTableEntity() { }

        public TValue RawValue { get; set; }

        public TKey GetKey(IKumoDictionaryValueSerializer serializer, StringKeySerializer keySerializer)
        {
            return keySerializer.DeserializeFromKey<TKey>(serializer, RowKey);
        }

        public TValue GetValue(IKumoDictionaryValueSerializer serializer)
        {
            return RawValue;
        }
    }

    internal class StringDictionaryTableEntity<TKey, TValue> : TableEntity, IDictionaryTableEntity<TKey, TValue>
    {
        public StringDictionaryTableEntity(string partitionKey, TKey rowKey, TValue value, IKumoDictionaryValueSerializer serializer, StringKeySerializer keySerializer)
        {
            PartitionKey = partitionKey;
            RowKey = keySerializer.SerializeAsKey(serializer, rowKey);
            RawValue = new UTF8Encoding(false).GetString(serializer.Serialize(value));
        }

        public StringDictionaryTableEntity() { }

        public string RawValue { get; set; }

        public TKey GetKey(IKumoDictionaryValueSerializer serializer, StringKeySerializer keySerializer)
        {
            return keySerializer.DeserializeFromKey<TKey>(serializer, RowKey);
        }

        public TValue GetValue(IKumoDictionaryValueSerializer serializer)
        {
            return serializer.Deserialize<TValue>(Encoding.UTF8.GetBytes(RawValue));
        }
    }
}
