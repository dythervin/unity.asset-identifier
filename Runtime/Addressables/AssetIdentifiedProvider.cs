using System;
using System.Collections.Generic;
using Dythervin.Addressables;
using Dythervin.ObjectPool;
using Dythervin.ObjectSelector;
using Dythervin.ObjectSelector.Editor;

namespace Dythervin.AssetIdentifier.Addressables
{
    public class AssetIdentifiedProvider : IObjProvider
    {
        private readonly Type _type;

        private List<FoundObject> _hashset;

        public AssetIdentifiedProvider(Type type)
        {
            _type = type;
            _hashset = ListPools<FoundObject>.Shared.Get();
        }

        ~AssetIdentifiedProvider()
        {
            ListPools<FoundObject>.Shared.Release(ref _hashset);
        }

        public IEnumerable<FoundObject> Get()
        {
            using (ListPools<AssetReferenceExt>.Shared.Get(out var list))
            {
                _hashset.Clear();
                foreach (var assetReference in AssetIdentifiedDatabase.Instance.GetReferencesEditor(_type, list))
                {
                    _hashset.Add(new FoundObject(assetReference.editorAsset));
                }

                return _hashset;
            }
        }
    }
}