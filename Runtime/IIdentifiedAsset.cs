namespace Dythervin.AssetIdentifier
{
    public interface IIdentifiedAsset
    {
        AssetId Id { get; }
#if UNITY_2021_3_OR_NEWER
        protected internal
#endif
            void SetID(AssetId id);
    }
}