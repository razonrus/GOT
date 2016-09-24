using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLogic;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class Test
    {
        public class Variant
        {
            public List<House> Houses { get; }

            public double MinusScore { get; }
            public StringBuilder Sb { get; set; }

            public Variant(List<House> houses, double minusScore, StringBuilder sb)
            {
                Houses = houses;
                MinusScore = minusScore;
                Sb = sb;
            }
        }

        [Test]
        public void Stats()
        {
            var store = new Store();

            var players = new List<string>()
            {
                Players.Dimon,
                Players.Ruslan,
                Players.Levch,
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            };
            foreach (var player in players)
            {
                Console.WriteLine(player);

                foreach (HouseType type in Enum.GetValues(typeof(HouseType)))
                {
                    Console.WriteLine(type + " " + store.Games.Count(x=>x.Houses.Any(h=>h.HouseType == type && h.Name == player)));
                }
                foreach (var neighbor in players.Where(x => x != player))
                {
                    Console.WriteLine("pair with " + neighbor + " " + GetCountOfGamesWithPair(player, neighbor, store.Games));
                }
                Console.WriteLine("___________________");
            }
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
            var looses = games.Count(g => g.Winner != player);

            return new WinStat
            {
                GamesCount = looses,
                WinsCount = nWins,
                WinsPercent = looses == 0 ? 0 : nWins * 100 / (double)looses
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
        public void CountWithoutRepeatPairs()
        {
            var store = new Store();
            
            var result = new List<Variant>();
            var players = new List<string>()
            {
                Players.Dimon,
                Players.Ruslan,
                Players.Gleb,
                Players.Levch,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            };
            var permutations = GetPermutations(players, players.Count).ToList();

            Console.WriteLine("permutations count: " + permutations.Count);

            var enumerable = permutations.Select(p => p.Select((n, i) =>
                new House
                {
                    Name = n,
                    HouseType = (HouseType)i
                }).ToList())
                .ToList();

            foreach (var houses in enumerable)
            {
                var sb = new StringBuilder();

                var minusScore = MinusMutual(houses, store.Games.OrderByDescending(x => x.Date).Take(1).ToList(), sb)
                    ;

                result.Add(new Variant(houses, minusScore, sb));
            }

            var min = result.Min(x => x.MinusScore);

            var best = result.Where(x => Math.Abs(x.MinusScore - min) < 0.001).ToList();

            Console.WriteLine("best count: " + best.Count);

            foreach (var item in result.OrderBy(x => x.MinusScore).Take(best.Count + 3))
            {
                Console.WriteLine("minus score: " + item.MinusScore);
                foreach (var house in item.Houses.OrderBy(x => x.HouseType))
                {
                    Console.WriteLine(house.HouseType + " " + house.Name);
                }
                Console.WriteLine(item.Sb.ToString());
                Console.WriteLine("_____________________________");
            }
        }

        [Test]
        public void M1()
        {
            var store = new Store();


            var result = new List<Variant>();
            var players = new List<string>()
            {
                Players.Dimon,
                //Players.Ruslan,
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            };
            var permutations = GetPermutations(players, players.Count).ToList();

            Console.WriteLine("permutations count: " + permutations.Count);

            var enumerable = permutations.Select(p => p.Select((n, i) =>
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

            Console.WriteLine("best count: " + best.Count);

            var playerStats = players.ToDictionary(x => x, x => GetPlayerWinStat(store, x));
            var neighborStats = players.ToDictionary(x => x, x => GetNeighborWinStat(store, x));
            var houseStats = Enum.GetValues(typeof (HouseType)).Cast<HouseType>().ToDictionary(x => x, x => GetHouseWinStat(store, x));

            foreach (var item in result.OrderBy(x=>x.MinusScore).Take(best.Count + 3))
            {

                var winScores = item.Houses.ToDictionary(x => x,
                    house =>
                    {
                        var neighbors = players.Where(p => AreNeighbors(p, house.Name, item.Houses)).ToList();


                        var playerStat = playerStats[house.Name];
                        var houseStat = houseStats[house.HouseType];

                        double neighborsAvgGames = neighbors.Sum(n => neighborStats[n].GamesCount) /2d;
                        return (100/6d + playerStat.WinsPercent*playerStat.GamesCount + houseStat.WinsPercent*houseStat.GamesCount + neighbors.Sum(n => neighborStats[n].WinsPercent)/4*neighborsAvgGames)/
                               (1 + playerStat.GamesCount + houseStat.GamesCount + neighborsAvgGames);

                        //var count = 1 + playerStats[house.Name].GamesCount + 

                        //var percents = new[]
                        //{
                        //    100/6d,

                        //    playerStats[house.Name].WinsPercent,
                        //    playerStats[house.Name].WinsPercent,

                        //    houseStats[house.HouseType].WinsPercent,
                        //    houseStats[house.HouseType].WinsPercent,

                        //    neighbors.Sum(n => neighborStats[n].WinsPercent)/2
                        //};
                        //return percents.Sum(x => x);
                    });


                Console.WriteLine("minus score: " + item.MinusScore);
                foreach (var house in item.Houses.OrderBy(x=>x.HouseType))
                {
                    Console.WriteLine(house.HouseType + " " + house.Name);

                    var winwith = winScores[house]*100/winScores.Sum(x=>x.Value);
                    Console.WriteLine($"Wins with: {winwith:0.##}%");
                }
                Console.WriteLine(item.Sb.ToString());
                Console.WriteLine("_____________________________");


            }
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
}
