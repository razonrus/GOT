using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BusinessLogic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class Test
    {
        private const string BasePath = @"C:\Home_Project\GOT-master\json\";

        public class Variant
        {
            public List<HouseDto> Houses { get; }

            public double MinusScore { get; set; }

            [JsonIgnore]
            private StringBuilder Sb { get; }

            public string Description => Sb.ToString();
            public double Probability { get; set; }

            public Variant(List<House> houses, double minusScore, StringBuilder sb)
            {
                Houses = houses.Select(h => new HouseDto()
                {
                    House = h
                }).ToList();
                MinusScore = minusScore;
                Sb = sb;
            }
        }

        [Test]
        public void ToJson()
        {
            SaveJson(new Store(), @"store");
        }

        [Test]
        public void Stats()
        {
            var store = new Store();

            var players = new List<string>
            {
                Players.Ruslan,
                Players.Valya,
                Players.Maxim,
                Players.Dima_M,
                Players.Misha,
                Players.Sasha,
                Players.Stas,
                Players.Leha
            };

            var playerStats = new List<PlayerStat>();

            foreach (var player in players.Where(p=>store.Games.Count(g=>g.Houses.Any(h=>h.PlayerName == p)) > 0))
            {
                var stat = new PlayerStat
                {
                    Player = player,
                    WinStat = GetPlayerWinStat(player, store.Games),
                    Houses =
                        Enum.GetValues(typeof (HouseType)).Cast<HouseType>()
                            .ToDictionary(x => x,
                                type =>
                                {
                                    var games = store.Games.Where(x => x.Houses.Any(h => h.HouseType == type && h.PlayerName == player)).ToList();
                                    return new PlayerHouseStat
                                    {
                                        GamesCount = games.Count,
                                        WinsCount = games.Count(x=>x.Winner == player)
                                    };
                                }),
                    Neighbors = players.Where(x => x != player)
                        .ToDictionary(x => x, p =>
                        {
                            var games = store.Games.Where(g => AreNeighbors(player, p, g.Houses)).ToList();
                            return new PlayerNeighborStat
                            {
                                GamesCountWithPair = games.Count,
                                WinsCountWithPair = games.Count(x=>x.Winner == player)
                            };
                        }),
                    NeighborWinStat = GetNeighborWinStat(player, store.Games)
                };

                playerStats.Add(stat);

                Console.WriteLine(player);

                foreach (var houseStat in stat.Houses)
                    Console.WriteLine(houseStat.Key + " " + houseStat.Value.GamesCount);

                foreach (var neighbor in stat.Neighbors)
                {
                    Console.WriteLine("pair with " + neighbor.Key + " " + neighbor.Value.GamesCountWithPair);
                }
                Console.WriteLine("___________________");
            }


            var houseStats = Enum.GetValues(typeof(HouseType)).Cast<HouseType>()
                .ToDictionary(x=>x, x=> GetHouseWinStat(x, store.Games));

            SaveJson(new
            {
                PlayerStats = playerStats,
                HouseStats = houseStats
            }, @"statistic");
        }

        private static void SaveJson(object value, string fileName)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string json = JsonConvert.SerializeObject(value, Formatting.Indented, serializerSettings);
            File.WriteAllText(BasePath + fileName + @".json", json);
        }

        private static bool AreNeighbors(string player, string neighbor, List<House> houses)
        {
            if (player == neighbor)
                return false;

            return Helper.GetNeighbors(houses).Any(x => x.Contains(player) && x.Contains(neighbor));
        }

        [Test]
        public void RelativeScores()
        {
            var store = new Store();

            var houseWins = Enum.GetValues(typeof (HouseType)).Cast<HouseType>()
                .ToDictionary(x => x, x => store.Games.Count(g => g.Winner == g.GetHousePlayer(x)));

            var dict = Players.All().ToDictionary(x => x, x => (double) 0);

            foreach (var game in store.Games)
            {
                dict[game.Winner] += 1/(double) houseWins[game.GetHouseType(game.Winner).Value];
            }

            foreach (var pair in dict.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{pair.Key} {pair.Value:0.##}");
            }
        }

        [Test]
        public void CurrentForm()
        {
            var store = new Store();

            var games = store.Games.OrderBy(x => x.Date).ToList();

            var dict = Players.All().ToDictionary(x => x, x=>0);

            for (int i = 0; i < games.Count; i++)
            {
                dict[games[i].Winner] += i + 1;
            }

            foreach (var pair in dict.OrderByDescending(x=>x.Value))
            {
                Console.WriteLine(pair.Key + " " + pair.Value);
            }
        }


        [Test]
        public void WinnersStats()
        {
            var store = new Store();

            foreach (var player in new List<string>()
            {
                Players.Dima_M,
                Players.Ruslan,
                Players.Valya,
                Players.Sasha,
                Players.Leha,
                Players.Maxim,
                Players.Misha,
                Players.Stas
            })
            {
                Console.WriteLine(player);

                var stat = GetPlayerWinStat(player, store.Games);

                Console.WriteLine($"Games: {stat.GamesCount} | Wins: {stat.WinsCount} ({stat.WinsPercent:0.##}%) ({stat.WinsSevenCount} - {stat.WinsScoreCount})");

                var nStat = GetNeighborWinStat(player, store.Games);

                Console.WriteLine($"Neighbor wins: {nStat.WinsCount}/{nStat.GamesCount} {nStat.WinsPercent:0.##}%");

                Console.WriteLine("___________________");
            }

            Console.WriteLine("_______________________________");

            foreach (HouseType type in Enum.GetValues(typeof(HouseType)))
            {
                Console.WriteLine(type);

                var stat = GetHouseWinStat(type, store.Games);

                Console.WriteLine($"Games: {stat.GamesCount} | Wins: {stat.WinsCount} ({stat.WinsPercent:0.##}%) ({stat.WinsSevenCount} - {stat.WinsScoreCount})");

                Console.WriteLine("___________________");
            }
        }

        private static WinStat GetHouseWinStat(HouseType type, List<Game> games)
        {
            var houseGames = games.Where(x => x.Houses.Any(h => h.HouseType == type)).ToList();
            var wins = houseGames.Where(x => x.Houses.Single(h => h.PlayerName == x.Winner).HouseType == type).ToList();

            return new WinStat
            {
                GamesCount = houseGames.Count,
                WinsCount = wins.Count,
                WinsPercent = houseGames.Count == 0 ? 0 : wins.Count * 100 / (double)houseGames.Count,
                WinsScoreCount = wins.Count(x => x.WinType == WinType.Seven),
                WinsSevenCount = wins.Count(x => x.WinType == WinType.Score)
            };
        }

        private static WinStat GetNeighborWinStat(string player, List<Game> games)
        {
            var playerGames = games.Where(x => x.Houses.Any(h => h.PlayerName == player)).ToList();

            var nWins = playerGames.Count(g => AreNeighbors(player, g.Winner, g.Houses));

            return new WinStat
            {
                GamesCount = playerGames.Count,
                WinsCount = nWins,
                WinsPercent = playerGames.Count == 0 ? 0 : nWins * 100 / (double)playerGames.Count
            };
        }

        private static WinStat GetPlayerWinStat(string player, List<Game> games)
        {
            var playerGames = games.Where(x => x.Houses.Any(h => h.PlayerName == player)).ToList();
            var wins = playerGames.Where(x => x.Winner == player).ToList();

            return new WinStat
            {
                GamesCount = playerGames.Count,
                WinsCount = wins.Count,
                WinsPercent = playerGames.Any() ? wins.Count*100/(double) playerGames.Count : 0,
                WinsScoreCount = wins.Count(x => x.WinType == WinType.Seven),
                WinsSevenCount = wins.Count(x => x.WinType == WinType.Score)
            };
        }

        public class WinStat
        {
            public int GamesCount { get; set; }
            public int WinsCount { get; set; }
            public int WinsSevenCount { get; set; }
            public int WinsScoreCount { get; set; }
            public double WinsPercent{ get; set; }
        }
        
        [Test]
        public void GenerateNextGame()
        {
            var players = new List<string>()
            {
                Players.Ruslan,
                Players.Leha,
                Players.Dima_M,
                Players.Misha,
                Players.Sasha,
                Players.Maxim
            };
            Store store = new Store();
            var nextGame = GetNextGame(players, store.Games);

            var houses = nextGame.Variants.First().Houses.Select(d=>d.House).ToList();
            var facts = GetFacts(players)
                .Where(x=> x.ConditionPredicate.Function(houses))
                .ToList()
                ;

            Console.WriteLine("Тенденции победы:");
            WriteFacts(facts.Where(x=>x.ResultPredicate.IsFromCondition == false).ToList());
            Console.WriteLine("_____________________________________________________________________________");

            Console.WriteLine("Тенденции расклада:");
            WriteFacts(facts.Where(x => x.ResultPredicate.IsFromCondition).ToList(), houses);
            Console.WriteLine("_____________________________________________________________________________");
            

            SaveJson(nextGame, "next game");
        }

        [Test]
        public void ShowNextGameChances()
        {
            var random = new Random();

            var players = new List<string>()
            {
                Players.Ruslan,
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            };
            Store store = new Store();

            var result = GetAllVariants(players, store.Games);

            var sum = result.Sum(variant => Invert(variant.MinusScore));
            foreach (var variant in result)
            {
                variant.Probability = Invert(variant.MinusScore)/sum*100;
            }

            Console.WriteLine(result.Sum(x => x.Probability));

            

            var min = result.Min(x => x.Probability);

            var stepsCount = (int) Math.Ceiling(100/min);
            var step = 100/(double) stepsCount;

            //Assert.LessOrEqual(step, min);

            var randomValue = random.Next(stepsCount + 1)*step;



            Console.WriteLine("random: " + randomValue);
            double sum2 = 0;
            foreach (var variant in result.OrderByDescending(x => x.Probability))
            {
                sum2 += variant.Probability;
                if (sum2 > randomValue)
                {
                    WriteVariant(variant);
                    break;
                }
            }



            Console.WriteLine("__________________________________________");
            Console.WriteLine("__________________________________________");
            Console.WriteLine("__________________________________________");

            var dict = players.ToDictionary(x => x, x => Enum.GetValues(typeof (HouseType)).Cast<HouseType>().ToDictionary(t => t, t => (double)0));
            var dictNeighbors = players.ToDictionary(x => x, x => players.ToDictionary(t => t, t => (double)0));

            foreach (var variant in result)
            {
                foreach (var house in variant.Houses)
                    dict[house.House.PlayerName][house.House.HouseType] += variant.Probability;

                foreach (var pair in Helper.GetNeighbors(variant.Houses.Select(x=>x.House).ToList()))
                {
                    dictNeighbors[pair.First()][pair.Last()] += variant.Probability;
                    dictNeighbors[pair.Last()][pair.First()] += variant.Probability;
                }
            }

            foreach (var pair in dict)
            {
                Console.WriteLine(pair.Key + ":");
                foreach (var d in pair.Value.OrderByDescending(x => x.Value))
                {
                    Console.WriteLine($"{d.Key} {d.Value:0.##}%");
                }
                Console.WriteLine("__________________________________________");
                Console.WriteLine("Соседство с:");
                foreach (var neighbor in dictNeighbors[pair.Key].OrderByDescending(x=>x.Value))
                {
                    if(neighbor.Value > 0)
                        Console.WriteLine($"{neighbor.Key} {neighbor.Value:0.##}%");
                }
                Console.WriteLine("__________________________________________");
                Console.WriteLine("__________________________________________");
            }

            Console.WriteLine("__________________________________________");
            Console.WriteLine("__________________________________________");
            Console.WriteLine("__________________________________________");


            foreach (var item in result.OrderBy(x => x.MinusScore).Take(10).Concat(result.OrderByDescending(x => x.MinusScore).Take(3).OrderBy(x => x.MinusScore)))
            {
                WriteVariant(item);
            }
        }

        private static void WriteVariant(Variant item)
        {
            Console.WriteLine("minus score: " + item.MinusScore);
            Console.WriteLine("Probability: " + item.Probability);
            foreach (var house in item.Houses.OrderBy(x => x.House.HouseType))
            {
                Console.WriteLine(house.House.HouseType + " " + house.House.PlayerName);
            }
            Console.WriteLine("__________________________________________");
        }

        private static double Invert(double minusScore)
        {
            return Math.Pow(minusScore, -12);
        }

        [Test]
        public void M2()
        {
            var players1 = new List<string>()
            {
                Players.Ruslan,
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            };
            Store store = new Store();
            var nextGame = GetNextGame(players1, store.Games);

            Console.WriteLine("_____________________________________________________________________________");
            Console.WriteLine("_____________________________________________________________________________");
            Console.WriteLine("_____________________________________________________________________________");
            GetNextGame(new List<string>()
            {
                Players.Ruslan,
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            }, store.Games.Concat(new[] { new Game
            {
                Houses = nextGame.Variants.First().Houses.Select(x=>x.House).ToList(),
                Date = DateTime.Today,
                Winner = players1.First()
            } }).ToList());
        }

        private NextGame GetNextGame(List<string> players, List<Game> games)
        {
            var result = GetAllVariants(players, games);

            var min = result.Min(x => x.MinusScore);

            var best = result.Where(x => Math.Abs(x.MinusScore - min) < 0.001).ToList();

            var nextGame = new NextGame
            {
                BestCount = best.Count,
                Variants = new List<Variant>()
            };

            Console.WriteLine("best count: " + best.Count);

            var playerStats = players.ToDictionary(x => x, x => GetPlayerWinStat(x, games));
            var neighborStats = players.ToDictionary(x => x, x => GetNeighborWinStat(x, games));
            var houseStats = Enum.GetValues(typeof (HouseType)).Cast<HouseType>().ToDictionary(x => x, x => GetHouseWinStat(x, games));

            foreach (var item in result.OrderBy(x => x.MinusScore).Take(best.Count + 3))
            {
                var winScores = item.Houses.ToDictionary(x => x,
                    dto =>
                    {
                        var neighbors = players.Where(p => AreNeighbors(p, dto.House.PlayerName, item.Houses.Select(x => x.House).ToList())).ToList();


                        var playerStat = playerStats[dto.House.PlayerName];
                        var houseStat = houseStats[dto.House.HouseType];

                        double neighborsAvgGames = neighbors.Sum(n => neighborStats[n].GamesCount)/2d;
                        return (100/6d + playerStat.WinsPercent*playerStat.GamesCount + houseStat.WinsPercent*houseStat.GamesCount + neighbors.Sum(n => neighborStats[n].WinsPercent)/4*neighborsAvgGames)/
                               (1 + playerStat.GamesCount + houseStat.GamesCount + neighborsAvgGames);
                    });


                Console.WriteLine("minus score: " + item.MinusScore);
                foreach (var house in item.Houses.OrderBy(x => x.House.HouseType))
                {
                    Console.WriteLine(house.House.HouseType + " " + house.House.PlayerName);

                    house.WinsWith = winScores[house]*100/winScores.Sum(x => x.Value);
                    Console.WriteLine($"Wins with: {house.WinsWith:0.##}%");
                }
                Console.WriteLine(item.Description);
                Console.WriteLine("_____________________________");

                nextGame.Variants.Add(item);
            }
            return nextGame;
        }

        private List<Variant> GetAllVariants(List<string> players, List<Game> games)
        {
            var result = new List<Variant>();

            var enumerable = GetPermutations(players, players.Count).ToList()
                .Select(p => p.Select((n, i) =>
                    new House
                    {
                        PlayerName = n,
                        HouseType = (HouseType) i
                    }).ToList())
                .ToList();

            foreach (var houses in enumerable)
            {
                var sb = new StringBuilder();

                var minusScore = MinusHouses(houses, sb, games)
                                 + MinusMutual(houses, games, sb)
                    ;

                result.Add(new Variant(houses, minusScore, sb));
            }
            return result;
        }

        [Test]
        public void EqualChanсes()
        {
            var players = new List<string>()
            {
                Players.Ruslan,
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            };
            var store = new Store();
            var result = new List<Variant>();

            var enumerable = GetPermutations(players, players.Count).ToList()
                .Select(p => p.Select((n, i) =>
                    new House
                    {
                        PlayerName = n,
                        HouseType = (HouseType) i
                    }).ToList())
                .ToList();
            
            foreach (var houses in enumerable)
            {
                var sb = new StringBuilder();
                
                result.Add(new Variant(houses, 0, sb));
            }

            var playerStats = players.ToDictionary(x => x, x => GetPlayerWinStat(x, store.Games));
            var neighborStats = players.ToDictionary(x => x, x => GetNeighborWinStat(x, store.Games));
            var houseStats = Enum.GetValues(typeof (HouseType)).Cast<HouseType>().ToDictionary(x => x, x => GetHouseWinStat(x, store.Games));

            foreach (var item in result)
            {
                var winScores = item.Houses.ToDictionary(x => x,
                    dto =>
                    {
                        var neighbors = players.Where(p => AreNeighbors(p, dto.House.PlayerName, item.Houses.Select(x=>x.House).ToList())).ToList();


                        var playerStat = playerStats[dto.House.PlayerName];
                        var houseStat = houseStats[dto.House.HouseType];

                        double neighborsAvgGames = neighbors.Sum(n => neighborStats[n].GamesCount)/2d;
                        return (100/6d + playerStat.WinsPercent*playerStat.GamesCount + houseStat.WinsPercent*houseStat.GamesCount + neighbors.Sum(n => neighborStats[n].WinsPercent)/4*neighborsAvgGames)/
                               (1 + playerStat.GamesCount + houseStat.GamesCount + neighborsAvgGames);
                    });
                
                foreach (var house in item.Houses.OrderBy(x => x.House.HouseType))
                {
                    house.WinsWith = winScores[house]*100/winScores.Sum(x => x.Value);
                }

                item.MinusScore = item.Houses.Sum(h => Math.Abs(h.WinsWith - 100/6d));
            }

            var min = result.Min(x => x.MinusScore);

            var best = result.Where(x => Math.Abs(x.MinusScore - min) < 0.001).ToList();


            Console.WriteLine("best count: " + best.Count);

            foreach (var item in result.OrderBy(x => x.MinusScore).Take(best.Count + 3))
            {
                Console.WriteLine("minus score: " + item.MinusScore);
                foreach (var house in item.Houses.OrderBy(x => x.House.HouseType))
                {
                    Console.WriteLine(house.House.HouseType + " " + house.House.PlayerName);
                    
                    Console.WriteLine($"Wins with: {house.WinsWith:0.##}%");
                }
                Console.WriteLine("_____________________________");
                
            }
        }

        private static double MinusHouses(List<House> houses, StringBuilder sb, List<Game> games)
        {
            return houses.Sum(h => MinusHouseForPlayer(sb, h, games));
        }

        private static double MinusHouseForPlayer(StringBuilder sb, House house, List<Game> games)
        {
            var houseGames = games.Where(x=>x.Houses.Any(h=>h.PlayerName == house.PlayerName))
                .OrderByDescending(x=>x.Date)
                .Select((x,i)=>
                {
                    double koef;
                    switch (i)
                    {
                        case 0:
                            koef = 20;
                            break;
                        case 1:
                            koef = 12;
                            break;
                        case 2:
                            koef = 8;
                            break;
                        case 3:
                            koef = 5;
                            break;
                        case 4:
                            koef = 4;
                            break;
                        default:
                            koef = 1+1d/i;
                            break;
                    }

                    return new
                    {
                        Game = x,
                        Koeff = koef
                    };
                });

            return houseGames.Sum(g =>
            {
                var result = house.PlayerName == g.Game.Houses.Single(l=>l.HouseType == house.HouseType).PlayerName;

                if (sb != null && result && g.Game.Date > DateTime.Today.AddMonths(-1))
                {
                    sb.AppendLine($"Repeat {house} with the game at {g.Game.Date.ToShortDateString()}");
                }
                if (result)
                    return g.Koeff;
                return 0d;
            });
        }

        private double MinusMutual(List<House> houses, List<Game> games, StringBuilder sb)
        {
            double result = 0;
            games = games.OrderByDescending(x => x.Date).ToList();
            for (int index = 0; index < games.Count; index++)
            {
                double koef;
                switch (index)
                {
                    case 0:
                        koef = 10;
                        break;
                    case 1:
                        koef = 2;
                        break;
                    default:
                        koef = 1 + 1d / index;
                        break;
                }

                var game = games[index];
                var pairs = Helper.GetNeighbors(game.Houses);
                result += Helper.GetNeighbors(houses).Count(p =>
                {
                    var b = pairs.SingleOrDefault(p2 => Same(p, p2)) != null;
                    if (b && game.Date > DateTime.Today.AddMonths(-1))
                    {
                        sb.AppendLine($"Repeat pair {p[0]}-{p[1]} with the game at {game.Date.ToShortDateString()}");
                    }
                    return b;
                }) * koef;
            }
            return result;
        }

        private bool Same(string[] pair1, string[] pair2)
        {
            return pair1.Contains(pair2.First()) && pair1.Contains(pair2.Last());
        }

        static List<List<T>> GetPermutations<T>(List<T> list, int length)
        {
            if (length == 1)
                return list.Select(t => new List<T> {t}).ToList();

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new[] {t2}).ToList())
                    .ToList();
        }

        private class Fact
        {
            public override string ToString()
            {
                return $"{ResultPredicate.Name}, если {ConditionPredicate.Name} ({GamesCount} игры)";
            }

            public ConditionPredicate ConditionPredicate { get; set; }
            public ResultPredicate ResultPredicate { get; set; }
            public int GamesCount { get; set; }

            public string ToString(List<House> houses)
            {
                return $"{ToString()} {(ResultPredicate.Function(new Game {Houses = houses}) == false ? "НАРУШЕНО!" : "ПОДТВЕРЖДЕНО!")}";
            }
        }
        private class ResultPredicate
        {
            public string Name { get; }

            public Func<Game, bool?> Function { get; }
            public bool IsFromCondition { get; set; }

            public ResultPredicate(string name, Func<Game, bool?> function)
            {
                this.Name = name;
                this.Function = function;
            }
        }
        private class ConditionPredicate
        {
            public string Name { get; }

            public Func<List<House>, bool> Function { get; }

            public ConditionPredicate(string name, Func<List<House>, bool> function)
            {
                Name = name;
                Function = function;
            }

            public ResultPredicate ToResultPredicate()
            {
                return new ResultPredicate(Name, x => Function(x.Houses))
                {
                    IsFromCondition = true
                }
                    ;
            }
        }

        [Test]
        public void Facts()
        {
            var facts = GetFacts(Players.All());

            WriteFacts(facts);
        }

        private static void WriteFacts(List<Fact> facts, List<House> nextGame = null)
        {
            foreach (var line in facts
                .OrderBy(x=>x.ResultPredicate.IsFromCondition)
                .ThenBy(x=>facts.Count(f=>f.ResultPredicate.Name == x.ResultPredicate.Name))
                //.OrderBy(x => x.ResultPredicate.PlayerName.Contains("не побеждает"))
                .ThenBy(x => x.ResultPredicate.Name)
                .Select(fact => nextGame == null ? fact.ToString() : fact.ToString(nextGame))
                )
            {
                Console.WriteLine(line);
            }
        }

        private static List<Fact> GetFacts(List<string> players)
        {
            var store = new Store();

            var conditionPredicates =
                players.SelectMany(p => Enum.GetValues(typeof (HouseType)).
                    Cast<HouseType>()
                    .Select(h => new ConditionPredicate($"{h} - {p}", x => Game.GetHousePlayer(h, x) == p))
                    )
                    .Concat(
                        GetPermutations(players.ToList(), 2)
                            .GroupBy(x =>
                            {
                                x.Sort();
                                return x.First() + x.Last();
                            })
                            .Select(x => x.First())
                            .Select(p => new ConditionPredicate($"соседи {p.First()} и {p.Last()}", x => AreNeighbors(p.First(), p.Last(), x)))
                    )
                    .ToList();

            var resultPredicates =
                Enum.GetValues(typeof (HouseType)).
                    Cast<HouseType>()
                    .Select(h => new ResultPredicate($"{h} побеждает", x => Game.GetHousePlayer(h, x.Houses) == x.Winner))
                    .Concat(
                        Enum.GetValues(typeof (HouseType)).
                            Cast<HouseType>()
                            .Select(h => new ResultPredicate($"{h} не побеждает", x => Game.GetHousePlayer(h, x.Houses) != x.Winner))
                    )
                    .Concat(
                        players.Select(p => new ResultPredicate($"{p} побеждает", x => x.Winner == p))
                    )
                    .Concat(
                        players.Select(p => new ResultPredicate($"{p} не побеждает", x =>
                        {
                            if (x.Contains(p) == false)
                                return null;
                            return x.Winner != p;
                        }))
                    )
                    .Concat(conditionPredicates.Select(x => x.ToResultPredicate()))
                    .Concat(
                        players.Select(p => new ResultPredicate($"сосед игрока {p} побеждает", x => AreNeighbors(p, x.Winner, x.Houses)))
                    )
                    .Concat(
                        players.Select(p => new ResultPredicate($"сосед игрока {p} не побеждает", x =>
                        {
                            if (x.Contains(p) == false)
                                return null;
                            return AreNeighbors(p, x.Winner, x.Houses) == false;
                        }))
                    )
                    .Concat(
                        Enum.GetValues(typeof (HouseType)).
                            Cast<HouseType>()
                            .Select(h => new ResultPredicate($"сосед {h}'ов(ев) побеждает", x => AreNeighbors(Game.GetHousePlayer(h, x.Houses), x.Winner, x.Houses)))
                    )
                    .Concat(
                        Enum.GetValues(typeof (HouseType)).
                            Cast<HouseType>()
                            .Select(h => new ResultPredicate($"сосед {h}'ов(ев) не побеждает", x => AreNeighbors(Game.GetHousePlayer(h, x.Houses), x.Winner, x.Houses) == false))
                    )
                    .Concat(
                        new[]
                        {
                            new ResultPredicate("победитель определяется по очкам", x => x.WinType == WinType.Score),
                            new ResultPredicate("победитель берёт 7 замков", x => x.WinType == WinType.Seven)
                        }
                    )
                    .ToList()
                ;

            var facts = new List<Fact>();
            foreach (var condition in conditionPredicates)
            {
                facts.AddRange(resultPredicates
                    .Where(predicate2 => condition.Name != predicate2.Name)
                    .Where(result => store.Games
                        .Where(x => result.Function(x) != null)
                        .Where(x => condition.Function(x.Houses))
                        .All(x => result.Function(x) == true))
                    .Select(result => new Fact
                    {
                        GamesCount = store.Games.Count(x => result.Function(x) != null && condition.Function(x.Houses)),
                        ConditionPredicate = condition,
                        ResultPredicate = result
                    })
                    .Where(x => x.GamesCount > 1)
                    );
            }
            return facts;
        }
    }

    public class PlayerHouseStat
    {
        public int GamesCount { get; set; }
        public int WinsCount { get; set; }
    }
}
