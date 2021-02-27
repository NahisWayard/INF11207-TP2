using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TP2.Tree
{
    class Node<T>
    {
        private string classifierAttribute;
        private Dictionary<string, double> pClasses = new Dictionary<string, double>{ };
        private Dictionary<string, Dictionary<string, Tuple<int, double>>> countOfValuePerAttr = new Dictionary<string, Dictionary<string, Tuple<int, double>>> { };
        private double datasetEntropy = 0;
        private List<T> elements = new List<T> { };
        public string selectedAttribute { get; private set; } = "";
        private List<string> filteredAttributes = new List<string> { };
        private Dictionary<string, double> gainMap = new Dictionary<string, double> { };
        public Dictionary<string, Node<T>> subNodes { get; private set; } = new Dictionary<string, Node<T>> { };

        public Node(List<T> elements, List<string> filteredAttributes, string classifierAttribute)
        {
            this.elements = elements;
            this.filteredAttributes = filteredAttributes;
            this.classifierAttribute = classifierAttribute;
            
            mapAttributes();
            calcProbabilityClass();

            if (datasetEntropy == 0)
            {
                foreach (var s in filteredAttributes)
                {
                    Console.WriteLine($"{s} -> {Program.getProperty(elements.First(), s)} ");
                }

                foreach (var e in elements)
                {
                    Console.WriteLine($"{e}");
                }
                Console.WriteLine();
                return;
            }

            calcInformationGain();
            makeSubNodes();
        }

        public string getChoice()
        {
            if (subNodes.Count == 0)
                return (string)Program.getProperty(elements.First(), classifierAttribute);
            throw new Exception("Node is not a terminaison node, can't make a decision");
        }

        private void makeSubNodes()
        {
            var newFiltered = new List<string>(filteredAttributes);
            newFiltered.Add(selectedAttribute);
            foreach(var klass in countOfValuePerAttr[selectedAttribute])
            {
                var subNodeElems = elements.Where(x => (string)Program.getProperty(x, selectedAttribute) == klass.Key).ToList();

                subNodes.Add(klass.Key, new Node<T>(subNodeElems, newFiltered, classifierAttribute));
            }
        }

        private void calcInformationGain()
        {
            foreach(var attr in countOfValuePerAttr)
            {
                if (filteredAttributes.Contains(attr.Key))
                    continue;
                double IG = datasetEntropy;
                foreach (var se in attr.Value.ToList())
                {
                    double seEntropy = 0;
                    double seProb = (double)se.Value.Item1 / (double)elements.Count;

                    foreach(var klass in pClasses)
                    {
                        double pSEPerClass = (double) elements.Count(x => (string) Program.getProperty(x, classifierAttribute) == klass.Key && (string) Program.getProperty(x, attr.Key) == se.Key) / (double) se.Value.Item1;
                        seEntropy -= pSEPerClass * Math.Log2(pSEPerClass);
                    }
                    if (double.IsNaN(seEntropy))
                        seEntropy = 0;
                    IG -= seProb * seEntropy;
                    countOfValuePerAttr[attr.Key][se.Key] = new Tuple<int, double>(countOfValuePerAttr[attr.Key][se.Key].Item1, seEntropy);

                }
                gainMap.Add(attr.Key, IG);
            }
            selectedAttribute = gainMap.FirstOrDefault(x => x.Value == gainMap.Values.Max()).Key;
        }

        private void mapAttributes()
        {
            foreach (var r in elements)
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (!countOfValuePerAttr.ContainsKey(property.Name))
                    {
                        countOfValuePerAttr.Add(property.Name, new Dictionary<string, Tuple<int, double>> { });
                    }

                    if (!countOfValuePerAttr[property.Name].ContainsKey((string)property.GetValue(r)))
                    {
                        countOfValuePerAttr[property.Name].Add((string)property.GetValue(r), new Tuple<int, double>(1, 0));
                    }
                    else
                    {
                        var oldTuple = countOfValuePerAttr[property.Name][(string)property.GetValue(r)];

                        countOfValuePerAttr[property.Name][(string)property.GetValue(r)] = new Tuple<int, double>(oldTuple.Item1 + 1, oldTuple.Item2);
                    }
                }
            }
        }

        private void calcProbabilityClass()
        {
            foreach(var klass in countOfValuePerAttr[classifierAttribute])
            {
                pClasses.Add(klass.Key, (double) klass.Value.Item1 / (double) elements.Count);
            }
            foreach (var p in pClasses)
                datasetEntropy -= p.Value * Math.Log2(p.Value);
        }
    }
}
