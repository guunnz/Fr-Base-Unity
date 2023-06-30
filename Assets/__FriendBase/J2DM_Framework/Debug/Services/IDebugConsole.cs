namespace DebugConsole
{
    public interface IDebugConsole
    {
        bool isLogTypeEnable(LOG_TYPE logType);
        void ErrorLog(string className, string msg, string error);
        void TraceLog(LOG_TYPE logType, string text);
    }
}