
namespace Multiuser
{
    public abstract class AbstractTrama
    {
        public abstract string TramaID { get; }
        public abstract void Serialize();
        public abstract void Send();
    }
}

