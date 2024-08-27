using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentC_.BusinessLayer
{
    public class Eliminations
    {
        public string Team1 { get; set; }
        public string Team2 { get; set; }
        public string Result { get; set; }
    }
    public class EliminationSemiFinal
    {
        public Teams Team { get; set; }
        public string LotteryGroup { get; set; }
    }
    public class MedalsRanking
    {
        public Teams Team { get; set; }
        public MedalTypes Medal { get; set; }
    }
    public enum MedalTypes
    {
        Gold,
        Silver,
        Bronze
    }
}
