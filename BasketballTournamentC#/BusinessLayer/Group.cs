using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasketballTournamentC_.BusinessLayer
{
    public class Groups
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
        public Dictionary<string,string> HeadToHeadResult { get; set; }= new Dictionary<string,string>();
    }
    public class Leadboard
    {
        public List<Groups> Groups { get; set; }= new List<Groups>();
    }
    

}
