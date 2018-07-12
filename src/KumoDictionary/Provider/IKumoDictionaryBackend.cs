using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace KumoDictionary.Provider
{
    public interface IKumoDictionaryBackend
    {
        IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator<TKey, TValue>();
        Task ClearAsync(CancellationToken cancellationToken);
        Task<bool> RemoveAsync<TKey>(TKey key);
        Task<bool> ContainsKeyAsync<TKey>(TKey key);
        Task AddOrUpdateAsync<TKey, TValue>(TKey key, TValue value);
        Task AddAsync<TKey, TValue>(TKey key, TValue value);
        Task<TryGetAsyncResult<TValue>> TryGetValueAsync<TKey, TValue>(TKey key);
    }
}
