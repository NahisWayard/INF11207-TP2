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

        static void Main(string[] args)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ","
            };

            using (var reader = new StreamReader("D:/UQAR/OOP/TP2/data/tennis.csv"))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<TennisDecision>();
                var tree = new Tree<TennisDecision>(records.ToList(), "jouer");

                var toClassificate = new TennisDecision("ensoleilé", "douce", "haute", "oui", null);
                tree.classificateElement(toClassificate);
                Console.WriteLine(toClassificate);
            }
        }
    }
}
