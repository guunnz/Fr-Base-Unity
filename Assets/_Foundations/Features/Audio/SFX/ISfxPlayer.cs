namespace Audio.SFX
{
    public interface ISfxPlayer
    {
        void Play(string sfxKey, float vol = 1);
    }
}