using System;
using System.Collections.Generic;
using System.Text;

namespace TP2.Tree
{
    class Tree<T, U> where U : IEquatable<U>
    {
        private Node<T, U> rootNode { get; set; }
        public string className { get; set; }

        public Tree(List<T> records, string className, List<string> preIgnoredAttributes = null)
        {
            if (preIgnoredAttributes == null)
                preIgnoredAttributes = new List<string> { className };
            else
                preIgnoredAttributes.Add(className);
            this.className = className;
            this.rootNode = new Node<T, U>(records, preIgnoredAttributes, className);
        }

        private void classify(T element, Node<T, U> node)
        {

            if (node.subNodes.Count == 0)
            {
                element.GetType().GetProperty(className).SetValue(element, node.getChoice());
                return;
            }
            var attrValue = (U)Program.getProperty(element, node.selectedAttribute);
            foreach (var n in node.subNodes)
            {
                if (n.Key.Equals(attrValue))
                {
                    classify(element, n.Value);
                }
            }
        }

        public void classificateElement(T element)
        {
            classify(element, rootNode);
        }

        public void display()
        {
            this.rootNode.display(0);
        }
    }
}
