using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;


namespace ChatView.Infrastructure
{
    public class RandomColorUtilities
    {
        List<Color> colors;
        private Color lastColor;

        public RandomColorUtilities(List<Color> inputColors)
        {
            colors = inputColors;
            if (colors != null) lastColor = colors[0];
        }

        public Color RandomColor()
        {
            var shorList = colors.FindAll(c => c != lastColor);
            var random = new Random();
            int index = random.Next(shorList.Count);
            lastColor = shorList[index];
            return shorList[index];
        }

        public string RandomColorAsString()
        {
            var shorList = colors.FindAll(c => c != lastColor);
            var random = new Random();
            int index = random.Next(shorList.Count);
            lastColor = shorList[index];
            return ColorUtility.ToHtmlStringRGBA(shorList[index]);
        }

        public string ColorizeString(string text)
        {
            return "<color=#" + RandomColorAsString() + ">" + text + "</color>";
        }
    }
}