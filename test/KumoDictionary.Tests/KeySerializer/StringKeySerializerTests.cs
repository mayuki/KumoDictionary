using KumoDictionary.Serialization;
using KumoDictionary.Serialization.KeySerializer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace KumoDictionary.Tests.KeySerializer
{
    public class StringKeySerializerTests
    {
        [Fact]
        public void StringKeySerializer1SupportsEnumSerialization()
        {
            Assert.True(StringKeySerializer<MyEnum>.CanSerialize);
        }

        [Fact]
        public void SerializeEnum()
        {
            var serializer = new StringKeySerializer();
            Assert.Equal("5678", serializer.SerializeAsKey(null, MyEnum.B));
        }

        [Fact]
        public void DeserializeEnum()
        {
            var serializer = new StringKeySerializer();
            Assert.Equal(MyEnum.B, serializer.DeserializeFromKey<MyEnum>(null, "5678"));
        }

        public enum MyEnum
        {
            A = 1234,
            B = 5678,
            C = 10000
        }

        [Fact]
        public void SerializeDeserializeComplexType()
        {
            var serializer = new StringKeySerializer();
            var valueSerializer = new MessagePackValueSerializer();

            var value = new MyClass
            {
                PropBool = true,
                PropString = "Hello コンニチハ!",
                FieldA = 123456,
                Inner = new MyClass.MyStruct
                {
                    Value = Guid.NewGuid()
                }
            };

            var serializeDirectly = Convert.ToBase64String(MessagePack.LZ4MessagePackSerializer.Serialize(value, MessagePack.Resolvers.ContractlessStandardResolver.Instance));
            var deserializeDirectly = MessagePack.LZ4MessagePackSerializer.Deserialize<MyClass>(Convert.FromBase64String(serializeDirectly), MessagePack.Resolvers.ContractlessStandardResolver.Instance);

            var serialized = serializer.SerializeAsKey(valueSerializer, value);
            Assert.NotNull(serialized);
            Assert.Equal(serializeDirectly, serialized);

            var deserialized = serializer.DeserializeFromKey<MyClass>(valueSerializer, serialized);
            Assert.Equal(value.PropString, deserialized.PropString);
            Assert.Equal(value.PropBool, deserialized.PropBool);
            Assert.Equal(value.FieldA, deserialized.FieldA);
            Assert.Equal(value.Inner.Value, deserialized.Inner.Value);
        }

        public class MyClass
        {
            public string PropString { get; set; }
            public bool PropBool { get; set; }
            public int FieldA;

            public MyStruct Inner { get; set; }

            public struct MyStruct
            {
                public Guid Value { get; set; }
            }
        }


        [Fact]
        public void ValueSerializationIsBase64()
        {
            var serializer = new StringKeySerializer();
            Assert.Equal("AQID", serializer.SerializeAsKey(new FixedBinaryValueSerializer(new byte[] { 1, 2, 3 }, KumoDictionaryValueSerialization.Binary), default(MyClass)));
        }

        [Fact]
        public void ValueSerializationIsRawString()
        {
            var serializer = new StringKeySerializer();
            Assert.Equal("{\"Key\":123}", serializer.SerializeAsKey(new FixedBinaryValueSerializer(new UTF8Encoding(false).GetBytes("{\"Key\":123}"), KumoDictionaryValueSerialization.String), default(MyClass)));
        }

        class FixedBinaryValueSerializer : IKumoDictionaryValueSerializer
        {
            private byte[] _data;

            public KumoDictionaryValueSerialization ValueSerialization { get; }

            public FixedBinaryValueSerializer(byte[] data, KumoDictionaryValueSerialization valueSerialization)
            {
                _data = data;
                ValueSerialization = valueSerialization;
            }

            public T Deserialize<T>(Stream stream)
            {
                throw new NotImplementedException();
            }

            public T Deserialize<T>(byte[] rawData)
            {
                throw new NotImplementedException();
            }

            public byte[] Serialize<T>(T value)
            {
                return _data;
            }

            public void Serialize<T>(Stream stream, T value)
            {
                stream.Write(_data, 0, _data.Length);
            }
        }
    }
}
