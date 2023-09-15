using System.Collections.Generic;
using Dythervin.Addressables;

namespace Dythervin.AssetIdentifier.Addressables
{
    public interface IAssetIdentifiedDatabase
    {
        AssetReferenceExt GetRef<TAssetId>(TAssetId assetId)
            where TAssetId : IAssetId;

        bool TryGetRef<TAssetId>(TAssetId assetId, out AssetReferenceExt assetReference)
            where TAssetId : IAssetId;

        T Load<T>(AssetId assetId)
            where T : class;

        bool TryLoad<T>(AssetId assetId, out T asset)
            where T : class;

#if UNITY_EDITOR
        /// <summary>
        /// Editor only
        /// </summary>
        void Refresh();

        /// <summary>
        /// Editor only
        /// </summary>
        void AssetsDirty();
#endif
        IReadOnlyList<AssetId> AllIds { get; }

        IReadOnlyList<AssetReferenceExt> AllReferences { get; }
    }
}