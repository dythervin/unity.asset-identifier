namespace Dythervin.AssetIdentifier
{
    public interface IUnique
    {
        uint Id { get; }
#if UNITY_2021_3_OR_NEWER
        protected internal
 #endif
        void SetID(uint id);
    }
}