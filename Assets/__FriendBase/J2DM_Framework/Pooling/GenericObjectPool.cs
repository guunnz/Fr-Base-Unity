using System.Collections.Generic;
using UnityEngine;

namespace CustomTools.ObjectPool
{
    public abstract class GenericObjectPool<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private T prefab;
        [SerializeField] private GameObject container;

        public static GenericObjectPool<T> Instance { get; private set; }
        private Queue<T> objects = new Queue<T>();

        private void Awake()
        {
            Instance = this;
        }

        public T Get()
        {
            if (objects.Count == 0)
            {
                AddObjects(1);
            }
            return objects.Dequeue();
        }

        public void ReturnToPool(T objectToReturn)
        {
            objectToReturn.gameObject.SetActive(false);
            objects.Enqueue(objectToReturn);
            if (container != null)
            {
                objectToReturn.transform.SetParent(container.transform);
            }
        }

        public void AddObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var newObject = Instantiate(prefab);
                newObject.gameObject.SetActive(false);
                objects.Enqueue(newObject);

                if (container != null)
                {
                    newObject.transform.SetParent(container.transform);
                }
            }
        }
    }
}

