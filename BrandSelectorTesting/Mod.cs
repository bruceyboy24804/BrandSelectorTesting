using BrandSelectorTesting.Systems;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Buildings;
using Game.Modding;
using Game.Prefabs;
using Game.SceneFlow;
using System.Data;
//using static BrandSelectorTesting.Setting;

namespace BrandSelectorTesting
{
    public class Mod : IMod
    {
        public static readonly string Id = "BrandSelectorTesting";

        /// <summary>
        /// Gets the static reference to the mod instance.
        /// </summary>
        public static Mod Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets ILog for mod.
        /// </summary>
        internal ILog Log { get; private set; }


        public static ILog log = LogManager.GetLogger($"{nameof(BrandSelectorTesting)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

           

            updateSystem.World.GetOrCreateSystemManaged<PrefabSystem>();

            updateSystem.World.GetOrCreateSystemManaged<BrandDataQuerySystem>();
            updateSystem.UpdateBefore<BrandDataQuerySystem>(SystemUpdatePhase.PrefabUpdate);
            updateSystem.UpdateAfter<BrandDataQuerySystem>(SystemUpdatePhase.PrefabReferences);
            updateSystem.World.GetOrCreateSystemManaged<BrandPrefabQuerySystem>();
            updateSystem.UpdateBefore<BrandPrefabQuerySystem>(SystemUpdatePhase.PrefabUpdate);
            updateSystem.UpdateAfter<BrandPrefabQuerySystem>(SystemUpdatePhase.PrefabReferences);
            updateSystem.UpdateAfter<SelectedInfoPanelCustomDropdownSystem>(SystemUpdatePhase.UIUpdate);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            
        }
    }
}
