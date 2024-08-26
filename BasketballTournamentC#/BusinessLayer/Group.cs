using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentC_.BusinessLayer
{
    public class Group
    {
        public string Name { get; set; }
        public List<Teams> Teams { get; set; }
    }
    public class Teams
    {
        public string Team { get; set; }
        public string ISOCode { get; set; }
        public int FIBARanking { get; set; }
        public int Wins { get; set; } = 0;
        public int Losses { get; set; } = 0;
        public int Points { get; set; } = 0;
        public int PointsScored { get; set; } = 0;
        public int PointsReceived { get; set; } = 0;
        public int PointsDiff { get; set; } = 0;
    }
    public class Leadboard
    {
        public List<Group> Groups { get; set; }= new List<Group>();
    }
    public class LeadBordComparer : IComparer<Teams>
    {
        public int Compare(Teams x,Teams y)
        {
            return x.Wins.CompareTo(y.Wins);
        }
    }

}
