using BasketballTournamentC_.BusinessLayer;
using System.Text.Json;
class Program
{
    static void Main(string[] args)
    {
        string result = "";
        string group_json = File.ReadAllText("C:\\Users\\Djordje\\Desktop\\BasketballTournamentC#\\BasketballTournamentC#\\Data\\groups.json");
        var groups = JsonSerializer.Deserialize<Dictionary<string, List<Teams>>>(group_json);
        List<Dictionary<string,List<string>>> rounds= new List<Dictionary<string, List<string>>>();
        Leadboard leadboard = new Leadboard();
        foreach (var group in groups)
        {
            var team = group.Value;

            int Intround = 1;
            for (int i = 0; i < team.Count; i++)
            {
                if (i == 1) Intround = 3;
                if(i==2) Intround = 1;

                for (int j = i + 1; j < team.Count; j++)
                {
                    string StrRound = Intround.ToString();
                    var kitica = team[j];
                    Teams team1 = team[i];
                    Teams team2 = team[j];
                    SimulateGroup(team1, team2,rounds,StrRound);
                   
                    if (i == 1) Intround--;
                    else
                    {
                        Intround++;
                    }
                    

                }
                

            }
            Groups group1=new Groups();
            group1.Name = group.Key;
            group1.Teams = group.Value;
            leadboard.Groups.Add(group1);
            
            
        }
        foreach(var keys in rounds)
        {
           
            
            foreach(var key in keys)
            {
                var fixtures = key.Value;
                Console.WriteLine($"Kolo: {key.Key}");
                foreach(var fixture in fixtures)
                {
                    Console.WriteLine($"\t {fixture}");
                }
            }
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
             result = "";
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
        Console.WriteLine($"Polufinale:({semi_finals_teams.Count})");
        List<EliminationSemiFinal> elimination_semi_final = LotteringSemiFinal(semi_finals_teams, lottery);
        List<(Teams, Teams)> semi_final_pairs = new List<(Teams, Teams)>();
        foreach (var item in elimination_semi_final)
        {
            Console.WriteLine($"{item.Team.Team}    {item.LotteryGroup}");
            
        }
        for(int i = 0; i < elimination_semi_final.Count; i += 2)
        {
            semi_final_pairs.Add((elimination_semi_final[i].Team, elimination_semi_final[i + 1].Team));
        }
        Console.WriteLine("Polufinale:");
        List<Teams> teams_for_final_match = new List<Teams>(2);
        List<Teams> teams_for_bronze_match = new List<Teams>(2);
        List<(Teams,Teams)>final_match=new List<(Teams, Teams)>();
        List<(Teams,Teams)> match_for_bronze=new List<(Teams, Teams)>();
        foreach (var item in semi_final_pairs)
        {
            result = "";
            
            SimulateSemiFinalMatchs(item.Item1, item.Item2, out result,teams_for_final_match,teams_for_bronze_match);
            Console.WriteLine($"{item.Item1.Team} - {item.Item2.Team} ({result})");
        }
        List<MedalsRanking> ranking = new List<MedalsRanking>(3);
       
        final_match.Add((teams_for_final_match[0], teams_for_final_match[1]));
        match_for_bronze.Add((teams_for_bronze_match[0], teams_for_bronze_match[1]));
        Console.WriteLine("Utakmica za trece mesto:");
        SimulateFinalAndThirdPlace(match_for_bronze[0].Item1, match_for_bronze[0].Item2, ranking, out result);
        Console.WriteLine($"\t {match_for_bronze[0].Item1.Team}:{match_for_bronze[0].Item2.Team} ({result})");
        Console.WriteLine("Finale:");
        SimulateFinalAndThirdPlace(final_match[0].Item1, final_match[0].Item2, ranking, out result,true);
        Console.WriteLine($"\t {final_match[0].Item1.Team}:{final_match[0].Item2.Team} ({result})");
        var sorted_ranking = ranking.OrderBy(y => y.Medal);
        Console.WriteLine("Medalje:");
        k = 0;
        foreach (var rank in sorted_ranking)
        {
            
            k++;
            Console.WriteLine($"{k}. {rank.Team.Team}");
        }


    }
    public static void SimulateGroup(Teams team1, Teams team2,List<Dictionary<string, List<string>>> rounds,string round)
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
        
       GenerateScoreInGroups(winner_team, losser_team,rounds,round);
        
    }
    public static void GenerateScoreInGroups(Teams team1, Teams team2, List<Dictionary<string, List<string>>> rounds, string round)
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
        string value= $"{team1.Team}:{team2.Team}:({first_team_score}:{second_team_score})";
        AddValueIfNotExistsInRounds(round, value, rounds);
        
