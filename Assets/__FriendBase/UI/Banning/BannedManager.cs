using System.Collections;
using System.Collections.Generic;
using Architecture.Injector.Core;
using Data;
using LocalizationSystem;
using UnityEngine;

public class BannedManager : MonoBehaviour
{
    [SerializeField] protected UIMsgPanelBanned panelBanned;

    private IGameData gameData;
    protected ILanguage language;

    void Start()
    {
        language = Injection.Get<ILanguage>();
        gameData = Injection.Get<IGameData>();

        if (gameData.GetUserInformation().UserStatus.IsBanned())
        {
            panelBanned.OpenWithBannedDescription(null);
        }
    }
}
