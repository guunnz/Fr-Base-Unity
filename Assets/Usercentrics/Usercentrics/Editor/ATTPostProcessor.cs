using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;

public class ATTPostProcessor : MonoBehaviour
{
    [PostProcessBuild]
    public static void UpdatePlistFile(BuildTarget buildTarget, string buildPath)
    {
#if UNITY_IOS
        //if (buildTarget == BuildTarget.iOS && AppTrackingTransparency.m_ATTEnabled)
        //{
        //    // Get plist
        //    var plistPath = Path.Combine(buildPath, "Info.plist");
        //    var plist = new PlistDocument();
        //    plist.ReadFromFile(plistPath);

        //    // Get root
        //    PlistElementDict rootDict = plist.root;
        //    rootDict.SetString("NSUserTrackingUsageDescription", AppTrackingTransparency.m_ATTMessage);

        //    // Write plist
        //    File.WriteAllText(plistPath, plist.WriteToString());
        //}
#endif
    }
}