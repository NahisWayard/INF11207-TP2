using System;
using System.Collections.Generic;
using System.Text;

namespace TP2
{
    class TennisDecision
    {
        public string temps { get; set; }
        public string température{ get; set; }
        public string humidité { get; set; }
        public string vent { get; set; }
        public string jouer{ get; set; }

        public TennisDecision(string temps, string température, string humidité, string vent, string jouer)
        {
            this.temps = temps;
            this.température = température;
            this.humidité = humidité;
            this.vent = vent;
            this.jouer = jouer;
        }

        public override string ToString()
        {
            return $"jouer: {jouer}, température: {température}, humidité: {humidité}, vent: {vent}, temps: {temps}";
        }
    }
}
