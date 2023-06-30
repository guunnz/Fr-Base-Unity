using System.Collections.Generic;
using System.Linq;

namespace Tools.Collections
{
    public class ArrayBlackboard : IBlackboard
    {
        private readonly object[] objects;

        public ArrayBlackboard(IEnumerable<object> objects)
        {
            this.objects = objects.ToArray();
        }

        public T Get<T>()
        {
            return objects.OfType<T>().FirstOrDefault();
        }

        public bool TryGet<T>(out T t)
        {
            var ot = objects.OfType<T>();
            if (ot.Any())
            {
                t = ot.First();
                return true;
            }
            t = default;
            return false;
        }

        public T[] GetAll<T>()
        {
            return objects.OfType<T>().ToArray();
        }
    }

    public interface IBlackboard
    {
        T Get<T>();
        T[] GetAll<T>();
        bool TryGet<T>(out T t);
    }

    public static class BlackboardExtensions
    {
        public static IBlackboard AsBlackboard(this IEnumerable<object> objects)
        {
            return new ArrayBlackboard(objects);
        }
    }
}