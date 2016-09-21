﻿using System;
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
        private readonly List<string> players = new List<string>()
        {
            Players.Ruslan,
            Players.Gleb,
            Players.Semen,
            Players.Anotron,
            Players.Serega,
            Players.Igor
        };

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
            return games.Count(g => AreNeighbors(player, sosed, g));
        }

        private static bool AreNeighbors(string player, string neighbor, Game game)
        {
            if (player == neighbor)
                return false;

            return Helper.GetPairs(game.Houses).Any(x => x.Contains(player) && x.Contains(neighbor));
        }

        [Test]
        public void WinnersStats()
        {
            var store = new Store();

            foreach (var player in players)
            {
                Console.WriteLine(player);

                var games = store.Games.Where(x => x.Houses.Any(h => h.Name == player)).ToList();
                var wins = games.Where(x=>x.Winner == player).ToList();
                Console.WriteLine($"Games: {games.Count} | Wins: {wins.Count} ({wins.Count*100/games.Count}%) ({wins.Count(x => x.WinType == WinType.Seven)} - {wins.Count(x => x.WinType == WinType.Score)})");

                var nWins = games.Count(g=>AreNeighbors(player, g.Winner, g));
                var looses = games.Count(g=>g.Winner != player);
                Console.WriteLine($"Neighbor wins: {nWins}/{looses} {nWins*100/looses}%");

                Console.WriteLine("___________________");
            }

            Console.WriteLine("_______________________________");

            foreach (HouseType type in Enum.GetValues(typeof(HouseType)))
            {
                Console.WriteLine(type);

                var games = store.Games.Where(x => x.Houses.Any(h => h.HouseType == type)).ToList();
                var wins = games.Where(x => x.Houses.Single(h=>h.Name == x.Winner).HouseType == type).ToList();
                Console.WriteLine($"Games: {games.Count} | Wins: {wins.Count} ({wins.Count * 100 / games.Count}%) ({wins.Count(x => x.WinType == WinType.Seven)} - {wins.Count(x => x.WinType == WinType.Score)})");

                Console.WriteLine("___________________");
            }
        }

        [Test]
        public void CountWithoutRepeatPairs()
        {
            var store = new Store();
            
            var result = new List<Variant>();
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

            foreach (var item in result.OrderBy(x=>x.MinusScore).Take(best.Count + 3))
            {
                Console.WriteLine("minus score: " + item.MinusScore);
                foreach (var house in item.Houses.OrderBy(x=>x.HouseType))
                {
                    Console.WriteLine(house.HouseType + " " + house.Name);
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
