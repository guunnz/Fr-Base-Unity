#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.U2D;
using System.Reflection;
using UnityEditor.U2D;
using Gradients;

public class LoadSpriteInEditor : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    public void LoadSprite()
    {
        try
        {
            List<Sprite> spriteList = GetAllSpritesFromAtlases();
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null || spriteRenderer.sprite == null)
            {
                Debug.LogError("No sprite is currently assigned to the SpriteRenderer.");
                return;
            }

            if (transform.parent.GetComponent<AvatarCustomizationController>() != null)
            {
                //Destroy(transform.parent.GetComponent<AvatarCustomizationController>());
            }

            string currentSpriteName = spriteRenderer.sprite.name.Replace("(Clone)", "").Trim();

            if (spriteList.Any(x => x.name == currentSpriteName))
            {
                spriteRenderer.sprite = GetAllSpritesFromAtlases().FirstOrDefault(x => x.name == currentSpriteName);
            }
            else
            {
                Debug.LogError($"Sprite with name '{currentSpriteName}' could not be found in any texture atlas.");
            }
            Destroy(GetComponent<LoadSpriteInEditor>());
        }
        catch
        {

        }
    }

    public static List<Sprite> GetAllSpritesFromAtlases()
    {
        List<Sprite> sprites = new List<Sprite>();

        // Find all sprite atlases in the project
        string[] atlasGuids = AssetDatabase.FindAssets("t:SpriteAtlas");
        foreach (string atlasGuid in atlasGuids)
        {
            string atlasPath = AssetDatabase.GUIDToAssetPath(atlasGuid);
            SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
            if (atlas != null)
            {
                // Get the packable assets
                Object[] packableObjects = atlas.GetPackables();

                // Iterate through the packable assets and add Sprites to the sprites list
                foreach (Object packableObject in packableObjects)
                {
                    if (packableObject is Sprite sprite)
                    {
                        sprites.Add(sprite);
                    }
                }
            }
        }

        return sprites;
    }

}
#endif
