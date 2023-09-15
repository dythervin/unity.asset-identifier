using System;
using System.Collections.Generic;
using System.Linq;
using Dythervin.Addressables;
using Dythervin.Collections;
using Dythervin.Core.Extensions;

namespace Dythervin.AssetIdentifier.Addressables
{
    public static class AssetIdentifiedDatabaseExtensions
    {
        private static IAssetIdentifiedDatabase _identifiedDatabase;

        private static IAssetIdentifiedDatabase IdentifiedDatabase =>
            _identifiedDatabase ?? AssetIdentifiedDatabase.Instance;

        public static AssetReferenceExt GetRef<TAssetId>(this TAssetId assetId)
            where TAssetId : IAssetId
        {
            return IdentifiedDatabase.GetRef(assetId);
        }

        public static bool TryGetRef<TAssetId>(this TAssetId assetId, out AssetReferenceExt assetReference)
            where TAssetId : IAssetId
        {
            return IdentifiedDatabase.TryGetRef(assetId, out assetReference);
        }

        public static T Load<T>(this AssetId assetId)
            where T : class
        {
            return IdentifiedDatabase.Load<T>(assetId);
        }

        public static T Load<T>(this AssetIdRef<T> assetId)
            where T : class
        {
            return IdentifiedDatabase.Load<T>(assetId.value);
        }

        public static bool TryGet<T>(this AssetId assetId, out T asset)
            where T : class
        {
            return IdentifiedDatabase.TryLoad(assetId, out asset);
        }

        public static IEnumerable<AssetReferenceExt> GetReferencesEditor<T>(this IAssetIdentifiedDatabase database)
            where T : class
        {
#if UNITY_EDITOR
            return database.AllReferences.Where(x => x.editorAsset is T);
#else
            return null;
#endif
        }

        public static IEnumerable<AssetReferenceExt> GetReferencesEditor(this IAssetIdentifiedDatabase database, Type type)
        {
#if UNITY_EDITOR
            return database.AllReferences.Where(x => x.editorAsset != null && x.editorAsset.GetType().Implements(type));
#else
            return null;
#endif
        }

        public static ICollection<AssetReferenceExt> GetReferencesEditor<T>(this IAssetIdentifiedDatabase database,
            List<AssetReferenceExt> assetReferences)
            where T : class
        {
            assetReferences.Clear();
#if UNITY_EDITOR
            foreach (AssetReferenceExt assetReference in database.AllReferences.ToEnumerable())
            {
                if (assetReference.editorAsset is T)
                    assetReferences.Add(assetReference);
            }

#endif
            return assetReferences;
        }

        public static ICollection<AssetReferenceExt> GetReferencesEditor(this IAssetIdentifiedDatabase database, Type type,
            ICollection<AssetReferenceExt> assetReferences)
        {
            assetReferences.Clear();
#if UNITY_EDITOR
            foreach (AssetReferenceExt assetReference in database.AllReferences.ToEnumerable())
            {
                if (assetReference.editorAsset != null && assetReference.editorAsset.GetType().Implements(type))
                    assetReferences.Add(assetReference);
            }
#endif

            return assetReferences;
        }

        public static IEnumerable<AssetId> GetIdsEditor<T>(this IAssetIdentifiedDatabase database)
            where T : class
        {
#if UNITY_EDITOR
            return database.AllIds.Where(x => x.GetRef().editorAsset is T);
#else
            return null;
#endif
        }
    }
}