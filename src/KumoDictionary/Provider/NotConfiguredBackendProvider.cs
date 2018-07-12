using System;
using System.Collections.Generic;
using System.Text;

namespace KumoDictionary.Provider
{
    internal class NotConfiguredBackendProvider : IKumoDictionaryBackendProvider
    {
        public IKumoDictionaryBackend Create(string dictionaryName)
        {
            throw new InvalidOperationException("Default backend provider is not configured.");
        }
    }
}
