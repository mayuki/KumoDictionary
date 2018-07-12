using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace KumoDictionary.Serialization.KeySerializer
{
    internal class EnumKeySerializer<T>
    {
        public static Func<T, string> Serialize { get; }
        public static Func<string, T> Deserialize { get; }

        static EnumKeySerializer()
        {
            if (!typeof(T).IsEnum) throw new NotSupportedException("Generic argument type is not Enum.");

            var underlyingType = typeof(T).GetEnumUnderlyingType();

            if (underlyingType == typeof(byte))
            {
                Serialize = (value) => Unsafe.As<T, byte>(ref value).ToString();
                Deserialize = (value) => { var v = byte.Parse(value); return Unsafe.As<byte, T>(ref v); };
            }
            else if (underlyingType == typeof(sbyte))
            {
                Serialize = (value) => Unsafe.As<T, sbyte>(ref value).ToString();
                Deserialize = (value) => { var v = sbyte.Parse(value); return Unsafe.As<sbyte, T>(ref v); };
            }
            else if (underlyingType == typeof(short))
            {
                Serialize = (value) => Unsafe.As<T, short>(ref value).ToString();
                Deserialize = (value) => { var v = short.Parse(value); return Unsafe.As<short, T>(ref v); };
            }
            else if (underlyingType == typeof(ushort))
            {
                Serialize = (value) => Unsafe.As<T, ushort>(ref value).ToString();
                Deserialize = (value) => { var v = ushort.Parse(value); return Unsafe.As<ushort, T>(ref v); };
            }
            else if (underlyingType == typeof(int))
            {
                Serialize = (value) => Unsafe.As<T, int>(ref value).ToString();
                Deserialize = (value) => { var v = int.Parse(value); return Unsafe.As<int, T>(ref v); };
            }
            else if (underlyingType == typeof(uint))
            {
                Serialize = (value) => Unsafe.As<T, uint>(ref value).ToString();
                Deserialize = (value) => { var v = uint.Parse(value); return Unsafe.As<uint, T>(ref v); };
            }
            else if (underlyingType == typeof(long))
            {
                Serialize = (value) => Unsafe.As<T, long>(ref value).ToString();
                Deserialize = (value) => { var v = long.Parse(value); return Unsafe.As<long, T>(ref v); };
            }
            else if (underlyingType == typeof(ulong))
            {
                Serialize = (value) => Unsafe.As<T, ulong>(ref value).ToString();
                Deserialize = (value) => { var v = ulong.Parse(value); return Unsafe.As<ulong, T>(ref v); };
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}
