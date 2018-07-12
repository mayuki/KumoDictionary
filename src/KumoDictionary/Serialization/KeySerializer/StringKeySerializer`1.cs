using System;
using System.Collections.Generic;
using System.Text;

namespace KumoDictionary.Serialization.KeySerializer
{
    internal class StringKeySerializer<T>
    {
        public static Func<T, string> Serialize { get; }
        public static Func<string, T> Deserialize { get; }

        public static bool CanSerialize { get; }

        static StringKeySerializer()
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
