using KumoDictionary.Provider;
using KumoDictionary.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KumoDictionary
{
    public class KumoDictionary<TValue> : KumoDictionary<string, TValue>
    {
        public KumoDictionary(string dictionaryName)
            : this(dictionaryName, KumoDictionaryBackendProvider.Default)
        { }

        public KumoDictionary(string dictionaryName, IKumoDictionaryBackendProvider provider)
            : base(dictionaryName, provider)
        { }
    }

    public partial class KumoDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private IKumoDictionaryBackend _backend;
        private KeyCollection _keyCollection;
        private ValueCollection _valueCollection;

        public string DictionaryName { get; }

        public TValue this[TKey key]
        {
            get
            {
                var result = TryGetValueAsync(key).GetAwaiter().GetResult();
                if (!result.Succeeded)
                {
                    throw new KeyNotFoundException();
                }

                return result.Value;
            }
            set
            {
                AddOrUpdateAsync(key, value).GetAwaiter().GetResult();
            }
        }

        public ICollection<TKey> Keys => _keyCollection ?? (_keyCollection = new KeyCollection(this));

        public ICollection<TValue> Values => _valueCollection ?? (_valueCollection = new ValueCollection(this));

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => false;

        public KumoDictionary(string dictionaryName)
            : this(dictionaryName, KumoDictionaryBackendProvider.Default)
        { }

        public KumoDictionary(string dictionaryName, IKumoDictionaryBackendProvider provider)
        {
            DictionaryName = dictionaryName;
            _backend = provider.Create(dictionaryName);
        }

        public void Add(TKey key, TValue value)
        {
            AddAsync(key, value).GetAwaiter().GetResult();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            AddAsync(item.Key, item.Value).GetAwaiter().GetResult();
        }

        public void Clear()
        {
            ClearAsync().GetAwaiter().GetResult();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var result) && result.Equals(item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return ContainsKeyAsync(key).GetAwaiter().GetResult();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _backend.GetEnumerator<TKey, TValue>();
        }

        public bool Remove(TKey key)
        {
            return RemoveAsync(key).GetAwaiter().GetResult();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return RemoveAsync(item.Key).GetAwaiter().GetResult();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var result = TryGetValueAsync(key).GetAwaiter().GetResult();
            value = result.Value;
            return result.Succeeded;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Task ClearAsync(CancellationToken cancellationToken = default)
        {
            return _backend.ClearAsync(cancellationToken);
        }

        public Task<bool> RemoveAsync(TKey key)
        {
            return _backend.RemoveAsync<TKey>(key);
        }

        public Task<bool> ContainsKeyAsync(TKey key)
        {
            return _backend.ContainsKeyAsync(key);
        }

        public Task AddOrUpdateAsync(TKey key, TValue value)
        {
            return _backend.AddOrUpdateAsync(key, value);
        }

        public Task AddAsync(TKey key, TValue value)
        {
            return _backend.AddAsync(key, value);
        }

        public Task<TryGetAsyncResult<TValue>> TryGetValueAsync(TKey key)
        {
            return _backend.TryGetValueAsync<TKey, TValue>(key);
        }

        private class ValueCollection : ICollection<TValue>
        {
            private KumoDictionary<TKey, TValue> _innerDictionary;

            public ValueCollection(KumoDictionary<TKey, TValue> dictionary)
            {
                _innerDictionary = dictionary;
            }

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => true;

            public void Add(TValue item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(TValue item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(TValue[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                foreach (var entry in _innerDictionary)
                {
                    yield return entry.Value;
                }
            }

            public bool Remove(TValue item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class KeyCollection : ICollection<TKey>
        {
            private KumoDictionary<TKey, TValue> _innerDictionary;

            public KeyCollection(KumoDictionary<TKey, TValue> dictionary)
            {
                _innerDictionary = dictionary;
            }

            public int Count => throw new NotImplementedException();

            public bool IsReadOnly => true;

            public void Add(TKey item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(TKey item)
            {
                return _innerDictionary.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                foreach (var entry in _innerDictionary)
                {
                    yield return entry.Key;
                }
            }

            public bool Remove(TKey item)
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }

}
