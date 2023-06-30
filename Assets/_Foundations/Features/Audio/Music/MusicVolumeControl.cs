using Audio.Core;
using Settings.Domain;

namespace Audio.Music
{
    public class MusicVolumeControl : VolumeControl
    {
        protected override float GetSourceVol(AudioSettingsModel settingsModel)
        {
            return settingsModel.musicVol;
        }
    }
}