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
        private string code;

        /// <summary>
        /// 账号类别
        /// </summary>
        private string category;

        /// <summary>
        /// 股东姓名
        /// </summary>
        private string name;

        public string Category { get => category; set => category = value; }
        public string Name { get => name; set => name = value; }
        public string Code { get => code; set => code = value; }
    }
}