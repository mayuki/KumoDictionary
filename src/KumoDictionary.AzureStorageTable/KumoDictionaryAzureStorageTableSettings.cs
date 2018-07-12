using KumoDictionary.Serialization;
using KumoDictionary.Serialization.KeySerializer;
using System;
using System.Collections.Generic;
using System.Text;

namespace KumoDictionary.AzureStorageTable
{
    public class KumoDictionaryAzureStorageTableSettings
    {
        public IKumoDictionaryValueSerializer Serializer { get; set; }
        public StringKeySerializer KeySerializer { get; set; }
        public bool DisableCreateTableIfNotExists { get; set; }

        public KumoDictionaryAzureStorageTableSettings(IKumoDictionaryValueSerializer serializer)
        {
            Serializer = serializer;
            KeySerializer = StringKeySerializer.Default;
        }
    }
}
