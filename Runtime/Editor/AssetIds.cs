#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Dythervin.AssetIdentifier.Editor
{
    internal static class AssetIds
    {
        [Serializable]
        internal class Data : ISerializationCallbackReceiver
        {
            [JsonIgnore]
            public readonly Dictionary<GUID, AssetIdsData> map = new();

            [JsonProperty]
            private Dictionary<string, AssetIdsData> _map;

            [JsonProperty]
            public ushort nextGroupId = 1;

            public void ResetValues()
            {
                nextGroupId = 1;
                map.Clear();
            }

            public void OnBeforeSerialize()
            {
                _map ??= new Dictionary<string, AssetIdsData>();
                _map.Clear();
                foreach (var data in map)
                {
                    _map[data.Key.ToString()] = data.Value;
                }
            }

            public void OnAfterDeserialize()
            {
                map.Clear();
                if (_map == null)
                {
                    return;
                }

                foreach (var data in _map)
                {
                    GUID.TryParse(data.Key, out GUID guid);
                    map[guid] = data.Value;
                }
            }
        }

        private const string FileName = "AssetIds.json";

        private static readonly string FullPath;

        private static readonly Data SData;

        static AssetIds()
        {
            string dirPath = $"{Environment.CurrentDirectory}/ProjectSettings";
            FullPath = $"{dirPath}/{FileName}";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            if (File.Exists(FullPath))
            {
                SData = JsonConvert.DeserializeObject<Data>(File.ReadAllText(FullPath));
                SData.OnAfterDeserialize();
            }
            else
            {
                SData = new Data();
                SData.ResetValues();
            }
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            Assets.Assets.OnBeforeSave += AssetsOnOnBeforeSave;
        }

        internal static AssetIdsData GetGroupData(GUID name)
        {
            if (!SData.map.TryGetValue(name, out AssetIdsData value) || value.GroupId == 0)
            {
                value = SData.map[name] = new AssetIdsData(SData.nextGroupId++);
            }

            return value;
        }

        private static void AssetsOnOnBeforeSave()
        {
            if (!AssetIdsData.dirty)
            {
                return;
            }

            Save();
            AssetIdsData.dirty = false;
        }

        private static HashSet<IIdentifiedAsset> GetAssets()
        {
            return Assets.Assets.LoadAllInterface<IIdentifiedAsset>(Assets.Assets.ObjectType.ScriptableObject);
        }

        [MenuItem("Tools/Configs/Request Ids")]
        private static void RequestIds()
        {
            RequestIds(GetAssets());
        }

        private static void RequestIds(HashSet<IIdentifiedAsset> uniques)
        {
            foreach (IIdentifiedAsset configObjectBase in uniques)
            {
                configObjectBase.RequestID();
            }

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Configs/Reset Counters")]
        private static void ResetCounter()
        {
            ResetCounterInternal();

            Save();
        }

        private static void ResetCounterInternal()
        {
            SData.ResetValues();
            foreach (AssetIdsData identifier in SData.map.Values)
            {
                identifier.ResetCounter();
            }
        }

        [MenuItem("Tools/Configs/[Warning] Reset Ids")]
        private static void ResetIds()
        {
            ResetCounterInternal();

            var assets = GetAssets();
            foreach (IIdentifiedAsset file in assets)
            {
                file.SetID(default);
            }

            RequestIds(assets);
            Save();
        }

        private static void Save()
        {
            SData.OnBeforeSerialize();
            File.WriteAllText(FullPath, JsonConvert.SerializeObject(SData, Formatting.Indented));
        }

        [MenuItem("Tools/Configs/Update")]
        private static void UpdateIds()
        {
            foreach (AssetIdsData identifier in SData.map.Values)
            {
                identifier.UpdateId();
            }

            Save();
        }
    }
}
#endif