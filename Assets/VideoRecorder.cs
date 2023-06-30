#if UNITY_EDITOR
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;

public class VideoRecorder : MonoBehaviour
{
    RecorderController recorderController;
    public bool enableRecorder = true;
    void Start()
    {
        SetUpRecorder();
    }

    void Update()
    {
        if (!enableRecorder)
            return;
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartRecording();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            StopRecording();
        }
    }

    void SetUpRecorder()
    {
        string videosFolderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyVideos);
        string tiktokFolderPath = Path.Combine(videosFolderPath, "Friendbase");

        var movieRecorderSettings = new UnityEditor.Recorder.MovieRecorderSettings
        {
            OutputFormat = UnityEditor.Recorder.MovieRecorderSettings.VideoRecorderOutputFormat.MP4,
            OutputFile = Path.Combine(tiktokFolderPath, "Video" + Random.Range(0, 99999999)),
            ImageInputSettings = new GameViewInputSettings()
        };
        movieRecorderSettings.AudioInputSettings.PreserveAudio = true;

        var recorderControllerSettings = new RecorderControllerSettings();
        recorderControllerSettings.AddRecorderSettings(movieRecorderSettings);

        recorderController = new RecorderController(recorderControllerSettings);
    }

    public void StartRecording()
    {
        recorderController.PrepareRecording();
        recorderController.StartRecording();
    }

    public void StopRecording()
    {
        recorderController.StopRecording();
    }
}
#endif