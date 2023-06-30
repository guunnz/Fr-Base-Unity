namespace DebugConsole
{
    public class DebugLogItem
    {
        public LOG_TYPE logType;
        public string color;
        public string prefix;
        public bool active;

        public DebugLogItem(LOG_TYPE logType, string prefix, string color, bool active)
        {
            this.logType = logType;
            this.prefix = prefix;
            this.color = color;
            this.active = active;
        }
    }
}