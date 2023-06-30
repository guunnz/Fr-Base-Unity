using System;
using System.Collections.Generic;
using System.Linq;
using Architecture.ViewManager;

namespace Shared.Utils
{
    public delegate INode Finder(Type type);

    public class TypedPool
    {
        private readonly Finder finder;

        private readonly Dictionary<Type, List<INode>> instances = new Dictionary<Type, List<INode>>();

        public TypedPool(Finder finder)
        {
            this.finder = finder;
        }

        private INode GetByType(Type type)
        {
            if (!instances.TryGetValue(type, out var list))
            {
                list = new List<INode>();
                instances[type] = list;
            }

            var instance = list.Where(Available).FirstOrDefault();

            if (instance == null)
            {
                instance = finder(type);
                list.Add(instance);
            }

            return instance;
        }

        private static bool Available(INode node) => !node.Enabled;


        public ViewNode Get(Type type)
        {
            return (ViewNode) GetByType(type);
        }

        public T Get<T>() where T : ViewNode
        {
            return GetByType(typeof(T)) as T;
        }

        public void Clear()
        {
            instances
                .SelectMany(pair => pair.Value)
                .ToList()
                .ForEach(p => p.Dispose());
            instances.Clear();
        }
    }
}