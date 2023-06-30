using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UI.ScrollView;
using Data.Rooms;
using LocalizationSystem;
using Architecture.Injector.Core;

public class UISelectEventType : AbstractUIPanel
{
    [SerializeField] private UISelectEventTypeScrollView selectEventTypeScrollView;
    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] private UIHostEventPanel hostEventPanel;

    private List<SelectEventTypeData> listEventTypeData;
    private RoomInformation roomInformation;
    private static List<SelectEventTypeData> listEventTypeDataCache;

    static public UISelectEventType instance;

    private void Awake()
    {
        instance = this;
    }

    protected override void Start()
    {
        base.Start();
        txtTitle.text = language.GetTextByKey(LangKeys.EVENTS_SELECT_TYPE_OF_EVENT);
        listEventTypeData = GetEventTypes();

        selectEventTypeScrollView.OnCardSelected += OnCardSelected;
    }

    public SelectEventTypeData GetDefault()
    {
        return listEventTypeData[0];
    }

    void OnDestroy()
    {
        selectEventTypeScrollView.OnCardSelected -= OnCardSelected;
    }

    void OnCardSelected(SelectEventTypeData selectEventTypeData, UIAbstractCardController cardController)
    {
        Close();
        hostEventPanel.OpenWithSelectTypeSelected(roomInformation, selectEventTypeData);
    }

    public void Open(RoomInformation roomInformation)
    {
        base.Open();
        this.roomInformation = roomInformation;
        selectEventTypeScrollView.ShowObjects(listEventTypeData);
    }

    public List<SelectEventTypeData> GetEventTypes()
    {
        if (listEventTypeDataCache != null)
        {
            return listEventTypeDataCache;
        }

        listEventTypeDataCache = new List<SelectEventTypeData>();
        listEventTypeDataCache.Add(new SelectEventTypeData(0, this.language.GetTextByKey(LangKeys.EVENTS_BEACH_PARTY)));
        listEventTypeDataCache.Add(new SelectEventTypeData(1, this.language.GetTextByKey(LangKeys.EVENTS_BIRTHDAY_PARTY)));
        listEventTypeDataCache.Add(new SelectEventTypeData(2, this.language.GetTextByKey(LangKeys.EVENTS_CHILLAX)));
        listEventTypeDataCache.Add(new SelectEventTypeData(3, this.language.GetTextByKey(LangKeys.EVENTS_COFFEE_BREAK)));
        listEventTypeDataCache.Add(new SelectEventTypeData(4, this.language.GetTextByKey(LangKeys.EVENTS_GARDEN_PARTY)));
        listEventTypeDataCache.Add(new SelectEventTypeData(5, this.language.GetTextByKey(LangKeys.EVENTS_RAIN_DANCE)));
        listEventTypeDataCache.Add(new SelectEventTypeData(6, this.language.GetTextByKey(LangKeys.EVENTS_PARTY)));
        listEventTypeDataCache.Add(new SelectEventTypeData(7, this.language.GetTextByKey(LangKeys.EVENTS_YACHT_PARTY)));
        listEventTypeDataCache.Add(new SelectEventTypeData(8, this.language.GetTextByKey(LangKeys.EVENTS_MY_CRIB)));
        listEventTypeDataCache.Add(new SelectEventTypeData(9, this.language.GetTextByKey(LangKeys.EVENTS_MY_PLACE)));

        return listEventTypeDataCache;
    }

    public string GetEventTypeNameFromIndex(int index)
    {
        string nameEventType = "";
        List<SelectEventTypeData> listEventTypeData = GetEventTypes();
        foreach (SelectEventTypeData eventTypeData in listEventTypeData)
        {
            if (index == eventTypeData.Index)
            {
                return eventTypeData.TextEvent;
            }
        }

        return nameEventType;
    }
}
