namespace Dythervin.AssetIdentifier
{
    public interface IAssetId
    {
        ushort Group { get; }

        uint Id { get; }
    }

    public static class AssetIdExtensions
    {
        public static bool IsDefault<TAssetId>(this TAssetId assetId)
            where TAssetId : IAssetId
        {
            return assetId.Group == 0 || assetId.Id == 0;
        }
    }
}