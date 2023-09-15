namespace Dythervin.AssetIdentifier
{
    [System.Serializable]
    public struct AssetIdRef<T>
        where T : class
    {
        public AssetId value;

        public AssetIdRef(AssetId value)
        {
            this.value = value;
        }

        public static implicit operator AssetIdRef<T>(AssetId value)
        {
            return new AssetIdRef<T>(value);
        }

        public static implicit operator AssetId(AssetIdRef<T> value)
        {
            return value.value;
        }
    }
}