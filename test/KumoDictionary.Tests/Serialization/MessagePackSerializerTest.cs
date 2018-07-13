using KumoDictionary.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace KumoDictionary.Tests.Serialization
{
    public class MessagePackSerializerTest
    {
        [Fact]
        public void VerifyDefaultSettings()
        {
            var valueSerializer = new MessagePackValueSerializer();

            Assert.True(valueSerializer.EnableLZ4Compression);
            Assert.Equal(MessagePack.Resolvers.ContractlessStandardResolver.Instance, valueSerializer.Resolver);
        }

        [Fact]
        public void SerializeDeserializeComplexType()
        {
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

            var serializeDirectly = MessagePack.LZ4MessagePackSerializer.Serialize(value, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            var deserializeDirectly = MessagePack.LZ4MessagePackSerializer.Deserialize<MyClass>(serializeDirectly, MessagePack.Resolvers.ContractlessStandardResolver.Instance);

            var serialized = valueSerializer.Serialize(value);
            Assert.NotNull(serialized);
            Assert.Equal(serializeDirectly, serialized);

            var deserialized = valueSerializer.Deserialize<MyClass>(serialized);
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

    }
}
