namespace XmlExtensions
{
    internal class ObjectContainer
    {
        public object value;
        public ObjectContainer parent;
        public string objPath = "";

        public ObjectContainer(object value)
        {
            this.value = value;
        }

        public ObjectContainer(object value, ObjectContainer parent)
        {
            this.value = value;
            this.parent = parent;
        }
    }
}
