using System;
using System.Diagnostics;
using Dythervin.AssetIdentifier.Editor;
using Dythervin.Core.Extensions;
using Dythervin.Core.Utils;
using UnityEditor;
using UnityEngine;

namespace Dythervin.AssetIdentifier
{
    public static class AssetIdentifiedHelper
    {
#if UNITY_EDITOR
        public static event Action OnUpdated;
#endif

        [Conditional(Symbols.UNITY_EDITOR)]
        public static void RequestID<T>(this T asset)
            where T : ScriptableObject, IIdentifiedAsset
        {
            RequestID(asset, asset);
        }

        [Conditional(Symbols.UNITY_EDITOR)]
        internal static void RequestID(this IIdentifiedAsset asset)
        {
            RequestID(asset, (ScriptableObject)asset);
        }

        [Conditional(Symbols.UNITY_EDITOR)]
        internal static void RequestID(this IIdentifiedAsset asset, ScriptableObject scriptableObject)
        {
#if UNITY_EDITOR
            MonoScript monoScript = MonoScript.FromScriptableObject(scriptableObject);
            GUID guid = AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(monoScript));
            AssetIdsData assetGroupData = AssetIds.GetGroupData(guid);
            AssetId newId = assetGroupData.GetId(asset);
            if (newId == asset.Id)
                return;

            asset.SetID(newId);
            scriptableObject.Dirty();
            OnUpdated?.Invoke();
#endif
        }
    }
}