using Audio.Core;
using Settings.Domain;

namespace Audio.SFX
{
    public class SfxVolumeControl : VolumeControl
    {
        protected override float GetSourceVol(AudioSettingsModel settingsModel)
        {
            return settingsModel.sfxVol;
        }
    }
}