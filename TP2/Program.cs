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

                tree.display();
                var toClassificate = new TennisDecision("ensoleilé", "douce", "haute", "oui", null);
                tree.classificateElement(toClassificate);
                Console.WriteLine(toClassificate);
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
            var toClassificate = new Zoo.ClassifiedAnimal("strange animal", 1, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1);
            tree.classificateElement(toClassificate);
            toClassificate.className = animals.First(x => x.classNumber == toClassificate.classNumber).className;
            Console.WriteLine(toClassificate);
        }

        static void Main(string[] args)
        {
            try
            {
                switch (args.Length)
                {
                    case 1:
                        TennisDemo(args[0]);
                        break;
                    case 3:
                        var animals = FormatZoo(args[0], args[1], args[2]);
                        ZooDemo(animals);
                        break;
                    default:
                        Console.WriteLine("Usage:");
                        Console.WriteLine("./TP2 pathToTennisCSV");
                        Console.WriteLine("./TP2 pathToZooCSV pathToClassCSV pathOutputCSV");
                        break;
                }
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine(e.Message, Console.Error);
                Environment.Exit(1);
            }
        }
    }
}
