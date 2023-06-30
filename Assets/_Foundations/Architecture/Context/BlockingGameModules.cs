using System.Collections.Generic;
using AuthFlow;
using AuthFlow.FacebookLogin;
using ConnectingToServer;
using DeepLink;
using Socket;

namespace Architecture.Context
{
    /// <summary>
    /// Here we are going to initialize the dependencies that require an asynchronous
    /// initialization but that blocks the initialization pipeline until it completes.
    /// Dependencies must implement <see cref="IBlockerModule"/>
    /// </summary>
    public class BlockingGameModules : List<IBlockerModule>
    {
        public void Declare()
        {
            //setup dependencies in the correct order
            Dependency<ConnectingToServerModule>();
            Dependency<AuthFlowModule>();
            Dependency<FacebookLoginModule>();
            Dependency<DeepLinkModule>();
        }

        private void Dependency<T>() where T : class, IBlockerModule, new()
        {
            Add(new T());
        }
    }
}