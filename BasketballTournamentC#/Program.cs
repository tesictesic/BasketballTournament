using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using BasketballTournamentC_.BusinessLayer;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
class Program
{
    static void Main(string[] args)
    {
        string group_json = File.ReadAllText("C:\\Users\\Djordje\\Desktop\\BasketballTournamentC#\\BasketballTournamentC#\\Data\\groups.json");
        var groups = JsonSerializer.Deserialize<Dictionary<string, List<Teams>>>(group_json);
        Leadboard leadboard = new Leadboard();
        foreach (var group in groups)
        {
            var team = group.Value;
            for (int i = 0; i < team.Count; i++)
            {
                for (int j = i + 1; j < team.Count; j++)
                {
                    Teams team1 = team[i];
                    Teams team2 = team[j];
                    SimulateGroup(team1, team2);
                }
            }
            Groups group1=new Groups();
            group1.Name = group.Key;
            group1.Teams = group.Value;
            leadboard.Groups.Add(group1);
            
            
        }
        List<Teams> all_Teams=new List<Teams>();
        foreach(var group in leadboard.Groups)
        {
            List<Teams> ordered_group = RankTeamsInGroup(group.Teams);
            Console.WriteLine($"Group:{group.Name}");
            Console.WriteLine("(Ime - pobede/porazi/bodovi/postignuti koševi/primljeni koševi/koš razlika)::");
            
            foreach (var team in ordered_group)
            {
               
                Console.WriteLine($"{team.Team}        {team.Wins} / {team.Losses} / {team.Points} / {team.PointsScored} / {team.PointsReceived} / {team.PointsDiff}");
                all_Teams.Add(team);
            }
        }
        List<Teams> sorted_teams=RankTeamsInGroup(all_Teams);
        List<Teams> finally_teams = sorted_teams.GetRange(0, 9);
        Console.WriteLine("Najbolji timovi Timovi:");
        int k = 0;
        Console.WriteLine("(Ime - pobede/porazi/bodovi/postignuti koševi/primljeni koševi/koš razlika)::");
        string output = "";
        List<Teams>teams_for_lottery = new List<Teams>();
        foreach (var team in finally_teams)
        {
            k++;
            if (k == 9)
            {
                output = $"{k}. {team.Team}        {team.Wins} / {team.Losses} / {team.Points} / {team.PointsScored} / {team.PointsReceived} / {team.PointsDiff}      Ne nastavlja takmicenje";
            }
            else
            {
                output = $"{k}. {team.Team}        {team.Wins} / {team.Losses} / {team.Points} / {team.PointsScored} / {team.PointsReceived} / {team.PointsDiff}";
                teams_for_lottery.Add(team);
            }
            
            Console.WriteLine(output);
        }
        
        
        Dictionary<string,List<Teams>> lottery=new Dictionary<string,List<Teams>>();
        string[] sesiri = { "Sesir D", "Sesir E", "Sesir F", "Sesir G" };

        for (int i = 0; i < sesiri.Length; i++)
        {
            lottery[sesiri[i]] = new List<Teams>
            {
                teams_for_lottery[i * 2],
                        teams_for_lottery[i * 2 + 1]
            };
                    
        }
        foreach (var lot in lottery)
        {
            Console.WriteLine($"{lot.Key}:");
            var teams = lot.Value;
            foreach( var team in teams)
            {
                Console.WriteLine($"\t {team.Team}");
            }
        }
        List<(Teams,Teams)>eliminatios_phase=Lottering(lottery);
        List<Eliminations> eliminations=new List<Eliminations>();
        Console.WriteLine("Eliminaciona faza:");
        List<Teams> semi_finals_teams = new List<Teams>();
        foreach (var teams in eliminatios_phase)
        {
            string result = "";
            Console.WriteLine($"\t {teams.Item1.Team} - {teams.Item2.Team}");
            SimulateEliminationMatch(teams.Item1, teams.Item2,semi_finals_teams, out result);
            Eliminations elimination = new Eliminations
            {
                Team1 = teams.Item1.Team,
                Team2 = teams.Item2.Team,
                Result = result
            };
            eliminations.Add(elimination);
            
        }
        Console.WriteLine("Eliminaciona faza (Konacan rezultat):");
        foreach (var teams in eliminations)
        {
            Console.WriteLine($"\t {teams.Team1} - {teams.Team2} ({teams.Result})");
        }
       







    }
    public static void SimulateGroup(Teams team1, Teams team2)
    {
        Teams winner_team = null;
        Teams losser_team = null;
        int first_team_fiba_ranking = team1.FIBARanking;
        int second_team_fiba_ranking = team2.FIBARanking;
        if (first_team_fiba_ranking < second_team_fiba_ranking)
        {
            winner_team = team1;
            losser_team = team2;
        }
        else
        {
            winner_team = team2;
            losser_team = team1;
        }
        GenerateScoreInGroups(winner_team, losser_team);
    }
    public static void GenerateScoreInGroups(Teams team1, Teams team2)
    {

        Random random = new Random();
        int first_team_score = random.Next(70, 120);
        int second_team_score = random.Next(10, 69);
        int first_team_diff_points = first_team_score - second_team_score;
        int second_team_diff_points = second_team_score - first_team_score;
        team1.Wins++;
        team2.Losses++;
        team1.Points += 2;
        team2.Points += 1;
        team1.PointsScored += first_team_score;
        team2.PointsScored += second_team_score;
        team1.PointsReceived += second_team_score;
        team2.PointsReceived += first_team_score;
        team1.PointsDiff += first_team_diff_points;
        team2.PointsDiff += second_team_diff_points;
        team1.HeadToHeadResult.Add(team2.Team, $"{first_team_score}:{second_team_score}");
        team2.HeadToHeadResult.Add(team1.Team, $"{first_team_score}:{second_team_score}");


    }
    public static List<Teams> RankTeamsInGroup(List<Teams> teams)
    {
        teams.Sort((x, y) =>
        {
            int result = x.Points.CompareTo(y.Points)*-1;
            if (result == 0)
            {
                if (x.HeadToHeadResult.ContainsKey(y.Team))
                {
                    result = x.HeadToHeadResult[y.Team].CompareTo(y.HeadToHeadResult[x.Team]);
                }

                if (result == 0)
                {
                    
                    result = y.PointsDiff.CompareTo(x.PointsDiff);
                }
                if (result == 0)
                {
                   
                    result = y.PointsScored.CompareTo(x.PointsScored);
                }
            }

            return result;
        });
        return teams;
    }
    public static List<(Teams,Teams)>Lottering(Dictionary<string, List<Teams>> lottery)
    {
        List<(Teams, Teams)> pom_list=new List<(Teams, Teams)>();
        List<Teams> GroupD = lottery["Sesir D"];
        List<Teams> GroupE = lottery["Sesir E"];
        List<Teams> GroupF = lottery["Sesir F"];
        List<Teams> GroupG = lottery["Sesir G"];
        Shuffle(GroupD);
        Shuffle(GroupE);
        Shuffle(GroupF);
        Shuffle(GroupG);

        bool validPairsFound = false;
        while (!validPairsFound)
           
        {
            for (int i = 0; i < GroupD.Count; i++)
            {
                validPairsFound = true;
                Teams teamD = GroupD[i];
                Teams teamG = GroupG[i];

                Teams teamE = GroupE[i];
                Teams teamF = GroupF[i];



                if ((!teamD.HeadToHeadResult.Any(y => y.Key.Contains(teamG.Team))) && (!teamE.HeadToHeadResult.Any(y => y.Key.Contains(teamF.Team))))
                {
                    pom_list.Add((teamD, teamG));
                    pom_list.Add((teamE, teamF));
                    
                }
                else
                {
                    validPairsFound=false;
                    Shuffle(GroupD);
                    Shuffle(GroupE);
                    Shuffle(GroupF);
                    Shuffle(GroupG);
                    break;
                }
            }
        }
        return pom_list;
    }
    public static void Shuffle(List<Teams> list)
    {
        Random random = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            Teams value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

        public static void SimulateEliminationMatch(Teams team1,Teams team2,List<Teams> semi_finals, out string el)
    {
        Random random_result_for_teams=new Random();
        int team1_points = random_result_for_teams.Next(60, 120);
        int team2_points = random_result_for_teams.Next(60, 120);
        el = $"{team1_points}:{team2_points}";
        if (team1_points > team2_points)
        {
            semi_finals.Add(team1);
            
        }
        else
        {
            semi_finals.Add(team2);
        }
    }
}
    
