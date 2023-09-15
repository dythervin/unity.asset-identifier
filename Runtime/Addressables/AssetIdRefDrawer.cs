#if UNITY_EDITOR
using System;
using Dythervin.Core.Extensions;
using Dythervin.ObjectSelector;
using Dythervin.ObjectSelector.Editor;
using UnityEditor;
using UnityEngine;

namespace Dythervin.AssetIdentifier.Addressables
{
    [CustomPropertyDrawer(typeof(AssetIdRef<>))]
    internal class AssetIdRefDrawer : PropertyDrawer
    {
        private AssetIdRefSelectorField _fieldDrawer;

        private bool _isValidField;

        private Type _refType;
        

        private void Initialize(SerializedProperty property)
        {
            if (_fieldDrawer != null)
                return;

            _refType = fieldInfo.FieldType.GenericTypeArguments[0];

            _isValidField = _refType != null;
            FieldConfig config = new()
            {
                filter = null,
                fieldLabel = _refType.GetNiceName(),
                provider = new AssetIdentifiedProvider(_refType),
                popUpType = PopUpType.Dropdown
            };

            _fieldDrawer = new AssetIdRefSelectorField(property, _refType, config);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 20;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize(property);
            property.Next(true);

            if (!_isValidField)
            {
                EditorGUI.PropertyField(position, property, label, true);
                return;
            }

            _fieldDrawer.OnGUI(position, label);
        }
    }
}
#endif