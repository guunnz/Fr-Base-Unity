using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DatePickerModal : AbstractUIPanel
{
    [SerializeField] protected UIDataPickerScrollView dayPicker;
    [SerializeField] protected UIDataPickerScrollView monthPicker;
    [SerializeField] protected UIDataPickerScrollView yearPicker;

    [SerializeField] protected TextMeshProUGUI txtTitle;
    [SerializeField] protected TextMeshProUGUI txtButton;

    public delegate void DateSelected(int year, int month, int day);
    public event DateSelected OnDateSelected;

    protected override void Start()
    {
        base.Start();
        dayPicker.OnChangeData += OnChangeDataDay;
        monthPicker.OnChangeData += OnChangeDataMonthYear;
        yearPicker.OnChangeData += OnChangeDataMonthYear;
    }

    public override void OnOpen()
    {
        language.SetTextByKey(txtTitle, LangKeys.NAUTH_SELECT_BIRTH);
        language.SetTextByKey(txtButton, LangKeys.NAUTH_SELECT);
    }

    void OnChangeDataDay(UIDataPickerCardController card)
    {

    }

    void OnChangeDataMonthYear(UIDataPickerCardController card)
    {
        DataPickerData yearData = yearPicker.GetDataPicerDataSelected();
        if (yearData == null)
        {
            return;
        }

        DataPickerData monthData = monthPicker.GetDataPicerDataSelected();
        if (monthData == null)
        {
            return;
        }

        int days = DateTime.DaysInMonth(yearData.Id, monthData.Id);

        dayPicker.ChangeDayAmount(days);
    }

    void OnDestroy()
    {
        dayPicker.OnChangeData -= OnChangeDataDay;
        monthPicker.OnChangeData -= OnChangeDataMonthYear;
        yearPicker.OnChangeData -= OnChangeDataMonthYear;
    }

    public void OnSelectPressed()
    {
        DataPickerData yearData = yearPicker.GetDataPicerDataSelected();
        if (yearData == null)
        {
            return;
        }

        DataPickerData monthData = monthPicker.GetDataPicerDataSelected();
        if (monthData == null)
        {
            return;
        }

        DataPickerData dayData = dayPicker.GetDataPicerDataSelected();
        if (dayData == null)
        {
            return;
        }

        if (OnDateSelected!=null)
        {
            OnDateSelected(yearData.Id, monthData.Id, dayData.Id);
        }
        Close();
    }
}
