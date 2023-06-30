namespace AuthFlow.AppleLogin.Infrastructure
{
    public interface IAppleNonceGenerator
    {
        string GenerateAppleNonce(int length = 32);
    }
}