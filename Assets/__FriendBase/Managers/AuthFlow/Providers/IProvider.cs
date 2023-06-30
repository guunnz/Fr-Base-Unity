using System.Threading.Tasks;

public interface IProvider
{
    AbstractProvider AddProvider(AbstractProvider provider);
    AbstractProvider GetProvider(ProviderType providerType);
}
