using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


// Declare type of Custom Editor
namespace Adinmo
{
    [CustomEditor(typeof(AdinmoTexture))]
    public class AdinmoTextureEditor : UnityEditor.Editor
    {

        private SerializedProperty testImage;
        // OnInspector GUI
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            AdinmoTexture adinmoTexture = (AdinmoTexture)target;
            List<string> labels=new List<string>();
            int displaymask = 0;

            for (int i = 0; i < 32; i++)
            {
                if (LayerMask.LayerToName(i) != "")
                {
                    int tempMask = 1 << labels.Count;
                    labels.Add(LayerMask.LayerToName(i));
                        
                    if ((1 << i & adinmoTexture.layerMask) != 0)
                        displaymask |= tempMask;
                }
            }
            
            if (labels.Count > 0)
            {
                int newdisplaymask = EditorGUILayout.MaskField("Occlusion test ignores Layers", displaymask, labels.ToArray());
                if (newdisplaymask != -1 && newdisplaymask != displaymask)
                {
                    int newMask = 0;
                    for (int i = 0; i < labels.Count; i++)
                    {
                        if ((newdisplaymask & 1 << i) != 0)
                        {
                            newMask |= 1 << LayerMask.NameToLayer(labels[i]);
                        }
                    }
                    adinmoTexture.layerMask = newMask;
                }
                else if(newdisplaymask==-1)
                {
                    Debug.LogError("Can't filter out all layers");
                }
                
            }

            if (Application.isPlaying)
            {
                if (adinmoTexture.GetObjectType() == AdinmoReplace.ObjectType.Image || !AdinmoManager.s_manager.m_debugImageRender)
                {
                    EditorGUILayout.LabelField("Latest Sample", ("" + (adinmoTexture.LatestSampleValue * 100).ToString("F1") + "%"));
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.PrefixLabel("Sample Texture");
                    GUILayout.Button(adinmoTexture.SampleTexture, GUILayout.Width(48), GUILayout.Height(48));
                    EditorGUILayout.BeginVertical(GUILayout.Width(50));
                    GUILayout.Label("Latest Sample");
                    GUILayout.Label("" + (adinmoTexture.LatestSampleValue * 100).ToString("F1") + "%");

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
}
