﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
        private const string BasePath = @"D:\Projects\GOT\json\";

        public class Variant
        {
            public List<HouseDto> Houses { get; }

            public double MinusScore { get; set; }

            [JsonIgnore]
            private StringBuilder Sb { get; }

            public string Description => Sb.ToString();

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
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor,
                Players.Edele,
                Players.Dimon,
                Players.Levch
            };

            var playerStats = new List<PlayerStat>();

            foreach (var player in players.Where(p=>store.Games.Count(g=>g.Houses.Any(h=>h.Name == p)) > 1))
            {
                var stat = new PlayerStat
                {
                    Player = player,
                    WinStat = GetPlayerWinStat(player, store.Games),
                    Houses =
                        Enum.GetValues(typeof (HouseType)).Cast<HouseType>()
                            .ToDictionary(x => x,
                                type =>
                                    new PlayerHouseStat
                                    {
                                        GamesCount = store.Games.Count(x => x.Houses.Any(h => h.HouseType == type && h.Name == player))
                                    }),
                    Neighbors = players.Where(x => x != player)
                        .ToDictionary(x => x, p => new PlayerNeighborStat
                        {
                            GamesCountWithPair = GetCountOfGamesWithPair(player, p, store.Games)
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

        private static int GetCountOfGamesWithPair(string player, string sosed, List<Game> games)
        {
            return games.Count(g => AreNeighbors(player, sosed, g.Houses));
        }

        private static bool AreNeighbors(string player, string neighbor, List<House> houses)
        {
            if (player == neighbor)
                return false;

            return Helper.GetPairs(houses).Any(x => x.Contains(player) && x.Contains(neighbor));
        }

        [Test]
        public void WinnersStats()
        {
            var store = new Store();

            foreach (var player in new List<string>()
            {
                Players.Dimon,
                Players.Ruslan,
                Players.Levch,
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor,
                Players.Edele
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
            var wins = houseGames.Where(x => x.Houses.Single(h => h.Name == x.Winner).HouseType == type).ToList();

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
            var playerGames = games.Where(x => x.Houses.Any(h => h.Name == player)).ToList();

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
            var playerGames = games.Where(x => x.Houses.Any(h => h.Name == player)).ToList();
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
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            };
            Store store = new Store();
            var nextGame = GetNextGame(players, store.Games);

            var facts = GetFacts()
                .Where(x=>x.ConditionPredicate.Function(nextGame.Variants.First().Houses.Select(d=>d.House).ToList()))
                .ToList()
                ;

            WriteFacts(facts);
            
            SaveJson(nextGame, "next game");
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
            var result = new List<Variant>();

            var enumerable = GetPermutations(players, players.Count).ToList()
                .Select(p => p.Select((n, i) =>
                    new House
                    {
                        Name = n,
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
                        var neighbors = players.Where(p => AreNeighbors(p, dto.House.Name, item.Houses.Select(x => x.House).ToList())).ToList();


                        var playerStat = playerStats[dto.House.Name];
                        var houseStat = houseStats[dto.House.HouseType];

                        double neighborsAvgGames = neighbors.Sum(n => neighborStats[n].GamesCount)/2d;
                        return (100/6d + playerStat.WinsPercent*playerStat.GamesCount + houseStat.WinsPercent*houseStat.GamesCount + neighbors.Sum(n => neighborStats[n].WinsPercent)/4*neighborsAvgGames)/
                               (1 + playerStat.GamesCount + houseStat.GamesCount + neighborsAvgGames);
                    });


                Console.WriteLine("minus score: " + item.MinusScore);
                foreach (var house in item.Houses.OrderBy(x => x.House.HouseType))
                {
                    Console.WriteLine(house.House.HouseType + " " + house.House.Name);

                    house.WinsWith = winScores[house]*100/winScores.Sum(x => x.Value);
                    Console.WriteLine($"Wins with: {house.WinsWith:0.##}%");
                }
                Console.WriteLine(item.Description);
                Console.WriteLine("_____________________________");

                nextGame.Variants.Add(item);
            }
            return nextGame;
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
                        Name = n,
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
                        var neighbors = players.Where(p => AreNeighbors(p, dto.House.Name, item.Houses.Select(x=>x.House).ToList())).ToList();


                        var playerStat = playerStats[dto.House.Name];
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
                    Console.WriteLine(house.House.HouseType + " " + house.House.Name);
                    
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
            var houseGames = games.Where(x=>x.Houses.Any(h=>h.Name == house.Name))
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
                var result = house.Name == g.Game.Houses.Single(l=>l.HouseType == house.HouseType).Name;

                if (sb != null && result)
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
                var pairs = Helper.GetPairs(game.Houses);
                result += Helper.GetPairs(houses).Count(p =>
                {
                    var b = pairs.SingleOrDefault(p2 => Same(p, p2)) != null;
                    if (b)
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
                return $"Всегда, когда {ConditionPredicate.Name} ({GamesCount} игры) - {ResultPredicate.Name}";
            }

            public ConditionPredicate ConditionPredicate { get; set; }
            public ResultPredicate ResultPredicate { get; set; }
            public int GamesCount { get; set; }
        }
        private class ResultPredicate
        {
            public string Name { get; }

            public Func<Game, bool> Function { get; }

            public ResultPredicate(string name, Func<Game, bool> function)
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
                return new ResultPredicate(Name, x=>Function(x.Houses));
            }
        }

        [Test]
        public void Facts()
        {
            var facts = GetFacts();

            WriteFacts(facts);
        }

        private static void WriteFacts(List<Fact> facts)
        {
            foreach (var line in facts
                .Select(fact => fact.ToString())
                .OrderByDescending(x => x.Contains("побеждает"))
                .ThenBy(x => x)
                )
            {
                Console.WriteLine(line);
            }
        }

        private static List<Fact> GetFacts()
        {
            var store = new Store();

            var conditionPredicates =
                Players.All().SelectMany(p => Enum.GetValues(typeof (HouseType)).
                    Cast<HouseType>()
                    .Select(h => new ConditionPredicate($"{h} - {p}", x => Game.GetHousePlayer(h, x) == p))
                    )
                    .Concat(
                        GetPermutations(Players.All().ToList(), 2)
                            .GroupBy(x =>
                            {
                                x.Sort();
                                return x.First() + x.Last();
                            })
                            .Select(x => x.First())
                            .Select(p => new ConditionPredicate($"соседи {p.First()} и {p.Last()}", x => AreNeighbors(p.First(), p.Last(), x)))
                    )
                    .ToList();

            var predicate2s =
                Enum.GetValues(typeof (HouseType)).
                    Cast<HouseType>()
                    .Select(h => new ResultPredicate($"побеждает {h}", x => Game.GetHousePlayer(h, x.Houses) == x.Winner))
                    .Concat(
                        Enum.GetValues(typeof (HouseType)).
                            Cast<HouseType>()
                            .Select(h => new ResultPredicate($"не побеждает {h}", x => Game.GetHousePlayer(h, x.Houses) != x.Winner))
                    )
                    .Concat(
                        Players.All().Select(p => new ResultPredicate($"побеждает {p}", x => x.Winner == p))
                    )
                    .Concat(
                        Players.All().Select(p => new ResultPredicate($"не побеждает {p}", x => x.Winner != p))
                    )
                    .Concat(conditionPredicates.Select(x=>x.ToResultPredicate()))
                    .Concat(
                        Players.All().Select(p => new ResultPredicate($"побеждает сосед игрока {p}", x => AreNeighbors(p, x.Winner, x.Houses)))
                    )
                    .Concat(
                        Enum.GetValues(typeof (HouseType)).
                            Cast<HouseType>()
                            .Select(h => new ResultPredicate($"побеждает сосед {h}'ов(ев)", x => AreNeighbors(Game.GetHousePlayer(h, x.Houses), x.Winner, x.Houses)))
                    )
                    .ToList()
                ;

            var facts = new List<Fact>();
            foreach (var predicate1 in conditionPredicates)
            {
                facts.AddRange(predicate2s
                    .Where(predicate2 => predicate1.Name != predicate2.Name)
                    .Where(predicate2 => store.Games
                        .Where(x=>predicate1.Function(x.Houses))
                        .All(predicate2.Function))
                    .Select(predicate2 => new Fact
                    {
                        GamesCount = store.Games.Count(x=>predicate1.Function(x.Houses)),
                        ConditionPredicate = predicate1,
                        ResultPredicate = predicate2
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
    }
}
