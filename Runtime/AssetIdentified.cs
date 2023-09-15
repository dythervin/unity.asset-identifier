#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace Dythervin.AssetIdentifier
{
    public abstract class AssetIdentified : ScriptableObject, IIdentifiedAsset
    {
#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField]
        private AssetId id;

        public AssetId Id => id;

        void IIdentifiedAsset.SetID(AssetId value)
        {
            id = value;
        }

        protected virtual void Awake()
        {
            this.RequestID();
        }

        protected virtual void OnEnable()
        {
            this.RequestID();
        }
    }
}