using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace TP2.Zoo
{
    interface IAnimal
    {
        [Name("animal_name")]
        public string name { get; set; }
        public int hair { get; set; }
        public int feathers { get; set; }
        public int eggs { get; set; }
        public int milk { get; set; }
        public int airborne { get; set; }
        public int aquatic { get; set; }
        public int predator { get; set; }
        public int toothed { get; set; }
        public int backbone { get; set; }
        public int breathes { get; set; }
        public int venomous { get; set; }
        public int fins { get; set; }
        public int legs { get; set; }
        public int tail { get; set; }
        public int domestic { get; set; }
        public int catsize { get; set; }
        [Name("class_type")]
        public int classNumber { get; set; }
    }
}
