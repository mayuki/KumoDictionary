namespace KumoDictionary
{
    public struct TryGetAsyncResult<TValue>
    {
        public bool Succeeded { get; }
        public TValue Value { get; }

        public static readonly TryGetAsyncResult<TValue> NotFound = new TryGetAsyncResult<TValue>(false, default);

        private TryGetAsyncResult(bool succeeded, TValue value)
        {
            Succeeded = succeeded;
            Value = value;
        }

        public TryGetAsyncResult(TValue value) : this(true, value)
        {
        }
    }
}
