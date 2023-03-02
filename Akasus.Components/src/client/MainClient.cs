using System.Collections.Generic;
using System.Linq;
using LogicAPI;
using LogicAPI.Client;
using LogicSettings;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Akasus.Components.Client
{
    public class MainClient : ClientMod
    {
        [Setting_SliderInt("Akasus.Components.Ram.DataBusSize",AutomaticallySaveInFiles = true)]
        public int DataBusCount { get; set; } = 3;

        public static AssetBundle modAssets { get; private set; }
        public static AssetBundle uiAssets { get; private set; }
        public static IDispatcher dispatcher { get; private set; }
        protected override void Initialize()
        {
            dispatcher = Dispatcher;
            //Logger.Info( string.Join(":",Enumerable.Range(0,32).Select(LayerMask.LayerToName)));
            modAssets = Assets.LoadAssetBundle("display");
            //No initialization code for this mod (yet).
            uiAssets = Assets.LoadAssetBundle("uielements");

            TryListResources();
            
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
            SettingsManager.Instance.;
        }

        private void TryListResources()
        {
            var res = Resources.FindObjectsOfTypeAll<GameObject>();

            var prefs = new List<GameObject>();


            foreach (var go in res)
            {
                var rootgo = go.transform.root.gameObject;
                if (!prefs.Contains(rootgo))
                {
                    prefs.Add(rootgo);
                }
            }
            Logger.Info("All: \n" + string.Join("\n", res.Select(e => e.name)));
            Logger.Info("Prefabs: \n" + string.Join("\n", prefs.Select(e => e.name)));
        }

        private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.buildIndex != 6) return;
            Logger.Info("Loading Ram Menu!");

            var menuInst = uiAssets.InstantiateAsset<GameObject>("RamSettingsMenu");
            var menu = menuInst.AddComponent<Ram256b8DEditMenu>();
            SceneManager.MoveGameObjectToScene(menuInst, arg0);
            menuInst.SetActive(false);
            Logger.Info("Call Initialize On Ram Menu");
            Helper.InitializeRecursive(menuInst);
        }
    }
}