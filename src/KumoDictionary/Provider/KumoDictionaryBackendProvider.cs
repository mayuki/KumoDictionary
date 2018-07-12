namespace KumoDictionary.Provider
{
    public class KumoDictionaryBackendProvider
    {
        public static IKumoDictionaryBackendProvider Default { get; set; } = new NotConfiguredBackendProvider();
    }
}
