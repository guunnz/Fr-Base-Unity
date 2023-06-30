using Settings.Services;
using UnityEditor;
using UnityEngine;

namespace Settings.View
{
    public class ScriptableSettingsEditor : EditorWindow
    {
        private readonly ISettingsRepository repository = new LocalSettingsRepository();

        private void OnGUI()
        {
            var settings = repository.LoadSettings();
            GUILayout.BeginHorizontal();
            GUILayout.Label("AudioSettings.MusicVol : ");
            GUILayout.Label(settings.audioSettings.musicVol + "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("AudioSettings.SFXVol : ");
            GUILayout.Label(settings.audioSettings.sfxVol + "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("AudioSettings.MasterVol : ");
            GUILayout.Label(settings.audioSettings.masterVol + "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("GameSettings.Difficulty : ");
            GUILayout.Label(settings.gameSettings.difficulty + "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("VideoSettings.Quality : ");
            GUILayout.Label(settings.videoSettings.quality + "");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("VideoSettings.FullScreen : ");
            GUILayout.Label(settings.videoSettings.fullScreen + "");
            GUILayout.EndHorizontal();
        }

        [MenuItem(Const.GameNameMenu + "/Settings Repo Status")]
        private static void Init()
        {
            var window = (ScriptableSettingsEditor) GetWindow(typeof(ScriptableSettingsEditor));
            window.Show();
        }
    }
}