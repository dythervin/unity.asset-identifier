#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

namespace Dythervin.AssetIdentifier.Editor
{
    internal static class AssetGroups
    {
        [Serializable]
        internal class Data
        {
            public Dictionary<string, ObjData> identifiers;
            public uint version;
        }

        private const string FileName = "AssetsGroups.json";
        private static readonly string FullPath;
        private static readonly Data SData;

        static AssetGroups()
        {
            string dirPath = $"{Environment.CurrentDirectory}/ProjectSettings";
            FullPath = $"{dirPath}/{FileName}";

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            SData = File.Exists(FullPath)
                ? JsonConvert.DeserializeObject<Data>(File.ReadAllText(FullPath))
                : new Data { identifiers = new Dictionary<string, ObjData>() };
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            Assets.Assets.OnBeforeSave += AssetsOnOnBeforeSave;
        }

        internal static ObjData GetData(string name)
        {
            if (SData.identifiers.TryGetValue(name, out ObjData value))
                return value;

            return SData.identifiers[name] = new ObjData();
        }

        private static void AssetsOnOnBeforeSave()
        {
            if (!ObjData.dirty)
                return;

            Save();
            ObjData.dirty = false;
        }


        private static HashSet<IUnique> GetAssets()
        {
            return Assets.Assets.LoadAllInterface<IUnique>(Assets.Assets.ObjFlags.ScriptableObject);
        }

        [MenuItem("Tools/Configs/Request Ids")]
        private static void RequestIds()
        {
            RequestIds(GetAssets());
        }

        private static void RequestIds(HashSet<IUnique> uniques)
        {
            foreach (IUnique configObjectBase in uniques)
                configObjectBase.RequestID();

            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Configs/Reset Counters")]
        private static void ResetCounter()
        {
            foreach (ObjData identifier in SData.identifiers.Values)
                identifier.ResetCounter();

            SData.version++;
            Save();
        }

        [MenuItem("Tools/Configs/[Warning] Reset Ids")]
        private static void ResetIds()
        {
            foreach (ObjData identifier in SData.identifiers.Values)
                identifier.ResetCounter();

            var assets = GetAssets();
            foreach (IUnique file in assets)
                file.SetID(0);

            RequestIds(assets);
            SData.version++;
            Save();
        }

        private static void Save()
        {
            File.WriteAllText(FullPath, JsonConvert.SerializeObject(SData, Formatting.Indented));
        }

        [MenuItem("Tools/Configs/Update")]
        private static void UpdateIds()
        {
            foreach (ObjData identifier in SData.identifiers.Values)
            {
                identifier.UpdateId();
            }

            Save();
        }

        internal static ulong Version => SData.version;
    }
}
#endif