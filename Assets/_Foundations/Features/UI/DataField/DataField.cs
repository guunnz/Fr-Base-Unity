using UnityEngine;

namespace UI.DataField
{
    public abstract class DataField<T> : MonoBehaviour, IDataField<T>
    {
        public abstract T Value { get; set; }
        public abstract void ShowError(string errorMessage);
    }
}