using System;
using System.Collections.Generic;
using System.Text;

namespace KumoDictionary.Serialization.KeySerializer
{
    public class StringKeySerializerResolver<T>
    {
        public static Func<T, string> Serialize { get; }
        public static Func<string, T> Deserialize { get; }

        public static bool CanSerialize { get; }

        static StringKeySerializerResolver()
        {
            if (typeof(T).IsEnum)
            {
                Serialize = EnumKeySerializer<T>.Serialize;
                Deserialize = EnumKeySerializer<T>.Deserialize;
                CanSerialize = true;
            }
            else
            {
                if (SystemTypeKeySerializer<T>.CanSerialize)
                {
                    Serialize = SystemTypeKeySerializer<T>.Serialize;
                    Deserialize = SystemTypeKeySerializer<T>.Deserialize;
                    CanSerialize = true;
                }
            }
        }
    }
}
