using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;

namespace Dythervin.AssetIdentifier
{
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    [Serializable]
    public struct AssetId : IEquatable<AssetId>, IAssetId
    {
#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        public ushort group;

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        public uint id;

        public AssetId(ushort group, uint id)
        {
            this.group = group;
            this.id = id;
        }

        public static IEqualityComparer<AssetId> GroupIDComparer { get; } = new GroupIDEqualityComparer();

        public override bool Equals(object obj)
        {
            return obj is AssetId other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (group.GetHashCode() * 397) ^ (int)id;
            }
        }

        public bool Equals(AssetId other)
        {
            return group == other.group && id == other.id;
        }

        public static bool operator ==(AssetId left, AssetId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AssetId left, AssetId right)
        {
            return !left.Equals(right);
        }

        private sealed class GroupIDEqualityComparer : IEqualityComparer<AssetId>
        {
            public bool Equals(AssetId x, AssetId y)
            {
                return x.group == y.group && x.id == y.id;
            }

            public int GetHashCode(AssetId obj)
            {
                return HashCode.Combine(obj.group, obj.id);
            }
        }

        public ushort Group => group;

        public uint Id => id;
    }
}