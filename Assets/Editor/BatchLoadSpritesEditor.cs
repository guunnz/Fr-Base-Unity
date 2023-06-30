
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.U2D;
using UnityEditor.U2D;
using Gradients;

[CustomEditor(typeof(BatchLoadSprites))]
public class BatchLoadSpritesEditor : UnityEditor.Editor
{
#if UNITY_EDITOR
    public string psbFolderPath = "Assets/__FriendBase/Art/Avatars/AvatarParts"; // Change this to your desired folder path
    private List<string> generatedAtlasPaths = new List<string>();
    private delegate void UpdateDelegate();
    private UpdateDelegate currentUpdate;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        BatchLoadSprites batchLoadSprites = (BatchLoadSprites)target;

        if (GUILayout.Button("Press me!"))
        {
            currentUpdate = () =>
            {
                MakePSBsReadable();
                currentUpdate = () =>
                {
                    AddLoadSpriteInEditorToChildrenAndPopulateList(batchLoadSprites);
                    currentUpdate = () =>
                    {
                        LoadSprites(batchLoadSprites);
                        currentUpdate = null;
                    };
                };
            };
        }

        //if (GUILayout.Button("Load Colors"))
        //{
        //    AddLoadSpriteInEditorToChildrenAndPopulateList(batchLoadSprites);
        //    LoadMaterialsFromJsonForAllGradientItems();
        //}

        if (GUILayout.Button("Only press if magic button fails"))
        {
            MakePSBsUnreadableAndDeleteAtlases();
        }
    }

    private void OnDestroy()
    {
        MakePSBsUnreadableAndDeleteAtlases();
    }
#if UNITY_EDITOR
    // Create a new method to call the LoadMaterialFromJson method on all GradientItemController components in the scene
    private void LoadMaterialsFromJsonForAllGradientItems()
    {
        GradientItemController[] gradientItemControllers = FindObjectsOfType<GradientItemController>();

        foreach (GradientItemController gradientItemController in gradientItemControllers)
        {
            if (gradientItemController != null)
            {
                gradientItemController.LoadMaterialFromJson();
            }
        }
    }
#endif
    private void OnEnable()
    {
        EditorApplication.update += OnUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnUpdate;
    }

    private void OnUpdate()
    {
        if (currentUpdate != null)
        {
            currentUpdate.Invoke();
        }
    }

    private void AddLoadSpriteInEditorToChildrenAndPopulateList(BatchLoadSprites batchLoadSprites)
    {
        batchLoadSprites.objectsWithLoadSpriteInEditor.Clear();
        foreach (Transform child in batchLoadSprites.transform)
        {
            LoadSpriteInEditor loadSpriteInEditor = child.GetComponent<LoadSpriteInEditor>();
            if (loadSpriteInEditor == null)
            {
                loadSpriteInEditor = child.gameObject.AddComponent<LoadSpriteInEditor>();
            }
            batchLoadSprites.objectsWithLoadSpriteInEditor.Add(child.gameObject);
        }
    }

    private void LoadSprites(BatchLoadSprites batchLoadSprites)
    {
        foreach (GameObject go in batchLoadSprites.objectsWithLoadSpriteInEditor)
        {
            if (go != null)
            {
                LoadSpriteInEditor loadSpriteInEditor = go.GetComponent<LoadSpriteInEditor>();
                if (loadSpriteInEditor != null)
                {
                    loadSpriteInEditor.LoadSprite();
                }
            }
        }
        MakePSBsUnreadableAndDeleteAtlases();
    }

    // ...

    private void MakePSBsReadable()
    {
        // Find all the PSBs in the specified folder
        string[] psbGuids = AssetDatabase.FindAssets("t:Texture2D", new string[] { psbFolderPath });

        foreach (string psbGuid in psbGuids)
        {
            // Load the texture from the GUID
            string psbPath = AssetDatabase.GUIDToAssetPath(psbGuid);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(psbPath);

            // Create a sprite atlas for the PSB
            string atlasPath = Path.GetDirectoryName(psbPath) + "/" + Path.GetFileNameWithoutExtension(psbPath) + ".spriteatlas";
            SpriteAtlas atlas = new SpriteAtlas();
            AssetDatabase.CreateAsset(atlas, atlasPath);

            //// Set sprite atlas texture settings
            //TextureImporter atlasImporter = (TextureImporter)AssetImporter.GetAtPath(atlasPath);
            //if (atlasImporter != null)
            //{
            //    atlasImporter.textureType = TextureImporterType.Sprite;
            //    atlasImporter.SaveAndReimport();
            //}

            // Add all the sprites in the PSB to the sprite atlas
            Object[] spriteObjects = AssetDatabase.LoadAllAssetsAtPath(psbPath);
            foreach (Object obj in spriteObjects)
            {
                if (obj is Sprite)
                {
                    Sprite sprite = obj as Sprite;
                    SpriteAtlasExtensions.Add(atlas, new[] { sprite });
                }
                else if (obj is SpriteRenderer)
                {
                    SpriteRenderer spriteRenderer = obj as SpriteRenderer;
                    if (spriteRenderer.sprite.name.Contains("Mask"))
                        return;
                    
                    SpriteAtlasExtensions.Add(atlas, new[] { spriteRenderer.sprite });
                }
            }

            // Print the sprite names in the sprite atlas
            Sprite[] sprites = new Sprite[atlas.spriteCount];
            atlas.GetSprites(sprites);
            foreach (Sprite sprite in sprites)
            {
                Debug.Log(sprite.name + " in " + atlasPath);
            }
        }
    }

    public List<Sprite> ExtractSpritesFromPSB(Texture2D psbTexture)
    {
        List<Sprite> sprites = new List<Sprite>();

        // Get all the assets that the PSB depends on
        string[] dependencies = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(psbTexture), true);

        // Filter the assets to find the sprites
        foreach (string dependency in dependencies)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(dependency);
            if (asset is Sprite sprite)
            {
                sprites.Add(sprite);
            }
        }

        return sprites;
    }



    private void MakePSBsUnreadableAndDeleteAtlases()
    {
        string[] allPSBFiles = AssetDatabase.FindAssets("t:Texture2D", new string[] { psbFolderPath });

        foreach (string psbGuid in allPSBFiles)
        {
            string psbPath = AssetDatabase.GUIDToAssetPath(psbGuid);

            // Delete generated sprite atlas
            string spriteAtlasPath = Path.GetDirectoryName(psbPath) + "/" + Path.GetFileNameWithoutExtension(psbPath) + ".spriteatlas";
            if (AssetDatabase.DeleteAsset(spriteAtlasPath))
            {
                Debug.Log("Deleted sprite atlas at path: " + spriteAtlasPath);
            }
            else
            {
                Debug.LogWarning("Failed to delete sprite atlas at path: " + spriteAtlasPath);
            }
        }
    }


    private int GetPlatformMaxTextureSize(TextureImporter textureImporter)
    {
        int maxTextureSize = 1024;
        TextureImporterPlatformSettings platformSettings = textureImporter.GetPlatformTextureSettings(EditorUserBuildSettings.activeBuildTarget.ToString());
        if (platformSettings != null)
        {
            maxTextureSize = platformSettings.maxTextureSize;
        }
        return maxTextureSize;
    }
#endif
}

