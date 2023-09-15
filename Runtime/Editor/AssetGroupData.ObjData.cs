#if UNITY_EDITOR
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Dythervin.Collections;
using Newtonsoft.Json;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Dythervin.AssetIdentifier.Editor
{
    [Serializable]
    internal class AssetIdsData
    {
        [JsonProperty] private uint _nextId = 1;

        [JsonProperty] public ushort GroupId { get; private set; }

        private readonly DictionaryCross<IIdentifiedAsset, uint> _objId = new DictionaryCross<IIdentifiedAsset, uint>();

        public static bool dirty;

        public AssetIdsData(ushort groupId) : this()
        {
            GroupId = groupId;
        }

        public AssetIdsData()
        {
            ClearValues();
        }

        public void ResetCounter()
        {
            ClearValues();
            dirty = true;
        }

        private void ClearValues()
        {
            _objId.Clear();
            _nextId = 1;
        }

        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        public AssetId GetId(IIdentifiedAsset obj)
        {
            AssetId current = obj.Id;
            if (current.group == GroupId && current.id != 0)
            {
                if (!_objId.TryGetValue(current.id, out IIdentifiedAsset savedObj) || savedObj == null)
                {
                    _objId[obj] = current.id;
                    return current;
                }

                if (savedObj == obj)
                    return current;

                DateTime savedObjDate = GetCreationTimeSafe(savedObj);
                DateTime currentObj = GetCreationTimeSafe(obj);

                if (savedObjDate > currentObj)
                {
                    _objId[obj] = current.id;
                    savedObj.RequestID();
                    return current;
                }
            }

            return new AssetId(GroupId, _objId[obj] = GetNew());
        }

        private DateTime GetCreationTimeSafe(IIdentifiedAsset file)
        {
            try
            {
                return File.GetCreationTime(AssetDatabase.GetAssetPath((Object)file));
            }
            catch (ArgumentException)
            {
                return DateTime.Now;
            }
        }

        private uint GetNew()
        {
            while (_objId.ContainsKey(_nextId))
            {
                _nextId++;
            }

            dirty = true;
            return _nextId;
        }

        public void UpdateId()
        {
            _nextId = _objId.Values.Max();
        }
    }
}
#endif