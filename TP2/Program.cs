using CsvHelper;
using System.Linq;
using System.Collections.Generic;
using CsvHelper.Configuration;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using TP2.Tree;

namespace TP2
{
    class Program
    {
        static public object getProperty(object o, string prop)
        {
            return o.GetType().GetProperty(prop).GetValue(o, null);
        }

        static void TennisDemo(string filepath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ","
            };

            using (var reader = new StreamReader(filepath))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<TennisDecision>();
                var tree = new Tree<TennisDecision, string>(records.ToList(), "jouer");

                var toClassificate = new TennisDecision("ensoleilé", "douce", "haute", "oui", null);
                tree.classificateElement(toClassificate);
                Console.WriteLine(toClassificate);
                tree.display();
            }
        }

        static List<Zoo.ClassifiedAnimal> FormatZoo(string zooFilepath, string classFilepath, string outputPath)
        {
            var classifiedAnimals = new List<Zoo.ClassifiedAnimal> { };

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ","
            };

            using (var classReader = new StreamReader(classFilepath))
            using (var classCSV = new CsvReader(classReader, config))
            {
                var classes = classCSV.GetRecords<Zoo.Class>().ToList();

                using (var zooReader = new StreamReader(zooFilepath))
                using (var zooCSV = new CsvReader(zooReader, config))
                {
                    var animals = zooCSV.GetRecords<Zoo.Animal>().ToList();

                    foreach (var a in animals)
                    {
                        var ca = new Zoo.ClassifiedAnimal(a, classes);
                        classifiedAnimals.Add(ca);
                    }

                    using (var zooWriter = new StreamWriter(outputPath))
                    using (var outputCSV = new CsvWriter(zooWriter, config))
                    {
                        outputCSV.WriteRecords(classifiedAnimals);
                        return classifiedAnimals;
                    }
                }
            }
        }

        static void ZooDemo(List<Zoo.ClassifiedAnimal> animals)
        {
            var tree = new Tree<Zoo.ClassifiedAnimal, int>(animals, "classNumber", new List<string> { "name", "className", "animalNames" });
            tree.display();
        }

        static void Main(string[] args)
        {
            TennisDemo("D:/UQAR/OOP/TP2/data/tennis.csv");
            Console.WriteLine(new String('-', Console.WindowWidth));
            var animals = FormatZoo("D:/UQAR/OOP/TP2/data/zoo.csv", "D:/UQAR/OOP/TP2/data/class.csv", "D:/UQAR/OOP/TP2/data/formatedZoo.csv");
            ZooDemo(animals);
        }
    }
}
