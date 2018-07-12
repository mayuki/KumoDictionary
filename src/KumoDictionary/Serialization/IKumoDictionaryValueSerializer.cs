using System.IO;

namespace KumoDictionary.Serialization
{
    public interface IKumoDictionaryValueSerializer
    {
        byte[] Serialize<T>(T value);
        void Serialize<T>(Stream stream, T value);
        T Deserialize<T>(Stream stream);
        T Deserialize<T>(byte[] rawData);

        KumoDictionaryValueSerialization ValueSerialization { get; }
    }

    public enum KumoDictionaryValueSerialization
    {
        Binary,
        String
    }
}
