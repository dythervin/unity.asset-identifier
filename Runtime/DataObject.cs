#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace Dythervin.AssetIdentifier
{
    public abstract class DataObject : ScriptableObject, IUnique
    {
#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        [SerializeField]
        private uint id;

        public uint Id => id;

        void IUnique.SetID(uint value)
        {
            id = value;
        }

        protected virtual void OnEnable()
        {
            this.RequestID();
        }
    }
}