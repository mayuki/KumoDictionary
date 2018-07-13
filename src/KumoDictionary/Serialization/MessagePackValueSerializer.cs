using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KumoDictionary.Serialization
{
    public class MessagePackValueSerializer : IKumoDictionaryValueSerializer
    {
        public IFormatterResolver Resolver { get; set; } = MessagePack.Resolvers.ContractlessStandardResolver.Instance;

        public bool EnableLZ4Compression { get; set; } = true;

        public KumoDictionaryValueSerialization ValueSerialization => KumoDictionaryValueSerialization.Binary;

        public T Deserialize<T>(Stream stream)
        {
            return (EnableLZ4Compression)
                ? LZ4MessagePackSerializer.Deserialize<T>(stream, Resolver)
                : MessagePackSerializer.Deserialize<T>(stream, Resolver);
        }

        public T Deserialize<T>(byte[] rawBytes)
        {
            return (EnableLZ4Compression)
                ? LZ4MessagePackSerializer.Deserialize<T>(rawBytes, Resolver)
                : MessagePackSerializer.Deserialize<T>(rawBytes, Resolver);
        }

        public void Serialize<T>(Stream stream, T value)
        {
            if (EnableLZ4Compression)
            {
                LZ4MessagePackSerializer.Serialize(stream, value, Resolver);
            }
            else
            {
                MessagePackSerializer.Serialize(stream, value, Resolver);
            }
        }

        public byte[] Serialize<T>(T value)
        {
            return (EnableLZ4Compression)
                ? LZ4MessagePackSerializer.Serialize(value, Resolver)
                : MessagePackSerializer.Serialize(value, Resolver);
        }
    }
}