        //return $"{team1.Team}:{team2.Team} ({first_team_score}:{second_team_score})";


    }
    public static void AddValueIfNotExistsInRounds(string key, string value, List<Dictionary<string, List<string>>> rounds)
    {
        if (rounds.Count == 0)
        {
            var newRound = new Dictionary<string, List<string>>
        {
            { key, new List<string> { value } }
        };
            rounds.Add(newRound);
            return;
        }
        var splited_value = value.Split(":");
         var team1 =splited_value[0];
        var team2 = splited_value[1];

        bool isKeyExist = false;

        foreach (var round in rounds)
        {
            // Check if any existing key contains both teams, regardless of order
            foreach (var item in round)
            {

                if (item.Key.Contains(key))
                {
                    isKeyExist = true;
                   foreach(var vals in item.Value)
                    {
                        var splited_val = vals.Split(":");
                        var teamIn1 = splited_val[0];
                        var teamIn2 = splited_val[1];
                        if (!(teamIn1.Equals(team2) && (teamIn1.Equals(team1))&&(teamIn2.Equals(team2))&&(teamIn2.Equals(team1)))){
                            
                            round[item.Key].Add(value);
                            return;
                        }
                       
                    }
                }

                //if ((teamsInExistingKey[0] == teamsInNewKey[0] && teamsInExistingKey[1] == teamsInNewKey[1]) ||
                //    (teamsInExistingKey[0] == teamsInNewKey[1] && teamsInExistingKey[1] == teamsInNewKey[0]))
                //{
                //    keyExists = true;

                //    // Add the value only if it doesn't already exist in the list
                //    if (!round[existingKey].Contains(value))
                //    {
                //        round[existingKey].Add(value);
                //    }

                //    break;
                //}
            
            }

           
        }
            if(!isKeyExist)
        {
            // If the key doesn't exist in any form, create a new list with the value and add it to the dictionary
            var newRound = new Dictionary<string, List<string>>
        {
            { key, new List<string> { value } }
        };
            rounds.Add(newRound);
            return;
        }

        // If the key wasn't found in any round, add it as a new entry

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
            validPairsFound = true;
            pom_list.Clear(); // Očistite listu da biste krenuli ispočetka

            for (int i = 0; i < GroupD.Count; i++)
            {
                Teams teamD = GroupD[i];
                Teams teamG = GroupG[i];

                Teams teamE = GroupE[i];
                Teams teamF = GroupF[i];

                // Proveravamo da li timovi nisu igrali jedni protiv drugih
                if ((!teamD.HeadToHeadResult.Any(y => y.Key.Contains(teamG.Team))) &&
                    (!teamE.HeadToHeadResult.Any(y => y.Key.Contains(teamF.Team))))
                {
                    // Proveravamo da li timovi već postoje u pom_listi
                    bool teamDExists = pom_list.Any(pair => pair.Item1 == teamD || pair.Item2 == teamD);
                    bool teamGExists = pom_list.Any(pair => pair.Item1 == teamG || pair.Item2 == teamG);
                    bool teamEExists = pom_list.Any(pair => pair.Item1 == teamE || pair.Item2 == teamE);
                    bool teamFExists = pom_list.Any(pair => pair.Item1 == teamF || pair.Item2 == teamF);

                    // Dodajemo timove samo ako već ne postoje u pom_listi
                    if (!teamDExists && !teamGExists && !teamEExists && !teamFExists)
                    {
                        pom_list.Add((teamD, teamG));
                        pom_list.Add((teamE, teamF));
                    }
                }

                // Ako smo dodali sve parove, završavamo petlju
                if (pom_list.Count == 4)
                {
                    validPairsFound = true;
                    break;
                }
            }

            // Ako nakon jednog prolaska kroz sve timove nismo formirali sve parove, ponovno mešamo timove
            if (pom_list.Count < 4)
            {
                validPairsFound = false;
                Shuffle(GroupD);
                Shuffle(GroupE);
                Shuffle(GroupF);
                Shuffle(GroupG);
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
        
        int team1_points = SimulatingGame();
        int team2_points = SimulatingGame();
        if (team1_points == team2_points)
        {
            SimulateEliminationMatch(team1, team2, semi_finals,out el);
        }
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
    public static int SimulatingGame()
    {
        Random random_result_for_teams = new Random();
        return random_result_for_teams.Next(60, 120);
    }
    public static List<EliminationSemiFinal> LotteringSemiFinal(List<Teams> teams, Dictionary<string, List<Teams>> lottery)
    {
        List<EliminationSemiFinal> elimination_semi_final=new List<EliminationSemiFinal>();
        foreach(var lot in lottery)
        {
            var lot_teams = lot.Value;
            foreach(var lot_team in lot_teams)
            {
                foreach(var team in teams)
                {
                    if (lot_team.Team == team.Team)
                    {
                        EliminationSemiFinal eliminationSemiFinal = new EliminationSemiFinal
                        {
                            Team = lot_team,
                            LotteryGroup = lot.Key
                        };
                        elimination_semi_final.Add(eliminationSemiFinal);
                    }
                }
            }
        }
        return elimination_semi_final;
    }
    public static void SimulateSemiFinalMatchs(Teams team1,Teams team2,out string el,List<Teams> teams_final_match,List<Teams> teams_bronze_match)
    {
        int team1_points = SimulatingGame();
        int team2_points = SimulatingGame();
        if (team1_points == team2_points)
        {
            SimulateSemiFinalMatchs(team1,team2,out el,teams_final_match,teams_bronze_match);  
        }
        el = $"{team1_points}:{team2_points}";
        if (team1_points > team2_points)
        {
            teams_final_match.Add(team1);
            teams_bronze_match.Add(team2);
        }
        else
        {
            teams_final_match.Add(team2);
            teams_bronze_match.Add(team1);
        }
    }
    public static void SimulateFinalAndThirdPlace(Teams team1,Teams team2,List<MedalsRanking> ranking,out string result,bool finale_match = false)
    {
        int team1_points = SimulatingGame();
        int team2_points = SimulatingGame();
        MedalsRanking medalsRankingGold = null;
        MedalsRanking medalsRankingSilver = null;
        MedalsRanking medalRankingBronze = null;
        result = $"{team1_points}:{team2_points}";
        if (team1_points == team2_points)
        {
            SimulateFinalAndThirdPlace(team1,team2,ranking,out result,finale_match);
        }

        if(team1_points > team2_points)
        {
            if (finale_match)
            {
                medalsRankingGold = new MedalsRanking
                {
                    Team = team1,
                    Medal = MedalTypes.Gold
                };
                medalsRankingSilver = new MedalsRanking
                {
                    Team = team2,
                    Medal = MedalTypes.Silver
                };
                ranking.Add(medalsRankingGold);
                ranking.Add(medalsRankingSilver);
            }
            else
            {
                medalRankingBronze = new MedalsRanking
                {
                    Team = team1,
                    Medal = MedalTypes.Bronze
                };
                ranking.Add(medalRankingBronze);
            }
        }
        else
        {
            if (finale_match)
            {
                medalsRankingGold = new MedalsRanking
                {
                    Team = team2,
                    Medal = MedalTypes.Gold
                };
                ranking.Add(medalsRankingGold);
                medalsRankingSilver = new MedalsRanking
                {
                    Team = team1,
                    Medal = MedalTypes.Silver
                };
                ranking.Add(medalsRankingSilver);
            }
            else
            {
                medalRankingBronze = new MedalsRanking
                {
                    Team = team2,
                    Medal = MedalTypes.Bronze
                };
                ranking.Add(medalRankingBronze);
            }
        }
        ;
        
        
       
    }

    
}
    
