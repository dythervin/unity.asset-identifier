#if UNITY_EDITOR
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dythervin.Addressables;
using Dythervin.Collections;
using Dythervin.Core.Extensions;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Dythervin.AssetIdentifier.Addressables
{
    public partial class AssetIdentifiedDatabase
    {
        private bool _isRefreshNeeded;

        static AssetIdentifiedDatabase()
        {
#if UNITY_EDITOR
            AssetIdentifiedHelper.OnUpdated += OnAssetIdentifiedHelperUpdated;
#endif
        }

        private static void OnAssetIdentifiedHelperUpdated()
        {
            Instance.AssetsDirty();
        }

        partial void RefreshIfNeeded()
        {
            if (_isRefreshNeeded)
            {
                Refresh();
                _isRefreshNeeded = false;
                return;
            }
            
            if (map != null && ((_allReferences != null && map.Count >= _allReferences.Length) ||
                                (_allIds != null && map.Count >= _allIds.Length)))
            {
                _allReferences = null;
                _allIds = null;
            }
        }

        [MenuItem("Tools/AssetIdentifier/ForceRefreshDatabase")]
        private static void ForceRefresh()
        {
            Instance.Refresh();
        }

        public void Refresh()
        {
            SetIds(Assets.Assets.LoadAllInterface<IIdentifiedAsset>(Assets.Assets.ObjectType.ScriptableObject));
        }

        public void AssetsDirty()
        {
            _isRefreshNeeded = true;
            ResetAll();
        }

        private void Reset()
        {
            ResetAll();
            Refresh();
        }

        [SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
        private void SetIds(IReadOnlyCollection<IIdentifiedAsset> objects)
        {
            map ??= new SerializedDictionary<ushort, SerializedDictionary<uint, AssetReferenceExt>>();
            map.Clear();
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;

            var defaultGroup = settings.DefaultGroup;
            // if (group == null)
            //     settings.CreateGroup(groupName, false, false, false, settings.DefaultGroup.Schemas, settings.DefaultGroup. );

            int i = 0;
            foreach (var identifiedAsset in objects)
            {
                Object asset = (Object)identifiedAsset;
                string path = AssetDatabase.GetAssetPath(asset);
                string guid = AssetDatabase.AssetPathToGUID(path);
                var id = identifiedAsset.Id;
                AddressableAssetEntry entry = settings.FindAssetEntry(guid);
                if (entry == null)
                {
                    entry = settings.CreateOrMoveEntry(guid, defaultGroup, true, false);
                    defaultGroup.Dirty();
                }

                if (!map.TryGetValue(id.group, out var refMap))
                    map[id.group] = refMap = new SerializedDictionary<uint, AssetReferenceExt>();

                refMap[id.id] = new AssetReferenceExt(guid);

                i++;
            }

            this.Dirty();
        }
    }
}
#endif