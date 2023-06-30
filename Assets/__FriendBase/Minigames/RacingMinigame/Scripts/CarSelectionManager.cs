using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CarSkin
{
    public CarSelectionManager.CarSkinEnum Car;
    public Material carMaterial;
    public Material driverMaterial;
    public Color carColor;
}

[System.Serializable]
public class CarSkinMenu : CarSkin
{
    public GameObject selectionObject;
}

public class CarSelectionManager : MonoBehaviour
{
    public RacingMinigame RM;
    public SkinnedMeshRenderer CarSkinnedMesh;
    public SkinnedMeshRenderer DriverSkinnedMesh;
    public List<CarSkinMenu> carSkinMenus = new List<CarSkinMenu>();
    [SerializeField] AudioSource _AudioSource;
    [SerializeField] List<RacingSound> RacingSounds;
    public enum CarSkinEnum
    {
        pink = 0,
        red = 1,
        blue = 2,
        green = 3,
        yellow = 4,
    }

    public void playAudio(eRacingSound sound)
    {
        _AudioSource.clip = RacingSounds.Single(x => x.racingSound == sound).AudioClip;
        _AudioSource.Play();
    }

    public void Confirm()
    {
        playAudio(eRacingSound.FA_Confirm_Button_1_3);
    }

    public void SetCarSkin(int skinNum)
    {
        playAudio(eRacingSound.FA_Select_Button_1_1);
        carSkinMenus.ForEach(x => x.selectionObject.SetActive(false));
        CarSkinMenu carSkin = carSkinMenus.Single(x => x.Car == (CarSkinEnum)skinNum);
        carSkin.selectionObject.SetActive(true);
        CarSkinnedMesh.material = carSkin.carMaterial;
        DriverSkinnedMesh.material = carSkin.driverMaterial;

        if (RM.IsMultiplayerGame())
        {
            RM.MultiplayerManager.SetCarSkin(skinNum);
        }
        else
        {
            PlayerPrefs.SetInt("PlayerSkin", skinNum);
        }
    }
}