namespace Settings.Domain
{
    public struct SettingsModel
    {
        public VideoSettingsModel videoSettings;
        public AudioSettingsModel audioSettings;
        public GameSettingsModel gameSettings;
    }

    public struct GameSettingsModel
    {
        public DifficultyLevel difficulty;
    }

    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard
    }

    public struct AudioSettingsModel
    {
        public float masterVol;
        public float musicVol;
        public float sfxVol;
    }

    public enum VideoQuality
    {
        Low,
        Mid,
        High
    }

    public struct VideoSettingsModel
    {
        public VideoQuality quality;
        public bool fullScreen;
    }
}