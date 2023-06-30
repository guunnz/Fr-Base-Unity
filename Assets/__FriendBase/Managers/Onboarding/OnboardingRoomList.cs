using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnboardingRoomList : AbstractUIPanel
{
    [SerializeField] private TextMeshProUGUI TxtClose;
    [SerializeField] private TextMeshProUGUI TxtPlaces;
    [SerializeField] private TextMeshProUGUI TxtEvents;

    protected override void Start()
    {
        base.Start();
    }

    public override void OnOpen()
    {
        language.SetTextByKey(TxtClose, LangKeys.ONBOARDING_LETS_VISIT_THE_PARK);
        language.SetTextByKey(TxtPlaces, LangKeys.NAV_PLACES);
        language.SetTextByKey(TxtEvents, LangKeys.NAV_EVENTS);
    }
}
