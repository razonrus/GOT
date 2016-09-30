﻿using System;
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
        private const string BasePath = @"D:\Projects\GOT\json\";

        public class Variant
        {
            public List<HouseDto> Houses { get; }

            public double MinusScore { get; }

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
                Players.Dimon,
                Players.Levch
            };

            var playerStats = new List<PlayerStat>();

            foreach (var player in players.Where(p=>store.Games.Count(g=>g.Houses.Any(h=>h.Name == p)) > 1))
            {
                var stat = new PlayerStat
                {
                    Player = player,
                    WinStat = GetPlayerWinStat(store, player),
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
                    NeighborWinStat = GetNeighborWinStat(store, player)
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
                .ToDictionary(x=>x, x=> GetHouseWinStat(store, x));

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
                Players.Igor
            })
            {
                Console.WriteLine(player);

                var stat = GetPlayerWinStat(store, player);

                Console.WriteLine($"Games: {stat.GamesCount} | Wins: {stat.WinsCount} ({stat.WinsPercent:0.##}%) ({stat.WinsSevenCount} - {stat.WinsScoreCount})");

                var nStat = GetNeighborWinStat(store, player);

                Console.WriteLine($"Neighbor wins: {nStat.WinsCount}/{nStat.GamesCount} {nStat.WinsPercent:0.##}%");

                Console.WriteLine("___________________");
            }

            Console.WriteLine("_______________________________");

            foreach (HouseType type in Enum.GetValues(typeof(HouseType)))
            {
                Console.WriteLine(type);

                var stat = GetHouseWinStat(store, type);

                Console.WriteLine($"Games: {stat.GamesCount} | Wins: {stat.WinsCount} ({stat.WinsPercent:0.##}%) ({stat.WinsSevenCount} - {stat.WinsScoreCount})");

                Console.WriteLine("___________________");
            }
        }

        private static WinStat GetHouseWinStat(Store store, HouseType type)
        {
            var games = store.Games.Where(x => x.Houses.Any(h => h.HouseType == type)).ToList();
            var wins = games.Where(x => x.Houses.Single(h => h.Name == x.Winner).HouseType == type).ToList();

            return new WinStat
            {
                GamesCount = games.Count,
                WinsCount = wins.Count,
                WinsPercent = wins.Count * 100 / (double)games.Count,
                WinsScoreCount = wins.Count(x => x.WinType == WinType.Seven),
                WinsSevenCount = wins.Count(x => x.WinType == WinType.Score)
            };
        }

        private static WinStat GetNeighborWinStat(Store store, string player)
        {
            var games = store.Games.Where(x => x.Houses.Any(h => h.Name == player)).ToList();

            var nWins = games.Count(g => AreNeighbors(player, g.Winner, g.Houses));

            return new WinStat
            {
                GamesCount = games.Count,
                WinsCount = nWins,
                WinsPercent = nWins * 100 / (double)games.Count
            };
        }

        private static WinStat GetPlayerWinStat(Store store, string player)
        {
            var games = store.Games.Where(x => x.Houses.Any(h => h.Name == player)).ToList();
            var wins = games.Where(x => x.Winner == player).ToList();

            return new WinStat
            {
                GamesCount = games.Count,
                WinsCount = wins.Count,
                WinsPercent = games.Any() ? wins.Count*100/(double) games.Count : 0,
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
        public void M1()
        {
            var store = new Store();
            
            var result = new List<Variant>();
            var players = new List<string>()
            {
                Players.Ruslan,
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            };

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

                var minusScore = MinusHouses(houses, store, sb)
                                 + MinusMutual(houses, store.Games, sb)
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

            var playerStats = players.ToDictionary(x => x, x => GetPlayerWinStat(store, x));
            var neighborStats = players.ToDictionary(x => x, x => GetNeighborWinStat(store, x));
            var houseStats = Enum.GetValues(typeof (HouseType)).Cast<HouseType>().ToDictionary(x => x, x => GetHouseWinStat(store, x));

            foreach (var item in result.OrderBy(x => x.MinusScore).Take(best.Count + 3))
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

            SaveJson(nextGame, "next game");
        }

        private static double MinusHouses(List<House> houses, Store store, StringBuilder sb)
        {
            return houses.Sum(h => MinusHouseForPlayer(store, sb, h));
        }

        private static double MinusHouseForPlayer(Store store, StringBuilder sb, House house)
        {
            var games = store.Games.Where(x=>x.Houses.Any(h=>h.Name == house.Name))
                .OrderByDescending(x=>x.Date)
                .Select((x,i)=>
                {
                    double koef;
                    switch (i)
                    {
                        case 0:
                            koef = 15;
                            break;
                        case 1:
                            koef = 12;
                            break;
                        case 2:
                            koef = 8;
                            break;
                        case 3:
                            koef = 7;
                            break;
                        case 4:
                            koef = 6;
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

            return games.Sum(g =>
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
                        koef = 7;
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

        static IEnumerable<IEnumerable<T>> GetPermutations<T>(List<T> list, int length)
        {
            if (length == 1)
                return list.Select(t => new[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }

    public class PlayerHouseStat
    {
        public int GamesCount { get; set; }
    }
}
