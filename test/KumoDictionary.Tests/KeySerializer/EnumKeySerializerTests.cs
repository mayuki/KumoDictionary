using KumoDictionary.Serialization.KeySerializer;
using System;
using Xunit;

namespace KumoDictionary.Tests.KeySerializer
{
    public class EnumKeySerializerTests
    {
        [Fact]
        public void SerializeValue()
        {
            Assert.Equal("0", EnumKeySerializer<EnumByte>.Serialize(EnumByte.Default));
            Assert.Equal(byte.MinValue.ToString(), EnumKeySerializer<EnumByte>.Serialize(EnumByte.Min));
            Assert.Equal(byte.MaxValue.ToString(), EnumKeySerializer<EnumByte>.Serialize(EnumByte.Max));

            Assert.Equal("0", EnumKeySerializer<EnumSByte>.Serialize(EnumSByte.Default));
            Assert.Equal(sbyte.MinValue.ToString(), EnumKeySerializer<EnumSByte>.Serialize(EnumSByte.Min));
            Assert.Equal(sbyte.MaxValue.ToString(), EnumKeySerializer<EnumSByte>.Serialize(EnumSByte.Max));

            Assert.Equal("0", EnumKeySerializer<EnumInt16>.Serialize(EnumInt16.Default));
            Assert.Equal(short.MinValue.ToString(), EnumKeySerializer<EnumInt16>.Serialize(EnumInt16.Min));
            Assert.Equal(short.MaxValue.ToString(), EnumKeySerializer<EnumInt16>.Serialize(EnumInt16.Max));

            Assert.Equal("0", EnumKeySerializer<EnumUInt16>.Serialize(EnumUInt16.Default));
            Assert.Equal(ushort.MinValue.ToString(), EnumKeySerializer<EnumUInt16>.Serialize(EnumUInt16.Min));
            Assert.Equal(ushort.MaxValue.ToString(), EnumKeySerializer<EnumUInt16>.Serialize(EnumUInt16.Max));

            Assert.Equal("0", EnumKeySerializer<EnumUInt32>.Serialize(EnumUInt32.Default));
            Assert.Equal(uint.MinValue.ToString(), EnumKeySerializer<EnumUInt32>.Serialize(EnumUInt32.Min));
            Assert.Equal(uint.MaxValue.ToString(), EnumKeySerializer<EnumUInt32>.Serialize(EnumUInt32.Max));

            Assert.Equal("0", EnumKeySerializer<EnumInt32>.Serialize(EnumInt32.Default));
            Assert.Equal(int.MinValue.ToString(), EnumKeySerializer<EnumInt32>.Serialize(EnumInt32.Min));
            Assert.Equal(int.MaxValue.ToString(), EnumKeySerializer<EnumInt32>.Serialize(EnumInt32.Max));

            Assert.Equal("0", EnumKeySerializer<EnumInt64>.Serialize(EnumInt64.Default));
            Assert.Equal(long.MinValue.ToString(), EnumKeySerializer<EnumInt64>.Serialize(EnumInt64.Min));
            Assert.Equal(long.MaxValue.ToString(), EnumKeySerializer<EnumInt64>.Serialize(EnumInt64.Max));

            Assert.Equal("0", EnumKeySerializer<EnumUInt64>.Serialize(EnumUInt64.Default));
            Assert.Equal(ulong.MinValue.ToString(), EnumKeySerializer<EnumUInt64>.Serialize(EnumUInt64.Min));
            Assert.Equal(ulong.MaxValue.ToString(), EnumKeySerializer<EnumUInt64>.Serialize(EnumUInt64.Max));
        }

