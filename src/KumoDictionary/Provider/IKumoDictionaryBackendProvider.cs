namespace KumoDictionary.Provider
{
    public interface IKumoDictionaryBackendProvider
    {
        IKumoDictionaryBackend Create(string dictionaryName);
    }
}
