using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Data.Catalog;
using System;

namespace Gradients
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GradientItemController : MonoBehaviour
    {
        static readonly int GradientProperty = Shader.PropertyToID("_Gradient");

        [SerializeField] GradientList gradientList;

        private SpriteRenderer spriteRenderer;
        private int gradientID;

#if UNITY_EDITOR

        [SerializeField] string materialJson;

        [System.Serializable]
        public class MaterialPropertyData
        {
            public string name;
            public ShaderUtil.ShaderPropertyType type;
            public string texturePath;
            public Vector4 vectorValue;
            public float floatValue;
        }

        private bool initialized;

        [System.Serializable]
        public class MaterialData
        {
            public string shaderName;
            public List<MaterialPropertyData> properties;
        }
#endif

        void Awake()
        {
            try
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                if (transform.parent.GetComponent<AvatarCustomizationController>() != null)
                    spriteRenderer.material = new Material(Shader.Find("Friendbase/GradientSprite"));
            }
            catch (Exception ex)
            {

            }
#if UNITY_EDITOR

            if (spriteRenderer.sprite.name.Contains("Clone"))
                return;

            if (!string.IsNullOrEmpty(materialJson))
            {
                LoadMaterialFromJson();
            }
#endif
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!initialized)
            {
                LoadMaterialFromJson();

                initialized = true;
            }
        }
#endif

        public void SetGradientColor(ColorCatalogItem colorCatalogItem)
        {
            gradientID = colorCatalogItem.IdItem;
            Texture2D texture2D = gradientList.GetTextureByName(colorCatalogItem.NamePrefab);
            if (texture2D != null)
            {
                spriteRenderer.material.SetTexture(GradientProperty, texture2D);
#if UNITY_EDITOR
                // Save the material data as JSON
                MaterialData materialData = new MaterialData
                {
                    shaderName = spriteRenderer.material.shader.name,
                    properties = new List<MaterialPropertyData>()
                };

                int propertyCount = ShaderUtil.GetPropertyCount(spriteRenderer.material.shader);
                for (int i = 0; i < propertyCount; i++)
                {
                    MaterialPropertyData propertyData = new MaterialPropertyData
                    {
                        name = ShaderUtil.GetPropertyName(spriteRenderer.material.shader, i),
                        type = ShaderUtil.GetPropertyType(spriteRenderer.material.shader, i)
                    };

                    switch (propertyData.type)
                    {
                        case ShaderUtil.ShaderPropertyType.Color:
                        case ShaderUtil.ShaderPropertyType.Vector:
                            propertyData.vectorValue = spriteRenderer.material.GetVector(propertyData.name);
                            break;
                        case ShaderUtil.ShaderPropertyType.Float:
                        case ShaderUtil.ShaderPropertyType.Range:
                            propertyData.floatValue = spriteRenderer.material.GetFloat(propertyData.name);
                            break;
                        case ShaderUtil.ShaderPropertyType.TexEnv:
                            Texture texture = spriteRenderer.material.GetTexture(propertyData.name);
                            if (texture != null)
                            {
                                propertyData.texturePath = AssetDatabase.GetAssetPath(texture);
                            }
                            break;
                    }

                    materialData.properties.Add(propertyData);
                }

                materialJson = JsonUtility.ToJson(materialData);
#endif
            }
        }
#if UNITY_EDITOR
        public void LoadMaterialFromJson()
        {
            if (!string.IsNullOrEmpty(materialJson))
            {
                MaterialData materialData = JsonUtility.FromJson<MaterialData>(materialJson);

                spriteRenderer = GetComponent<SpriteRenderer>();
                // Create a new material with the saved shader name
                spriteRenderer.material = new Material(Shader.Find(materialData.shaderName));

                foreach (MaterialPropertyData propertyData in materialData.properties)
                {
                    switch (propertyData.type)
                    {
                        case ShaderUtil.ShaderPropertyType.Color:
                        case ShaderUtil.ShaderPropertyType.Vector:
                            spriteRenderer.sharedMaterial.SetVector(propertyData.name, propertyData.vectorValue);
                            break;
                        case ShaderUtil.ShaderPropertyType.Float:
                        case ShaderUtil.ShaderPropertyType.Range:
                            spriteRenderer.sharedMaterial.SetFloat(propertyData.name, propertyData.floatValue);
                            break;
                        case ShaderUtil.ShaderPropertyType.TexEnv:
                            if (!string.IsNullOrEmpty(propertyData.texturePath))
                            {
                                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(propertyData.texturePath);
                                if (texture != null)
                                {
                                    spriteRenderer.sharedMaterial.SetTexture(propertyData.name, texture);
                                }
                            }
                            break;
                    }
                }
            }
        }
#endif
    }
}
