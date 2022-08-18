using System;

namespace Dythervin.AssetIdentifier
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AssetGroupAttribute : Attribute
    {
        public readonly string name;

        public AssetGroupAttribute(string name)
        {
            this.name = name;
        }
    }
}