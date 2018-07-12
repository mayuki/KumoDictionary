using KumoDictionary.Provider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KumoDictionary
{
    public class KumoMultiTypeDictionary<TKey>
    {
        private IKumoDictionaryBackend _backend;
        public string DictionaryName { get; }

        public KumoMultiTypeDictionary(string dictionaryName)
            : this(dictionaryName, KumoDictionaryBackendProvider.Default)
        { }

        public KumoMultiTypeDictionary(string dictionaryName, IKumoDictionaryBackendProvider provider)
        {
            DictionaryName = dictionaryName;
            _backend = provider.Create(dictionaryName);
        }

        public void Add<TValue>(TKey key, TValue value)
        {
            AddAsync(key, value).GetAwaiter().GetResult();
        }

        public void Clear()
        {
            ClearAsync().GetAwaiter().GetResult();
        }

        public bool ContainsKey(TKey key)
        {
            return ContainsKeyAsync(key).GetAwaiter().GetResult();
        }

        public bool Remove(TKey key)
        {
            return RemoveAsync(key).GetAwaiter().GetResult();
        }

        public bool TryGetValue<TValue>(TKey key, out TValue value)
        {
            var result = TryGetValueAsync<TValue>(key).GetAwaiter().GetResult();
            value = result.Value;
            return result.Succeeded;
        }

        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            return _backend.ClearAsync(cancellationToken);
        }

        public Task<bool> RemoveAsync(TKey key)
        {
            return _backend.RemoveAsync(key);
        }

        public Task<bool> ContainsKeyAsync(TKey key)
        {
            return _backend.ContainsKeyAsync(key);
        }

        public Task AddOrUpdateAsync<TValue>(TKey key, TValue value)
        {
            return _backend.AddOrUpdateAsync(key, value);
        }

        public Task AddAsync<TValue>(TKey key, TValue value)
        {
            return _backend.AddAsync(key, value);
        }

        public Task<TryGetAsyncResult<TValue>> TryGetValueAsync<TValue>(TKey key)
        {
            return _backend.TryGetValueAsync<TKey, TValue>(key);
        }
    }
}
