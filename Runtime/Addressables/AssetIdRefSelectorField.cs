#if UNITY_EDITOR
using System;
using Dythervin.Addressables;
using Dythervin.ObjectSelector.Editor;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Dythervin.AssetIdentifier.Addressables
{
    internal class AssetIdRefSelectorField : SelectorFieldBase
    {
        private static readonly string TypeName =
            typeof(AssetIdRef<>).Name.Substring(0, typeof(AssetIdRef<>).Name.Length - 2);

        public AssetIdRefSelectorField(SerializedProperty property, Type type, in FieldConfig config) : base(property,
            type,
            in config)
        {
        }

        protected override bool IsValid(SerializedProperty serializedProperty, Type type, in FieldConfig config)
        {
            return serializedProperty.propertyType == SerializedPropertyType.Generic &&
                   serializedProperty.type.Contains(TypeName);
        }

        protected override Object GetObjectReference()
        {
            AssetIdRef.TryGetRef(out AssetReferenceExt obj);
            return obj?.editorAsset;
        }

        private IAssetId AssetIdRef => (IAssetId)property.boxedValue;

        protected override void ChangeObject(Object obj)
        {
            property.boxedValue = obj is IIdentifiedAsset identifiedAsset ? identifiedAsset.Id : default;
        }
    }
}
#endif