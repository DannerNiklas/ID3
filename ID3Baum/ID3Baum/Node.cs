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
        public List<(Enum, Node<T>)> Children { get; set; }
        public T? Result { get; set; } //Not properly nullable :/. Default value is (enum)0. --> E.g. "Yes" if it's the zero value of the enum. 

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
            //If node has a result it's childcount is 0 
            //Zao sheng hao jung wo shenzai wo bing chilling wohan shiwey bing chilling - Zhong Xina
            //Console.WriteLine("Child Kaunt: " + Children.Count);
            if(Children.Count == 0)
                return Result;
            
            foreach (var childNode in Children)
            {
                if(data.Contains(childNode.Item1))
                {
                    var tempData = data.ToList();
                    tempData.Remove(childNode.Item1);
                    return childNode.Item2.Evaluate(tempData.ToArray());
                }
            }
            throw new DataMisalignedException();
        }

    }
}
