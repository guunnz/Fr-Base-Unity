using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI.DateField
{
    public static class DateFieldUtil
    {
        public static readonly IReadOnlyList<TMP_Dropdown.OptionData> MonthsEnglish = new List<string>
        {
            "January", "February", "March", "April",
            "May", "June", "July", "August",
            "September", "October", "November", "December",
        }.Select(ToData).ToList();

        public static readonly IReadOnlyList<TMP_Dropdown.OptionData> MonthsKey = new List<string>
        {
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Oct",
            "Nov",
            "Dec",
        }.Select(ToData).ToList();

        public static readonly IReadOnlyList<int> DaysPerMonth = new[]
        {
            31, /*Jan*/ 29, /*Feb*/ 31, /*Mar*/ 30, /*Apr*/
            31, /*May*/ 30, /*Jun*/ 31, /*Jul*/ 31, /*Aug*/
            30, /*Sep*/ 31, /*Oct*/ 30, /*Nov*/ 31, /*Dec*/
        };


        static TMP_Dropdown.OptionData ToData(string info)
        {
            return new TMP_Dropdown.OptionData(info);
        }


        public static List<TMP_Dropdown.OptionData> NumbersInRange(int minYear, int maxYear, bool reverse = false)
        {
            var data = new List<TMP_Dropdown.OptionData>(maxYear - minYear);
            if (reverse)
            {
                for (int i = maxYear; i >= minYear; i--)
                {
                    data.Add(new TMP_Dropdown.OptionData(i.ToString()));
                }
            }
            else
            {
                for (int i = minYear; i <= maxYear; i++)
                {
                    data.Add(new TMP_Dropdown.OptionData(i.ToString()));
                }
            }

            return data;
        }

        public static IReadOnlyList<TMP_Dropdown.OptionData> GetMonths()
        {
            return MonthsEnglish;
        }
    }

    public class DateField : MonoBehaviour
    {
        [SerializeField] TMP_Dropdown dayDropdown;
        [SerializeField] TMP_Dropdown monthDropdown;
        [SerializeField] TMP_Dropdown yearDropdown;
        
        

        DateTime selectedDate = DateTime.UtcNow;
        

        void DisplayDate(int _ = 0)
        {
            var year = DropdownIndexToRealYear(yearDropdown.value);
            var month = monthDropdown.value;
            var day = dayDropdown.value;

            SetYMD(year, month, day);
        }

        

        void SetYMD(int year, int month, int day)
        {
            selectedDate = new DateTime(year, month + 1, day + 1);

            var today = DateTime.UtcNow;
            yearDropdown.options = DateFieldUtil.NumbersInRange(1900, today.Year, true);
            monthDropdown.options = DateFieldUtil.MonthsKey.ToList();
            dayDropdown.options = DateFieldUtil.NumbersInRange(1, DateFieldUtil.DaysPerMonth[monthDropdown.value]);
        }

        int RealYearToDropdownIndex(int year)
        {
            var currentYear = DateTime.UtcNow.Year;
            return currentYear - year;
        }

        int DropdownIndexToRealYear(int dropdownIndex)
        {
            var currentYear = DateTime.UtcNow.Year;
            return currentYear - dropdownIndex;
        }


        void Start()
        {
            DisplayDate();
            yearDropdown.onValueChanged.AddListener(DisplayDate);
            monthDropdown.onValueChanged.AddListener(DisplayDate);
            dayDropdown.onValueChanged.AddListener(DisplayDate);
        }

        public DateTime Date
        {
            get => selectedDate;
            set
            {
                SetYMD(value.Year, value.Month - 1, value.Day - 1);

                yearDropdown.value = RealYearToDropdownIndex(value.Year);
                monthDropdown.value = value.Month - 1;
                dayDropdown.value = value.Day - 1;
            }
        }

        public DateTime Value => Date;
        public bool HasValue => true;

        
    }
}