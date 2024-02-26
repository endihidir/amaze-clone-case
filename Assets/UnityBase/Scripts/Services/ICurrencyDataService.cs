namespace UnityBase.Service
{
    public interface ICurrencyDataService
    {
        public float SavedCoinAmount { get; }

        public void IncreaseCoin(float value);
        public void DecreaseCoin(float value);
    }
}