using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using BasketballTournamentC_.BusinessLayer;
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
            Group group1=new Group();
            group1.Name = group.Key;
            group1.Teams = group.Value;
            leadboard.Groups.Add(group1);
            
            
        }
        foreach(var group in leadboard.Groups)
        {
            var ordered_group = group.Teams.OrderByDescending(y => y.Wins);
            Console.WriteLine($"Group:{group.Name}");
            Console.WriteLine("(Ime - pobede/porazi/bodovi/postignuti koševi/primljeni koševi/koš razlika)::");
            foreach (var team in ordered_group)
            {
               
                Console.WriteLine($"{team.Team}        {team.Wins} / {team.Losses} / {team.Points} / {team.PointsScored} / {team.PointsReceived} / {team.PointsDiff}");
            }
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
        GenerateScore(winner_team, losser_team);
    }
    public static void GenerateScore(Teams team1, Teams team2)
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



    }
}
    
