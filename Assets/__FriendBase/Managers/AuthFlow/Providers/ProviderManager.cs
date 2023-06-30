using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ProviderManager : IProvider
{
    private List<AbstractProvider> listProviders;

    public ProviderManager()
    {
        listProviders = new List<AbstractProvider>();
        listProviders.Add(new MailProvider());
        listProviders.Add(new FacebookProvider());
        listProviders.Add(new AppleProvider());
        listProviders.Add(new GoogleProvider());
        listProviders.Add(new GuestProvider());
    }

    public AbstractProvider AddProvider(AbstractProvider providerToAdd)
    {
        if (providerToAdd == null)
        {
            return null;
        }

        AbstractProvider currenProvider = GetProvider(providerToAdd.TypeProvider);
        if (currenProvider!=null)
        {
            //The provider is already added => We can not have more than 1
            return null;
        }
        listProviders.Add(providerToAdd);
        return providerToAdd;
    }

    public AbstractProvider GetProvider(ProviderType providerType)
    {
        foreach (AbstractProvider provider in listProviders)
        {
            if (provider.TypeProvider == providerType)
            {
                return provider;
            }
        }
        return null;
    }
}
