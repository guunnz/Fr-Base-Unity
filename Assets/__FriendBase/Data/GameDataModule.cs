using Architecture.Context;
using Architecture.Injector.Core;
using Data.Catalog.Items;
using Data.Catalog;

namespace Data
{
    public class GameDataModule : IModule
    {
        public void Init()
        {
            Injection.Register<IGameData, GameData>();
            Injection.Register<IItemTypeUtils, ItemTypeUtils>();
        }
    }
}


