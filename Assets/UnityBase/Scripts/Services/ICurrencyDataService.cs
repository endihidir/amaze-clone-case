namespace UnityBase.Service
{
    public interface ICurrencyDataService
    {
        public int SavedCoinAmount { get; }
        public void IncreaseCoin(int value);
        public void DecreaseCoin(int value);
    }
}