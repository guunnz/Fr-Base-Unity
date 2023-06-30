using System.Collections.Generic;
using ChatView.Core.Services;
using JetBrains.Annotations;
using UnityEngine;

namespace ChatView.Infrastructure
{
    [UsedImplicitly]
    public class ChatServices : IChatServices
    {
        public const int maxMessageLenght = 144;
        const float  minMessageSeconds = 1f; 
        const float  messageToSecondsMultiplier = 5f;

        static string chatHistory;
        static bool clearHistory;

        //Friendbase chat colors
        //Hexadecimal colors.
        public const string friendbaseBlackHex = "#000000";
        public const string friendbaseBlueHex = "#33BCD2";
        public const string friendbaseWineHex = "#7A1602";
        public const string friendbaseGreenHex = "#82CB34";
        public const string friendbaseOrangeHex = "#ED4F39";

        public static readonly List<string> colorsHex = new List<string>
        {
            friendbaseBlackHex,
            friendbaseBlueHex,
            friendbaseWineHex,
            friendbaseGreenHex,
            friendbaseGreenHex,
            friendbaseOrangeHex
        };

        public static string ChatHistory
        {
            get => chatHistory;
            set => chatHistory = value;
        }

        public static bool ClearHistory
        {
            get => clearHistory;
            set => clearHistory = value;
        }

        public float MessageToSeconds(string message)
        {
            float messageLenght = message.Length;
            
            var seconds = messageToSecondsMultiplier * (messageLenght / maxMessageLenght);
            
            return seconds < minMessageSeconds ? minMessageSeconds : seconds;
        }

        public static List<Color> GetChatColorsList()
        {
            List<Color> friendBaseColors = new List<Color>();

            foreach (var colorHex in colorsHex)
            {
                ColorUtility.TryParseHtmlString(colorHex, out var result);
                friendBaseColors.Add(result);
            }

            return friendBaseColors;
        }

    }
}