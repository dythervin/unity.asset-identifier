using System;
using System.Collections.Generic;
using System.Linq;
using Dythervin.Addressables;
using Dythervin.Collections;
using Dythervin.Core;
using Dythervin.Core.Extensions;
using UnityEngine;

namespace Dythervin.AssetIdentifier.Addressables
{
    public partial class AssetIdentifiedDatabase : SingletonAsset<IAssetIdentifiedDatabase, AssetIdentifiedDatabase>,
        IAssetIdentifiedDatabase
    {
        [SerializeField] protected SerializedDictionary<ushort, SerializedDictionary<uint, AssetReferenceExt>> map;

        private AssetId[] _allIds;

        private AssetReferenceExt[] _allReferences;

        public IReadOnlyList<AssetId> AllIds =>
            _allIds ??= Map.SelectMany(pair => pair.Value.Keys.Select(id => new AssetId(pair.Key, id))).ToArray();

        public IReadOnlyList<AssetReferenceExt> AllReferences
        {
            get { return _allReferences ??= Map.SelectMany(pair => pair.Value.Values).ToArray(); }
        }

        private IReadOnlyDictionary<ushort, SerializedDictionary<uint, AssetReferenceExt>> Map
        {
            get
            {
                RefreshIfNeeded();
                return map;
            }
        }

        public AssetReferenceExt GetRef<TAssetId>(TAssetId assetId)
            where TAssetId : IAssetId
        {
            if (assetId.IsDefault())
                throw new NullReferenceException($"{nameof(TAssetId)} not set");

            return Map[assetId.Group][assetId.Id];
        }

        protected override void OnEnabled()
        {
            base.OnEnabled();
        }

        public bool TryGetRef<TAssetId>(TAssetId assetId, out AssetReferenceExt assetReference)
            where TAssetId : IAssetId
        {
            if (Map.TryGetValue(assetId.Group, out var group))
            {
                if (group.TryGetValue(assetId.Id, out AssetReferenceExt wrapper))
                {
                    assetReference = wrapper;
                    return true;
                }
            }

            assetReference = null;
            return false;
        }

        private void ResetAll()
        {
            _allIds = null;
            _allReferences = null;
            map?.Clear();
            this.Dirty();
        }

        partial void RefreshIfNeeded();

        public T Load<T>(AssetId assetId)
            where T : class
        {
            AssetReferenceExt assetReference = GetRef(assetId);

            T obj = assetReference.Load<T>();
            return obj;
        }

        public bool TryLoad<T>(AssetId assetId, out T asset)
            where T : class
        {
            if (TryGetRef(assetId, out AssetReferenceExt assetReference))
            {
                asset = assetReference.Load<T>();
                return true;
            }

            asset = null;
            return false;
        }
    }
}