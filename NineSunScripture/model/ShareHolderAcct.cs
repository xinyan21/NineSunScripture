namespace NineSunScripture.model
{
    /// <summary>
    /// 股东账号实体类
    /// </summary>
    public class ShareHolderAcct : BaseModel
    {
        /// <summary>
        /// 股东代码
        /// </summary>
        public string code;

        /// <summary>
        /// 账号类别
        /// </summary>
        public string category;

        /// <summary>
        /// 股东姓名
        /// </summary>
        public string name;
    }
}