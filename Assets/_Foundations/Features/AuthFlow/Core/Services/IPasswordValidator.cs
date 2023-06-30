namespace AuthFlow
{
    public interface IPasswordValidator
    {
        (bool, string) Validate(string pass);
    }
}
