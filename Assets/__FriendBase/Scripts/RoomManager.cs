using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Managers.InRoomGems;
using Audio.Music;
using Architecture.Injector.Core;

public class RoomManager : MonoBehaviour
{
    [SerializeField] List<CinemachineVirtualCamera> virtualCamerasList;
    [SerializeField] GameObject collidersContainer;

    [SerializeField] private InRoomGemsPoints gemPointsContainer;
    public InRoomGemsPoints GemPointsContainer => gemPointsContainer;

    [SerializeField] public AudioClip audioClip;
    [SerializeField] private TextAsset pathsInfo;

    private IMusicPlayer musicPlayer;

    private void OnEnable()
    {
        SetMusic();
    }

    void SetMusic()
    {
        musicPlayer = Injection.Get<IMusicPlayer>();

        //string trackName = Enum.GetName(typeof(MusicTracks.Track), roomStartMusic);
        //musicPlayer.Play(trackName, 0.5f);

        if (audioClip!=null)
        {
            musicPlayer.Play(audioClip, 0.5f);
        }
    }

    public void SetPathfinding()
    {
        if (pathsInfo!=null)
        {
            AstarPath.active.data.DeserializeGraphs(pathsInfo.bytes);
        }
    }

    public void SetCameras(Transform player)
    {
        virtualCamerasList.ForEach(camera => camera.Follow = player);
    }

    public void DeactivateCameras()
    {
        virtualCamerasList.ForEach(camera => camera.gameObject.SetActive(false));
    }

    public void ActivateCameras()
    {
        virtualCamerasList.ForEach(camera => camera.gameObject.SetActive(true));
    }

    public void ActivateColliders()
    {
        if (collidersContainer)
        {
            collidersContainer.SetActive(true);
        }
    }

    public void DeactivateColliders()
    {
        if (collidersContainer)
        {
            collidersContainer.SetActive(false);
        }
    }
}