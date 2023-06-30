using TMPro;
using UnityEngine;

namespace UI
{
    public class InputTextField : MonoBehaviour
    {
        [SerializeField] StringWidget nameLabel;
        [SerializeField] TMP_InputField valueInput;

        [SerializeField] string fieldName;
        [SerializeField] string fieldValue;

#if UNITY_EDITOR
        void OnValidate()
        {
            LoadValuesOnComponents();
        }
#endif
        void LoadValuesOnComponents()
        {
            if (valueInput)
            {
                valueInput.text = fieldValue;
            }

            if (nameLabel)
            {
                nameLabel.Value = fieldName;
            }
        }

        void Start()
        {
            LoadValuesOnComponents();
        }
        
        
        
    }
}