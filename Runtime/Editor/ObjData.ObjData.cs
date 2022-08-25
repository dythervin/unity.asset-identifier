#if UNITY_EDITOR
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Dythervin.Collections;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Dythervin.AssetIdentifier.Editor
{
    [Serializable]
    internal class ObjData
    {
        public uint id;
        private readonly DictionaryCross<IUnique, uint> _objId = new DictionaryCross<IUnique, uint>();
        public static bool dirty;

        public void ResetCounter()
        {
            _objId.Clear();
            id = 0;
            dirty = true;
        }

#if UNITY_EDITOR
        [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
        public uint GetId(IUnique obj)
        {
            uint current = obj.Id;

            if (!_objId.TryGetValue(current, out IUnique savedObj) || savedObj == null)
            {
                _objId[obj] = current;
                return current;
            }

            if (savedObj == obj)
                return current;

            DateTime savedObjDate = GetCreationTimeSafe(savedObj);
            DateTime currentObj = GetCreationTimeSafe(obj);

            if (savedObjDate > currentObj)
            {
                _objId[obj] = current;
                savedObj.RequestID();
                return current;
            }

            _objId[obj] = GetNew();
            return id;
        }
#endif

        private DateTime GetCreationTimeSafe(IUnique file)
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
            while (_objId.ContainsKey(id))
                id++;

            dirty = true;
            return id;
        }

        public void UpdateId()
        {
            id = _objId.Values.Max();
        }
    }
}
#endif