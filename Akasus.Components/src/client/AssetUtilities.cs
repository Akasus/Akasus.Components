// File: AssetUtilities.cs
// Project: Akasus.Components
// Author: Akasus (akasus.nevelas@gmail.com)
// Description:

using LogicAPI.Client;
using UnityEngine;

namespace Akasus.Components.Client
{
    public static class AssetUtilities
    {
        /// <summary>
        /// Loads a prefab but does not Instantiate it!
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bundleName"></param>
        /// <param name="AssetName"></param>
        /// <returns></returns>
        public static T LoadPrefab<T>(this IAssets assets, string bundleName, string AssetName) where T : Object
        {
            return assets.LoadAssetBundle(bundleName).LoadAsset<T>(AssetName);
        }

        public static T InstantiateAsset<T>(this AssetBundle bundle, string AssetName) where T : UnityEngine.Object
        {
            return Object.Instantiate(bundle.LoadAsset<T>(AssetName));
        }
        public static T InstantiateAsset<T>(this AssetBundle bundle, string AssetName, Transform parent) where T : UnityEngine.Object
        {
            return Object.Instantiate(bundle.LoadAsset<T>(AssetName),parent);
        }

        /// <summary>
        /// Instantiates an Asset in the Current Scene
        /// </summary>
        /// <typeparam name="T"> for e.g a GameObject</typeparam>
        /// <param name="bundleName">the AssetBundle to load from</param>
        /// <param name="assetName">the Name of the Asset</param>
        /// <returns></returns>
        public static T InstantiatePrefab<T>(this IAssets assets, string bundleName, string assetName) where T : Object
        {
            return Object.Instantiate(assets.LoadPrefab<T>(bundleName, assetName));
        }

        /// <summary>
        /// Instantiates an Asset in the Current Scene an places it into another Object
        /// </summary>
        /// <typeparam name="T">for e.g a GameObject</typeparam>
        /// <param name="bundleName">the AssetBundle to load from</param>
        /// <param name="assetName">the Name of the Asset</param>
        /// <param name="parent">The parent object</param>
        /// <returns></returns>
        public static T InstantiatePrefab<T>(this IAssets assets, string bundleName, string assetName, Transform parent) where T : Object
        {
            return Object.Instantiate(assets.LoadPrefab<T>(bundleName, assetName), parent);
        }
    }
}