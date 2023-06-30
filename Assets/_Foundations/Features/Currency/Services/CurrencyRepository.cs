namespace Currency.Services
{
    public interface ICurrencyRepository
    {
        int this[string key] { get; set; }
    }
}