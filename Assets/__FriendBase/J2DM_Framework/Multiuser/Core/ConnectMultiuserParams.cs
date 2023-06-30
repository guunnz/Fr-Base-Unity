

namespace Multiuser
{
    public class ConnectMultiuserParams
    {
        public string firebaseUrl;
        public string ip;
        public int port;

        public ConnectMultiuserParams()
        {
        }

        public ConnectMultiuserParams(string firebaseUrl, string ip, int port)
        {
            this.firebaseUrl = firebaseUrl;
            this.ip = ip;
            this.port = port;
        }

        public ConnectMultiuserParams(string firebaseUrl)
        {
            this.firebaseUrl = firebaseUrl;
        }

        public ConnectMultiuserParams(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
    }
}

