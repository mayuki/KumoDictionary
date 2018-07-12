using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace KumoDictionary.Serialization.KeySerializer
{
    internal class SystemTypeKeySerializer<T>
    {
        public static Func<T, string> Serialize { get; }
        public static Func<string, T> Deserialize { get; }
        public static bool CanSerialize { get; }

        static SystemTypeKeySerializer()
        {
            CanSerialize = true;

            if (typeof(T) == typeof(string))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) => (T)(object)value;
            }
            else if (typeof(T) == typeof(byte))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = Byte.Parse(value);
                    return Unsafe.As<byte, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(sbyte))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = SByte.Parse(value);
                    return Unsafe.As<sbyte, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(short))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = Int16.Parse(value);
                    return Unsafe.As<short, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(ushort))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = UInt16.Parse(value);
                    return Unsafe.As<ushort, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(int))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = Int32.Parse(value);
                    return Unsafe.As<int, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(uint))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = UInt32.Parse(value);
                    return Unsafe.As<uint, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(long))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = Int64.Parse(value);
                    return Unsafe.As<long, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(ulong))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = UInt64.Parse(value);
                    return Unsafe.As<ulong, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(float))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = Single.Parse(value);
                    return Unsafe.As<float, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(double))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = Double.Parse(value);
                    return Unsafe.As<double, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(TimeSpan))
            {
                Serialize = (value) => Unsafe.As<T, TimeSpan>(ref value).Ticks.ToString();
                Deserialize = (value) =>
                {
                    var v = TimeSpan.FromTicks(Int64.Parse(value));
                    return Unsafe.As<TimeSpan, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(DateTime))
            {
                Serialize = (value) => Unsafe.As<T, DateTime>(ref value).Ticks.ToString();
                Deserialize = (value) =>
                {
                    var v = new DateTime(Int64.Parse(value));
                    return Unsafe.As<DateTime, T>(ref v);
                };
            }
            else if (typeof(T) == typeof(Guid))
            {
                Serialize = (value) => value.ToString();
                Deserialize = (value) =>
                {
                    var v = Guid.Parse(value);
                    return Unsafe.As<Guid, T>(ref v);
                };
            }
            else
            {
                CanSerialize = false;
            }
        }
    }
}