        [Fact]
        public void DeserializeValue()
        {
            Assert.Equal(EnumByte.Default, EnumKeySerializer<EnumByte>.Deserialize("0"));
            Assert.Equal(EnumByte.Min, EnumKeySerializer<EnumByte>.Deserialize(byte.MinValue.ToString()));
            Assert.Equal(EnumByte.Max, EnumKeySerializer<EnumByte>.Deserialize(byte.MaxValue.ToString()));

            Assert.Equal(EnumSByte.Default, EnumKeySerializer<EnumSByte>.Deserialize("0"));
            Assert.Equal(EnumSByte.Min, EnumKeySerializer<EnumSByte>.Deserialize(sbyte.MinValue.ToString()));
            Assert.Equal(EnumSByte.Max, EnumKeySerializer<EnumSByte>.Deserialize(sbyte.MaxValue.ToString()));

            Assert.Equal(EnumInt16.Default, EnumKeySerializer<EnumInt16>.Deserialize("0"));
            Assert.Equal(EnumInt16.Min, EnumKeySerializer<EnumInt16>.Deserialize(short.MinValue.ToString()));
            Assert.Equal(EnumInt16.Max, EnumKeySerializer<EnumInt16>.Deserialize(short.MaxValue.ToString()));

            Assert.Equal(EnumUInt16.Default, EnumKeySerializer<EnumUInt16>.Deserialize("0"));
            Assert.Equal(EnumUInt16.Min, EnumKeySerializer<EnumUInt16>.Deserialize(ushort.MinValue.ToString()));
            Assert.Equal(EnumUInt16.Max, EnumKeySerializer<EnumUInt16>.Deserialize(ushort.MaxValue.ToString()));

            Assert.Equal(EnumInt32.Default, EnumKeySerializer<EnumInt32>.Deserialize("0"));
            Assert.Equal(EnumInt32.Min, EnumKeySerializer<EnumInt32>.Deserialize(int.MinValue.ToString()));
            Assert.Equal(EnumInt32.Max, EnumKeySerializer<EnumInt32>.Deserialize(int.MaxValue.ToString()));

            Assert.Equal(EnumUInt32.Default, EnumKeySerializer<EnumUInt32>.Deserialize("0"));
            Assert.Equal(EnumUInt32.Min, EnumKeySerializer<EnumUInt32>.Deserialize(uint.MinValue.ToString()));
            Assert.Equal(EnumUInt32.Max, EnumKeySerializer<EnumUInt32>.Deserialize(uint.MaxValue.ToString()));

            Assert.Equal(EnumInt64.Default, EnumKeySerializer<EnumInt64>.Deserialize("0"));
            Assert.Equal(EnumInt64.Min, EnumKeySerializer<EnumInt64>.Deserialize(long.MinValue.ToString()));
            Assert.Equal(EnumInt64.Max, EnumKeySerializer<EnumInt64>.Deserialize(long.MaxValue.ToString()));

            Assert.Equal(EnumUInt64.Default, EnumKeySerializer<EnumUInt64>.Deserialize("0"));
            Assert.Equal(EnumUInt64.Min, EnumKeySerializer<EnumUInt64>.Deserialize(ulong.MinValue.ToString()));
            Assert.Equal(EnumUInt64.Max, EnumKeySerializer<EnumUInt64>.Deserialize(ulong.MaxValue.ToString()));
        }

        public enum EnumByte : byte
        {
            Default = default(byte),
            Min = byte.MinValue,
            Max = byte.MaxValue,
        }
        public enum EnumInt16 : short
        {
            Default = default(short),
            Min = short.MinValue,
            Max = short.MaxValue,
        }
        public enum EnumInt32 : int
        {
            Default = default(int),
            Min = int.MinValue,
            Max = int.MaxValue,
        }
        public enum EnumInt64 : long
        {
            Default = default(long),
            Min = long.MinValue,
            Max = long.MaxValue,
        }
        public enum EnumSByte : sbyte
        {
            Default = default(sbyte),
            Min = sbyte.MinValue,
            Max = sbyte.MaxValue,
        }
        public enum EnumUInt16 : ushort
        {
            Default = default(ushort),
            Min = ushort.MinValue,
            Max = ushort.MaxValue,
        }
        public enum EnumUInt32 : uint
        {
            Default = default(uint),
            Min = uint.MinValue,
            Max = uint.MaxValue,
        }
        public enum EnumUInt64 : ulong
        {
            Default = default(ulong),
            Min = ulong.MinValue,
            Max = ulong.MaxValue,
        }
    }
}
