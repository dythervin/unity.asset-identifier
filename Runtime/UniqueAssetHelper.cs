using System;
using System.Diagnostics;
using System.Reflection;
using Dythervin.Core.Extensions;
using Dythervin.Core.Utils;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Dythervin.AssetIdentifier
{
    public static class UniqueAssetHelper
    {
        [Conditional(Symbols.UNITY_EDITOR)]
        public static void RequestID<T>(this T asset)
            where T : ScriptableObject, IUnique
        {
            RequestID(asset, asset);
        }

        [Conditional(Symbols.UNITY_EDITOR)]
        internal static void RequestID(this IUnique asset)
        {
            RequestID(asset, (ScriptableObject)asset);
        }

        [Conditional(Symbols.UNITY_EDITOR)]
        internal static void RequestID(this IUnique asset, ScriptableObject scriptableObject)
        {
#if UNITY_EDITOR
            Type type = asset.GetType();
            AssetGroupAttribute attribute;
            do
            {
                attribute = type.GetCustomAttribute<AssetGroupAttribute>();

                type = type.BaseType;
            } while (attribute == null && type != null);

            if (attribute == null)
            {
                Debug.LogError($"{asset.GetType()} or base classes must have attribute ProjectIdentifier");
                return;
            }

            uint newId = Editor.AssetGroups.GetData(attribute.name).GetId(asset);
            if (newId == asset.Id)
                return;

            asset.SetID(newId);
            scriptableObject.Dirty();
#endif
        }
    }
}