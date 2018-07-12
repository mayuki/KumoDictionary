using KumoDictionary.Serialization;
using KumoDictionary.Serialization.KeySerializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace KumoDictionary.AmazonDynamoDB
{
    public class KumoDictionaryAmazonDynamoDBSettings
    {
        public IKumoDictionaryValueSerializer Serializer { get; set; }
        public StringKeySerializer KeySerializer { get; set; }
        public string TableName { get; }

        public KumoDictionaryAmazonDynamoDBSettings(string tableName, IKumoDictionaryValueSerializer serializer)
        {
            TableName = tableName;
            Serializer = serializer;
            KeySerializer = StringKeySerializer.Default;
        }
    }
}
