// using System;
// using System.Collections.Generic;
// using System.Diagnostics.CodeAnalysis;
// using Sirenix.OdinInspector;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
//
// namespace Dythervin.AssetIdentifier.Addressables
// {
//     public partial class AssetIdentifiedDatabase
//     {
//         private class GroupData : ISerializationCallbackReceiver
//         {
//             [SerializeField]
//             private string[] labels = Array.Empty<string>();
//
//             [SerializeField]
// #if ODIN_INSPECTOR
//             [ReadOnly]
// #endif
//             private AssetId[] ids = Array.Empty<AssetId>();
//
//             [SerializeField]
// #if ODIN_INSPECTOR
//             [ReadOnly]
// #endif
//             private List<AssetReference> values;
//
//             private readonly Dictionary<AssetId, AssetReference> _values =
//                 new Dictionary<AssetId, AssetReference>();
//
//             public IReadOnlyDictionary<AssetId, AssetReference> Dictionary => _values;
//
//             public IReadOnlyList<AssetId> Ids => ids;
//
//             public IReadOnlyList<AssetReference> Values => values;
//
//             public AssetReference this[AssetId id] => _values[id];
//
//             public bool TryGetValue(AssetId id, out AssetReference assetReferenceT)
//             {
//                 return _values.TryGetValue(id, out assetReferenceT);
//             }
//
//             void ISerializationCallbackReceiver.OnAfterDeserialize()
//             {
//                 _values.Clear();
// #if UNITY_2021_3_OR_NEWER
//                 _values.EnsureCapacity(ids.Length);
// #endif
//
//                 for (int i = ids.Length - 1; i >= 0; i--)
//                 {
//                     _values.Add(ids[i], values[i]);
//                 }
//
// #if !UNITY_EDITOR
//             ids = null;
//             values = null;
//             labels = null;
// #endif
//             }
//
//             void ISerializationCallbackReceiver.OnBeforeSerialize()
//             {
//             }
//
//             [SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
//             public void SetIds(IReadOnlyCollection<TValue> objects)
//             {
// #if UNITY_EDITOR
//                 var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
//
//                 if (ids.Length < objects.Count)
//                     ids = new AssetId[objects.Count];
//
//                 if (values.Length < objects.Count)
//                     values = new AssetReference[objects.Count];
//
//                 var group = settings.DefaultGroup;
//                 // if (group == null)
//                 //     settings.CreateGroup(groupName, false, false, false, settings.DefaultGroup.Schemas, settings.DefaultGroup. );
//
//                 int i = 0;
//                 foreach (TValue equipmentData in objects)
//                 {
//                     string path = AssetDatabase.GetAssetPath(equipmentData);
//                     string guid = AssetDatabase.AssetPathToGUID(path);
//
//                     ids[i] = equipmentData.Id;
//                     var entry = settings.CreateOrMoveEntry(guid, group, true, false);
//                     foreach (string label in labels)
//                     {
//                         entry.labels.Add(label);
//                     }
//
//                     entry.address = path;
//                     ref var assetRef = ref values[i];
//                     if (assetRef == null)
//                         assetRef = new AssetReference(guid);
//                     else
//                         assetRef.SetEditorAsset(equipmentData);
//
//                     i++;
//                 }
// #endif
//             }
//
//             public IEnumerator<AssetId> GetEnumerator()
//             {
//                 return (IEnumerator<AssetId>)ids.GetEnumerator();
//             }
//
//             IEnumerator IEnumerable.GetEnumerator()
//             {
//                 return GetEnumerator();
//             }
//
//             public int Count => ids.Length;
//
//             public AssetId this[int index] => ids[index];
//         }
//     }
// }