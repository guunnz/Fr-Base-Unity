using System;
using System.Collections.Generic;
using System.Linq;
using Functional.Maybe;
using UnityEngine;

namespace Scriptables
{
    public class ScriptableList<T> : ScriptableObject where T : class, IFindableItem
    {
        public IReadOnlyList<T> List => list;
        [SerializeField] protected List<T> list;

        public Maybe<T> GetItem(string id)
        {
            return list.FirstMaybe(CompareId(id));
        }

        public bool HasItem(string id)
        {
            return list.Any(CompareId(id));
        }

        static Func<T, bool> CompareId(string id)
        {
            return item => item.Id.ToLower() == id.ToLower();
        }

        public IReadOnlyList<T> GetList() => list;
    }

    public interface IFindableItem
    {
        string Id { get; }
    }
}