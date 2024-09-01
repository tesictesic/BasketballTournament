using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentC_.BusinessLayer
{
    public class ExhibitionResult
    {
        public string Name { get; set; }
        public List<Match> Matches { get; set; }

    }
    public class Match
    {
        public string Date { get; set; }
        public string Opponent { get; set; }
        public string Result { get; set; }
    }
}
