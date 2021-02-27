using System;
using System.Collections.Generic;
using System.Text;

namespace TP2.Tree
{
    class Tree<T>
    {
        private Node<T> rootNode { get; set; }
        public string className { get; set; }

        public Tree(List<T> records, string className)
        {
            this.className = className;
            this.rootNode = new Node<T>(records, new List<string>{ className }, className);
        }

        private void classify(T element, Node<T> node)
        {

            if (node.subNodes.Count == 0)
            {
                element.GetType().GetProperty(className).SetValue(element, node.getChoice());
                return;
            }
            var attrValue = (string)Program.getProperty(element, node.selectedAttribute);
            foreach (var n in node.subNodes)
            {
                if (n.Key == attrValue)
                {
                    classify(element, n.Value);
                }
            }
        }

        public void classificateElement(T element)
        {
            classify(element, rootNode);
        }
    }
}
