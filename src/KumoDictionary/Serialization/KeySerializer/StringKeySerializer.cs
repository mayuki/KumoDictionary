using KumoDictionary.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace KumoDictionary.Serialization.KeySerializer
{
    public class StringKeySerializer
    {
        public static StringKeySerializer Default { get; } = new StringKeySerializer();

        public virtual string SerializeAsKey<T>(IKumoDictionaryValueSerializer serializer, T value)
        {
            if (StringKeySerializer<T>.CanSerialize)
            {
                return StringKeySerializer<T>.Serialize(value);
            }
            else
            {
                if (serializer.ValueSerialization == KumoDictionaryValueSerialization.Binary)
                {
                    return Convert.ToBase64String(serializer.Serialize(value));
                }
                else
                {
                    return new UTF8Encoding(false).GetString(serializer.Serialize(value));
                }
            }
        }

        public virtual T DeserializeFromKey<T>(IKumoDictionaryValueSerializer serializer, string value)
        {
            if (StringKeySerializer<T>.CanSerialize)
            {
                return StringKeySerializer<T>.Deserialize(value);
            }
            else
            {
                if (serializer.ValueSerialization == KumoDictionaryValueSerialization.Binary)
                {
                    return serializer.Deserialize<T>(Convert.FromBase64String(value));
                }
                else
                {
                    return serializer.Deserialize<T>(Encoding.UTF8.GetBytes(value));
                }
            }
        }
    }
}
