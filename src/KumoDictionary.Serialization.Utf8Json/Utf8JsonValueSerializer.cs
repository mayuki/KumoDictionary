using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utf8Json;

namespace KumoDictionary.Serialization
{
    public class Utf8JsonValueSerializer : IKumoDictionaryValueSerializer
    {
        public KumoDictionaryValueSerialization ValueSerialization => KumoDictionaryValueSerialization.String;

        public IJsonFormatterResolver Resolver { get; } = JsonSerializer.DefaultResolver;

        public T Deserialize<T>(Stream stream)
        {
            return JsonSerializer.Deserialize<T>(stream, Resolver);
        }

        public T Deserialize<T>(byte[] rawData)
        {
            return JsonSerializer.Deserialize<T>(rawData, Resolver);
        }

        public byte[] Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, Resolver);
        }

        public void Serialize<T>(Stream stream, T value)
        {
            JsonSerializer.Serialize(stream, value, Resolver);
        }
    }
}
