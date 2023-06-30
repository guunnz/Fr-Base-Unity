using System;
using System.Collections.Generic;
using System.Linq;
using Functional.Maybe;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.DateField
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class WriteOnDropdown : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerClickHandler
    {
        const int firstLetter = (int) KeyCode.A;
        const int lastLetter = (int) KeyCode.Z;
        const int firstNumber = (int) KeyCode.Alpha0;
        const int lastNumber = (int) KeyCode.Alpha9;

        TMP_Dropdown _dropdown;
        TMP_Dropdown Dropdown => _dropdown ??= GetComponent<TMP_Dropdown>();

        List<TMP_Dropdown.OptionData> _options;
        List<TMP_Dropdown.OptionData> Options => OptionsChange() ? _options = Dropdown.options : _options;

        string searchingText = string.Empty;


        bool OptionsChange()
        {
            if (_options == Dropdown.options) return false;
            if (_options == null) return true;
            if (_options.Count != Dropdown.options.Count) return true;
            return _options.Where((t, i) => t.text != Dropdown.options[i].text).Any();
        }

        void ResetWriteText()
        {
            searchingText = string.Empty;
        }

        public void OnSelect(BaseEventData eventData)
        {
            ResetWriteText();
            OpenKeyboard();
        }

        void OpenKeyboard()
        {
            if (TouchScreenKeyboard.isSupported && !TouchScreenKeyboard.visible)
            {
                TouchScreenKeyboard.Open("Search", TouchScreenKeyboardType.Search);
            }
        }

        public void OnDeselect(BaseEventData eventData)
        {
            ResetWriteText();
        }


        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject != gameObject) return;
            if (Input.anyKey)
            {
                var codes = LettersAndNumbers().ToList();
                var firstMaybe = codes.FirstMaybe(Input.GetKeyDown);
                var selectGetChar = firstMaybe.Select(GetChar);
                var appendSearchingText = selectGetChar.Do(ch => searchingText += ch);
                var updateTextSearch = appendSearchingText.Do(UpdateTextSearch);
                updateTextSearch.DoOnAbsent(OnKeyNotLetter);
            }
        }

        void OnKeyNotLetter()
        {
            if (KeysThatResetText.Any(Input.GetKeyDown))
            {
                ResetWriteText();
            }
        }

        static readonly IReadOnlyList<KeyCode> KeysThatResetText = new[]
        {
            KeyCode.Escape,
            KeyCode.Return,
            KeyCode.KeypadEnter,
        };


        bool MatchTextWith(Func<string, string, bool> f)
        {
            for (var i = 0; i < Options.Count; i++)
            {
                if (f(Options[i].text.ToLower(), searchingText))
                {
                    Dropdown.value = i;
                    return true;
                }
            }

            return false;
        }

        void UpdateTextSearch()
        {
            var _ = MatchTextWith((opt, st) => opt == st) ||
                    MatchTextWith((opt, st) => opt.StartsWith(st)) ||
                    MatchTextWith((opt, st) => opt.EndsWith(st)) ||
                    MatchTextWith((opt, st) => opt.Contains(st));
        }

        static char GetChar(KeyCode code)
        {
            var codeNumber = (int) code;

            if (firstNumber <= codeNumber && codeNumber <= lastNumber)
            {
                return (char) ('0' + (codeNumber - firstNumber));
            }

            if (firstLetter <= codeNumber && codeNumber <= lastLetter)
            {
                return (char) ('a' + (codeNumber - firstLetter));
            }

            throw new ArgumentException("You should not be analyzing char for " + code);
        }

        static IEnumerable<KeyCode> LettersAndNumbers()
        {
            for (var i = firstNumber; i <= lastNumber; i++)
            {
                yield return (KeyCode) i;
            }

            for (var i = firstLetter; i <= lastLetter; i++)
            {
                yield return (KeyCode) i;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}