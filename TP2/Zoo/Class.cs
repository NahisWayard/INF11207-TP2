using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TP2.Zoo
{
    class Class
    {
        [Name("Class_Number")]
        public int classNumber { get; set; }
        [Name("Number_Of_Animal_Species_In_Class")]
        public int countOfAnimals { get; set; }
        [Name("Class_Type")]
        public string className { get; set; }
        [Name("Animal_Names")]
        public string animalNames { private get; set; }
        public string[] getAnimalNames()
        {
            return animalNames.Split(',');
        }

        public override string ToString()
        {
            return $"{className}";
        }
    }
}
