
namespace Multiuser
{
    public abstract class AbstractIncomingTrama
    {
        public abstract string TramaID { get; }
        public abstract void Serialize();

        public int State { get; protected set; }
    }
}
