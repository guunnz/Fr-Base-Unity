namespace AuthFlow
{
    public interface IUserAuthRepo
    {
        void SaveUserAuthInfo(string userId, string displayName);
        (string userId, string displayName)? GetUserAuthInfo();
    }
}