using UniRx;

namespace Settings.Presentation
{
    public interface ISettingsScreen
    {
        IReactiveProperty<float> MusicVol { get; }
        IReactiveProperty<float> SfxVol { get; }
        IReactiveProperty<float> MasterVol { get; }
        IReactiveProperty<int> Difficulty { get; }
        IReactiveProperty<int> Quality { get; }
        IReactiveProperty<bool> FullScreen { get; }
    }
}