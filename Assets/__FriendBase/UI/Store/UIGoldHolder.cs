using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Data;
using Architecture.Injector.Core;

public class UIGoldHolder : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtAmountGold;

    private IGameData gameData;

    void Start()
    {
        gameData = Injection.Get<IGameData>();
        gameData.GetUserInformation().OnGoldUpdate += OnGoldUpdate;

        UpdateGold();
    }

    void OnGoldUpdate(int newAmount, int amountAdded)
    {
        UpdateGold();
    }

    void UpdateGold()
    {
        txtAmountGold.text = gameData.GetUserInformation().Gold.ToString();
    }

    private void OnDestroy()
    {
        gameData.GetUserInformation().OnGoldUpdate -= OnGoldUpdate;
    }
}
