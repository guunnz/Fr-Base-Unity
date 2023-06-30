namespace AuthFlow
{
    public interface IAuthStateManager
    {
        string Email { get; set; }
        string Password { get; set; }
    }
}