using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;
using Architecture.Injector.Core;

public class UIGemsHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtAmountGems;

    private IGameData gameData;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        gameData.GetUserInformation().OnGemsUpdate += OnGemsUpdate;
        
        UpdateGems();
    }

    void OnGemsUpdate(int newAmount, int amountAdded)
    {
        UpdateGems();
    }

    void UpdateGems()
    {
        txtAmountGems.text = gameData.GetUserInformation().Gems.ToString();
    }

    private void OnDestroy()
    {
        gameData.GetUserInformation().OnGemsUpdate -= OnGemsUpdate;
    }
}
