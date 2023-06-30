using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AddressablesSystem
{
    public class LoaderDataToCommit
    {
        public enum TypeCommit { SUSCRIBE = 0, UNSUSCRIBE = 1 };

        public string id;
        public IReceiveLoadedItem suscriber;
        public TypeCommit typeCommit;

        public LoaderDataToCommit(string id, IReceiveLoadedItem suscriber, TypeCommit typeCommit)
        {
            this.id = id;
            this.suscriber = suscriber;
            this.typeCommit = typeCommit;
        }
    }
}