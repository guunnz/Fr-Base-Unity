using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugConsole
{
    public enum LOG_TYPE { TRAMA_IN, TRAMA_OUT, TRAMA_IN_WITH_STATE, IAP, ADVERTISING, ANALYTICS, SOCKET_INFO, SOCKET_IN, SOCKET_OUT, GENERAL, WEB_RESPONSE, AUTH_FLOW };

    public class DebugConsole : IDebugConsole
    {
        private Dictionary<LOG_TYPE, DebugLogItem> logElements = new Dictionary<LOG_TYPE, DebugLogItem>();

        public DebugConsole()
        {
            logElements.Add(LOG_TYPE.TRAMA_IN, new DebugLogItem(LOG_TYPE.TRAMA_IN, "<<<<<", "green", false));
            logElements.Add(LOG_TYPE.TRAMA_OUT, new DebugLogItem(LOG_TYPE.TRAMA_OUT, ">>>>>", "yellow", false));
            logElements.Add(LOG_TYPE.TRAMA_IN_WITH_STATE, new DebugLogItem(LOG_TYPE.TRAMA_IN_WITH_STATE, "<<<<<", "gray", false));
            logElements.Add(LOG_TYPE.IAP, new DebugLogItem(LOG_TYPE.IAP, "--", "yellow", false));
            logElements.Add(LOG_TYPE.ADVERTISING, new DebugLogItem(LOG_TYPE.ADVERTISING, "...", "blue", false));
            logElements.Add(LOG_TYPE.ANALYTICS, new DebugLogItem(LOG_TYPE.ANALYTICS, "", "blue", false));

            logElements.Add(LOG_TYPE.SOCKET_INFO, new DebugLogItem(LOG_TYPE.SOCKET_INFO, "-----", "yellow", false));
            logElements.Add(LOG_TYPE.SOCKET_IN, new DebugLogItem(LOG_TYPE.SOCKET_IN, "<<<<<", "green", false));
            logElements.Add(LOG_TYPE.SOCKET_OUT, new DebugLogItem(LOG_TYPE.SOCKET_OUT, ">>>>>", "blue", false));

            logElements.Add(LOG_TYPE.GENERAL, new DebugLogItem(LOG_TYPE.GENERAL, "---", "green", false));
            logElements.Add(LOG_TYPE.WEB_RESPONSE, new DebugLogItem(LOG_TYPE.WEB_RESPONSE, "---", "green", false));

            logElements.Add(LOG_TYPE.AUTH_FLOW, new DebugLogItem(LOG_TYPE.AUTH_FLOW, "---", "green", true));
        }

        public bool isLogTypeEnable(LOG_TYPE logType)
        {
            if (logElements.ContainsKey(logType))
            {
                return logElements[logType].active;
            }
            else
            {
                ErrorLog("DebugConsole:isLogTypeEnable", "Invalid Log Type:" + logType, "");
            }
            return false;
        }

        public void ErrorLog(string className, string msg, string error)
        {
            //Debug.LogError("className:" + className + "\n msg:" + msg + "\n error:" + error);

            Debug.Log("<color=red>Error: </color> className:" + className + "\n msg:" + msg + "\n error:" + error);
        }

        public void TraceLog(LOG_TYPE logType, string text)
        {
            if (logElements.ContainsKey(logType))
            {
                Debug.Log("<color=" + logElements[logType].color + "> " + logElements[logType].prefix + ": </color>" + text);
            }
            else
            {
                Debug.Log("<color=green> Log: </color>" + text);
            }
        }
    }
}