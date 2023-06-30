namespace DeepLink
{
    public interface IDeepLinkProcess
    {
        (string key, string value)[] GetInfo(string absoluteURL);
    }
}