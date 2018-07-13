using KumoDictionary.AzureStorageTable.Internal;
using KumoDictionary.Serialization;
using KumoDictionary.Serialization.KeySerializer;
using System;
using Xunit;

namespace KumoDictionary.AzureStorageTable.Tests
{
    public class DictionaryTableEntityTests
    {
        [Fact]
        public void CreateKnownEdmTypeString()
        {
            Assert.True(DictionaryTableEntity.EntityTypeResolver<string, string>.IsValueEdmType);

            var entity = DictionaryTableEntity.Create<string, string>("PK1", "RK1", "Value1", null, StringKeySerializer.Default);
            Assert.IsType<EdmDictionaryTableEntity<string, string>>(entity);
        }

        [Fact]
        public void CreateKnownEdmTypeBool()
        {
            Assert.True(DictionaryTableEntity.EntityTypeResolver<string, bool>.IsValueEdmType);

            var entity = DictionaryTableEntity.Create<string, bool>("PK1", "RK1", true, null, StringKeySerializer.Default);
            Assert.IsType<EdmDictionaryTableEntity<string, bool>>(entity);
        }

        [Fact]
        public void CreateKnownEdmTypeInt32()
        {
            Assert.True(DictionaryTableEntity.EntityTypeResolver<string, int>.IsValueEdmType);

            var entity = DictionaryTableEntity.Create<string, int>("PK1", "RK1", Int32.MaxValue, null, StringKeySerializer.Default);
            Assert.IsType<EdmDictionaryTableEntity<string, int>>(entity);
        }

        [Fact]
        public void CreateKnownEdmTypeInt64()
        {
            Assert.True(DictionaryTableEntity.EntityTypeResolver<string, long>.IsValueEdmType);

            var entity = DictionaryTableEntity.Create<string, long>("PK1", "RK1", Int64.MaxValue, null, StringKeySerializer.Default);
            Assert.IsType<EdmDictionaryTableEntity<string, long>>(entity);
        }

        [Fact]
        public void CreateKnownEdmTypeDateTime()
        {
            Assert.True(DictionaryTableEntity.EntityTypeResolver<string, DateTime>.IsValueEdmType);

            var entity = DictionaryTableEntity.Create<string, DateTime>("PK1", "RK1", DateTime.Now, null, StringKeySerializer.Default);
            Assert.IsType<EdmDictionaryTableEntity<string, DateTime>>(entity);
        }

        [Fact]
        public void CreateKnownEdmTypeGuid()
        {
            Assert.True(DictionaryTableEntity.EntityTypeResolver<string, Guid>.IsValueEdmType);

            var entity = DictionaryTableEntity.Create<string, Guid>("PK1", "RK1", Guid.NewGuid(), null, StringKeySerializer.Default);
            Assert.IsType<EdmDictionaryTableEntity<string, Guid>>(entity);
        }

        [Fact]
        public void CreateKnownEdmTypeByteArray()
        {
            Assert.True(DictionaryTableEntity.EntityTypeResolver<string, byte[]>.IsValueEdmType);

            var entity = DictionaryTableEntity.Create<string, byte[]>("PK1", "RK1", new byte[] { 1, 2, 3 }, null, StringKeySerializer.Default);
            Assert.IsType<EdmDictionaryTableEntity<string, byte[]>>(entity);
        }


        [Fact]
        public void CreateNotEdmTypeUserClass()
        {
            Assert.False(DictionaryTableEntity.EntityTypeResolver<string, MyClass>.IsValueEdmType);

            var entity = DictionaryTableEntity.Create<string, MyClass>("PK1", "RK1", new MyClass(), new MessagePackValueSerializer(), StringKeySerializer.Default);
            Assert.IsType<BinaryDictionaryTableEntity<string, MyClass>>(entity);
        }

        public class MyClass { }

        [Fact]
        public void CreateNotEdmTypeUserStruct()
        {
            Assert.False(DictionaryTableEntity.EntityTypeResolver<string, MyStruct>.IsValueEdmType);

            var entity = DictionaryTableEntity.Create<string, MyStruct>("PK1", "RK1", new MyStruct(), new MessagePackValueSerializer(), StringKeySerializer.Default);
            Assert.IsType<BinaryDictionaryTableEntity<string, MyStruct>>(entity);
        }

        public struct MyStruct { }

    }
}
