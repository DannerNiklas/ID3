using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3Baum
{
    public class Node<T>
    {
        public Type AttrType { get; set; }
        public List<(string, Node<T>)> Children { get; set; }
        public T? Result { get; set; }

        public Node(Type attrType, T? result)
        {
            AttrType = attrType;
            Result = result;
            Children = new();
        }

        public Node()
        {
            Children = new();
        }

        public T Evaluate(Enum[] data)
        {
            for (int i = 0; i < length; i++)
            {

            }
            return default(T);
        }

    }
}
