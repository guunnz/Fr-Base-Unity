namespace LocalStorage.Core
{
    public interface ILocalStorage
    {
        void SetString(string key, string value);
        void SetInt(string key, int value);
        string GetString(string key);
        int GetInt(string key);
    }
}