using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlExtensions
{
    internal class ObjectContainer
    {
        public object child;
        public object parent;

        public ObjectContainer(object child)
        {
            this.child = child;
        }

        public ObjectContainer(object child, object parent)
        {
            this.child = child;
            this.parent = parent;
        }
    }
}
