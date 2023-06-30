namespace MemoryStorage.Core
{
    public interface IMemoryStorage
    {
        string Get(string key);
        void Set(string key, string value);
    }
}