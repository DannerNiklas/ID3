﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ID3Baum
{
    internal class Node
    {
        public int AttrIdx { get; set; }
        public List<Node> Children { get; set; }
        public Type Result { get; set; }

        public Node(int attrIdx, Type result )
        {
            AttrIdx = attrIdx;
            Result = result;
            Children = new();
        }

        public Enum Evaluate(Enum[] data)
        {
            return data[0];
        }

    }
}
