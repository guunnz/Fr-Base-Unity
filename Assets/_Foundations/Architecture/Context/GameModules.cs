using System.Collections.Generic;
using AvatarCustomization;
using BurguerMenu;
using CanvasInput;
using ChatView;
using CloudStorage;
using ConnectingToServer;
using Currency;
using FeatureToggle;
using Localization;
using LocalStorage;
using MemoryStorage;
using Settings;
using User;
using Multiuser;
using DebugConsole;
using Data;
using DeviceInput;
using FriendsView;
using PlayerRoom;
using Socket;
using WebClientTools;
using InAppPurchasing;
using LocalizationSystem;

namespace Architecture.Context
{
    /// features that requires blocking modules
    public class AfterGameModules : GameModules
    {
        public override void Declare()
        {
            //setup dependencies in the correct order

            Dependency<CloudStorageModule>();
            Dependency<FeatureToggleModule>();
            Dependency<BurguerMenuModule>();
            Dependency<FriendsModule>();
            Dependency<PlayerRoomModule>();
            Dependency<SocketModule>();
            Dependency<ChatModule>();

        }
    }

    ///settings and foundations
    public class BeforeGameModules : GameModules
    {
        public override void Declare()
        {
            //setup dependencies in the correct order
            Dependency<MemoryStorageModule>();
            Dependency<LocalStorageService>();
            Dependency<LocalizationModule>();
            Dependency<SettingsModule>();
            Dependency<CurrenciesModule>();
            Dependency<UserModule>();
            Dependency<MultiuserModule>();
            Dependency<AnalyticsModule>();
            Dependency<DebugConsoleModule>();
            Dependency<GameDataModule>();
            Dependency<WebClientToolModule>();
            Dependency<DeviceInputModule>();
            Dependency<CanvasInputModule>();
            Dependency<InAppPurchasingModule>();
            Dependency<AvatarEndpointsModule>();
            Dependency<RoomListEndpointsModule>();
            Dependency<LanguageModule>();
            Dependency<NauthFlowEndpointsModule>();
            Dependency<ProviderModule>();
        }
    }

    
    public abstract class GameModules : List<IModule>
    {
        public abstract void Declare();

        protected void Dependency<T>() where T : class, IModule, new()
        {
            Add(new T());
        }
    }
}