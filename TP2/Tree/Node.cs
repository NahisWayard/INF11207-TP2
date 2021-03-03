using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TP2.Tree
{
    class Node<T, U> where U : IEquatable<U>
    {
        private string classifierAttribute;
        private Dictionary<U, double> pClasses = new Dictionary<U, double>{ };
        private Dictionary<string, Dictionary<U, Tuple<int, double>>> countOfValuePerAttr = new Dictionary<string, Dictionary<U, Tuple<int, double>>> { };
        private double datasetEntropy = 0;
        private List<T> elements = new List<T> { };
        public string selectedAttribute { get; private set; } = "";
        private List<string> filteredAttributes = new List<string> { };
        private Dictionary<string, double> gainMap = new Dictionary<string, double> { };
        public Dictionary<U, Node<T, U>> subNodes { get; private set; } = new Dictionary<U, Node<T, U>> { };

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
                    try
                    {
                        Console.WriteLine($"{s} ->  {Program.getProperty(elements.First(), s)}");
                    } catch (System.ArgumentException e) {
                        Console.WriteLine($"{s} -> ?");
                    }
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

        public void display(int depth)
        {
            var padding = new String(' ', depth * 2);

            if (subNodes.Count == 0)
            {
                foreach (var e in elements)
                    Console.WriteLine($"{padding} - {e}");
            } else
            {
                Console.WriteLine($"{padding}Selected attribute {selectedAttribute}");
                Console.WriteLine($"{padding}Subnodes count {subNodes.Count}");
                Console.WriteLine($"{padding}Elements count {elements.Count}");
            }
            foreach (var n in subNodes)
            {
                Console.WriteLine($"{padding}{n.Key}:");
                n.Value.display(depth + 1);
            }
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
                var subNodeElems = elements.Where(x => klass.Key.Equals((U)Program.getProperty(x, selectedAttribute))).ToList();

                subNodes.Add(klass.Key, new Node<T, U>(subNodeElems, newFiltered, classifierAttribute));
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
                        double pSEPerClass = (double) elements.Count(x => klass.Key.Equals((U)Program.getProperty(x, classifierAttribute)) && se.Key.Equals((U) Program.getProperty(x, attr.Key))) / (double) se.Value.Item1;
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
                    if (property.Name != classifierAttribute && filteredAttributes.Contains(property.Name))
                        continue;
                    if (!countOfValuePerAttr.ContainsKey(property.Name))
                    {
                        countOfValuePerAttr.Add(property.Name, new Dictionary<U, Tuple<int, double>> { });
                    }
                    if (!countOfValuePerAttr[property.Name].ContainsKey((U)property.GetValue(r)))
                    {
                        countOfValuePerAttr[property.Name].Add((U)property.GetValue(r), new Tuple<int, double>(1, 0));
                    }
                    else
                    {
                        var oldTuple = countOfValuePerAttr[property.Name][(U)property.GetValue(r)];

                        countOfValuePerAttr[property.Name][(U)property.GetValue(r)] = new Tuple<int, double>(oldTuple.Item1 + 1, oldTuple.Item2);
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
