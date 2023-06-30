#if UNITY_IOS
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

// Adds Firebase PList to iOS
public class PostProcessIOS {
   [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget != BuildTarget.iOS)
        {
            return;
        }

        var projPath = ConcatPath(path, "Unity-iPhone.xcodeproj/project.pbxproj");
        var proj = new PBXProject();
        proj.ReadFromFile(projPath);
        // This is the Xcode target in the generated project 
        var target = proj.TargetGuidByName("Unity-iPhone");
        // Copy plist from the project folder to the build folder 
        var destination = ConcatPath(path, "GoogleService-Info.plist");
        FileUtil.CopyFileOrDirectory("Assets/GoogleService-Info.plist", destination);
        proj.AddFileToBuild(target, proj.AddFile("GoogleService-Info.plist", "GoogleService-Info.plist"));
        // Write PBXProject object back to the file 
        proj.WriteToFile(projPath);
    }

    static string ConcatPath(string path, string suffix)
    {
        const string separator = "/";
        return path.EndsWith(separator) ? $"{path}{suffix}" : $"{path}{separator}{suffix}";
    }
}
#endif
