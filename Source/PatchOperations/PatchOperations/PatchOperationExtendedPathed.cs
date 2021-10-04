namespace XmlExtensions
{
    public abstract class PatchOperationExtendedPathed : PatchOperationExtended
    {
        public string xpath;

        protected override void SetException()
        {
            exceptionVals = new string[] { xpath };
            exceptionFields = new string[] { "xpath" };
        }
    }
}