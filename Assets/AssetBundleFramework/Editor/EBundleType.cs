
namespace AssetBundleFramework.Editor
{
    /// <summary>
    /// 控制AB粒度
    /// </summary>
    public enum EBundleType
    {
        /// <summary>
        /// 以文件作为ab名字(最小粒度)
        /// </summary>
        File,
        /// <summary>
        /// 以目录作为ab的名字
        /// </summary>
        Directory,
        /// <summary>
        /// 以上所有的
        /// </summary>
        All
    }
}
