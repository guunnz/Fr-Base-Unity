using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UIDataPickerScrollView;

public class DataPickerData 
{
    public string TxtButton { get; private set; }
    public int Id { get; private set; }
    public TYPE_DATE_PICKER TypeDatePicker;

    public DataPickerData(TYPE_DATE_PICKER typeDatePicker, int id, string text)
    {
        Id = id;
        TxtButton = text;
        TypeDatePicker = typeDatePicker;
    }
}
