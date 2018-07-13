using KumoDictionary.Serialization.KeySerializer;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace KumoDictionary.Tests.KeySerializer
{
    public class SystemTypeStringKeySerializerTests
    {
        [Fact]
        public void DoesNotSupportUnknownType()
        {
            Assert.False(SystemTypeKeySerializer<MyClass>.CanSerialize);
        }
        class MyClass { }

        [Fact]
        public void SerializeString()
        {
            Assert.True(SystemTypeKeySerializer<String>.CanSerialize);

            var inValue = "Hello";
            var result = SystemTypeKeySerializer<String>.Serialize(inValue);
            Assert.Equal(inValue, result);
            Assert.Same(inValue, result);
        }

        [Fact]
        public void DeserializeString()
        {
            Assert.True(SystemTypeKeySerializer<String>.CanSerialize);

            var inValue = "Konnichiwa";
            var result = SystemTypeKeySerializer<String>.Deserialize(inValue);
            Assert.Equal(inValue, result);
            Assert.Same(inValue, result);
        }

        [Fact]
        public void SerializeDateTime()
        {
            Assert.True(SystemTypeKeySerializer<DateTime>.CanSerialize);

            var value = DateTime.Now;
            var result = SystemTypeKeySerializer<DateTime>.Serialize(value);
            Assert.Equal(value.Ticks.ToString(), result);
        }

        [Fact]
        public void DeserializeDateTime()
        {
            Assert.True(SystemTypeKeySerializer<DateTime>.CanSerialize);

            var value = new DateTime(636670718103940161);
            var result = SystemTypeKeySerializer<DateTime>.Deserialize("636670718103940161");
            Assert.Equal(value, result);
        }

        [Fact]
        public void SerializeTimeSpan()
        {
            Assert.True(SystemTypeKeySerializer<TimeSpan>.CanSerialize);

            var value = new TimeSpan(1234567890);
            var result = SystemTypeKeySerializer<TimeSpan>.Serialize(value);
            Assert.Equal("1234567890", result);
        }

        [Fact]
        public void DeserializeTimeSpan()
        {
            Assert.True(SystemTypeKeySerializer<TimeSpan>.CanSerialize);

            var value = new TimeSpan(1234567890);
            var result = SystemTypeKeySerializer<TimeSpan>.Deserialize("1234567890");
            Assert.Equal(value, result);
        }

        [Fact]
        public void SerializeGuid()
        {
            Assert.True(SystemTypeKeySerializer<Guid>.CanSerialize);

            var value = new Guid("b2eb9f9a-a397-49d8-93b0-c0fe636ff569");
            var result = SystemTypeKeySerializer<Guid>.Serialize(value);
            Assert.Equal("b2eb9f9a-a397-49d8-93b0-c0fe636ff569", result);
        }

        [Fact]
        public void DeserializeGuid()
        {
            Assert.True(SystemTypeKeySerializer<Guid>.CanSerialize);

            var value = new Guid("b2eb9f9a-a397-49d8-93b0-c0fe636ff569");
            var result = SystemTypeKeySerializer<Guid>.Deserialize("b2eb9f9a-a397-49d8-93b0-c0fe636ff569");
            Assert.Equal(value, result);
        }
    }
}
