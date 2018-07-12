using Amazon.DynamoDBv2.Model;
using KumoDictionary.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace KumoDictionary.AmazonDynamoDB.Internal
{
    internal class AttributeValueTypeAccessor<T>
    {
        public static readonly Func<AttributeValue, IKumoDictionaryValueSerializer, T> GetValue;
        public static readonly Action<AttributeValue, T, IKumoDictionaryValueSerializer> SetValue;

        static AttributeValueTypeAccessor()
        {
            var t = typeof(T);

            if (t == typeof(string))
            {
                GetValue = (attrValue, serializer) => { return (T)(object)attrValue.S; };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.S = (string)(object)rawValue; };
            }
            else if (t == typeof(bool))
            {
                GetValue = (attrValue, serializer) => { var v = attrValue.BOOL; return Unsafe.As<bool, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.BOOL = Unsafe.As<T, bool>(ref rawValue); };
            }
            else if (t == typeof(byte))
            {
                GetValue = (attrValue, serializer) => { var v = Byte.Parse(attrValue.N); return Unsafe.As<byte, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(sbyte))
            {
                GetValue = (attrValue, serializer) => { var v = SByte.Parse(attrValue.N); return Unsafe.As<sbyte, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(short))
            {
                GetValue = (attrValue, serializer) => { var v = Int16.Parse(attrValue.N); return Unsafe.As<short, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(ushort))
            {
                GetValue = (attrValue, serializer) => { var v = UInt16.Parse(attrValue.N); return Unsafe.As<ushort, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(int))
            {
                GetValue = (attrValue, serializer) => { var v = Int32.Parse(attrValue.N); return Unsafe.As<int, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(uint))
            {
                GetValue = (attrValue, serializer) => { var v = UInt32.Parse(attrValue.N); return Unsafe.As<uint, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(long))
            {
                GetValue = (attrValue, serializer) => { var v = Int64.Parse(attrValue.N); return Unsafe.As<long, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(ulong))
            {
                GetValue = (attrValue, serializer) => { var v = UInt64.Parse(attrValue.N); return Unsafe.As<ulong, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(float))
            {
                GetValue = (attrValue, serializer) => { var v = Single.Parse(attrValue.N); return Unsafe.As<float, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(double))
            {
                GetValue = (attrValue, serializer) => { var v = Double.Parse(attrValue.N); return Unsafe.As<double, T>(ref v); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.N = rawValue.ToString(); };
            }
            else if (t == typeof(byte[]))
            {
                GetValue = (attrValue, serializer) => { return (T)(object)attrValue.B.ToArray(); };
                SetValue = (attrValue, rawValue, serializer) => { attrValue.B = new MemoryStream((byte[])(object)rawValue); };
            }
            else
            {
                GetValue = (attrValue, serializer) =>
                {
                    if (serializer.ValueSerialization == KumoDictionaryValueSerialization.Binary)
                    {
                        return serializer.Deserialize<T>(attrValue.B);
                    }
                    else
                    {
                        return serializer.Deserialize<T>(Encoding.UTF8.GetBytes(attrValue.S));
                    }
                };
                SetValue = (attrValue, rawValue, serializer) =>
                {
                    if (serializer.ValueSerialization == KumoDictionaryValueSerialization.Binary)
                    {
                        attrValue.B = new MemoryStream(); serializer.Serialize(attrValue.B, rawValue);
                    }
                    else
                    {
                        attrValue.S = new UTF8Encoding(false).GetString(serializer.Serialize(rawValue));
                    }
                };
            }
        }

        public static AttributeValue CreateValue(T value, IKumoDictionaryValueSerializer serializer)
        {
            var attrValue = new AttributeValue();
            SetValue(attrValue, value, serializer);

            return attrValue;
        }
    }

}
