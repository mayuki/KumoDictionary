using System;
using System.Collections.Generic;
using System.Text;

namespace KumoDictionary.Serialization
{
    public class KumoDictionaryValueSerializer
    {
        public static IKumoDictionaryValueSerializer Default { get; } = new MessagePackValueSerializer();
    }
}
