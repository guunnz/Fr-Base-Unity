using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UINauthFlowDateInput : MonoBehaviour
{
    [SerializeField] private DatePickerModal datePickerModal;
    [SerializeField] private TextMeshProUGUI inputField;
    [SerializeField] private TextMeshProUGUI txtTitle;

    public int year { get; private set; } = -1000;
    public int month { get; private set; } = -1000;
    public int day { get; private set; } = -1000;

    public bool isReady { get; private set; } = false;
    private Color color;

    private void Start()
    {
        datePickerModal.OnDateSelected += OnDateSelected;
        inputField.text = "DD/MM/YYYY";
        
        if (ColorUtility.TryParseHtmlString("#" + "b79b94", out color))
        {
            inputField.color = color;
        }
    }

    void OnDestroy()
    {
        datePickerModal.OnDateSelected -= OnDateSelected;
    }

    public void SetTitle(string text)
    {
        txtTitle.text = text;
    }

    public string GetText()
    {
        if (!isReady)
        {
            return null;
        }
        DateTimeOffset date = new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero);
        string formattedDate = date.ToString("yyyy-MM-ddTHH:mm:ss.fffffffzzz");

        Debug.Log("formattedDate:" + formattedDate);

        return formattedDate;
    }

    public void SetDate(int year, int month, int day)
    {
        this.year = year;
        this.month = month;
        this.day = day;

        inputField.text = day.ToString("00") + "/"+ month.ToString("00") + "/" + year;

        if (ColorUtility.TryParseHtmlString("#" + "1c2430", out color))
        {
            inputField.color = color;
        }

        isReady = true;
    }

    public void OnOpenDatePicker()
    {
        datePickerModal.Open();
    }

    void OnDateSelected(int year, int month, int day)
    {
        SetDate(year, month, day);
    }
}
