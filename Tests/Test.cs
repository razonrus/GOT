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

            public int MinusScore { get; }
            public StringBuilder Sb { get; set; }

            public Variant(List<House> houses, int minusScore, StringBuilder sb)
            {
                Houses = houses;
                MinusScore = minusScore;
                Sb = sb;
            }
        }

//        [Test]
//        public void M2()
//        {
//            var permutations = GetPermutations(new List<string>() {"1", "2", "3", "4", "5", "6", "7"}, 7);
//
//            var s = string.Join(Environment.NewLine, permutations.Select(x => string.Join("", x)));
//
//            Console.WriteLine(s);
//        }

        [Test]
        public void M1()
        {
            var store = new Store();

            var players = new List<string>()
            {
                Players.Ruslan,
                Players.Gleb,
                Players.Semen,
                Players.Anotron,
                Players.Serega,
                Players.Igor
            };

            
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


            var filteredByLastGame = enumerable
                .Where(h =>
                {
                    return h.Any(x =>
                    {
                        var lastGame = store.Games.Where(g => g.Houses.Any(ho => ho.Name == x.Name)).OrderBy(q => q.Date).Last();

                        return CheckSameHouse(x, lastGame);
                    }) == false;
                })
                .ToList();

            Console.WriteLine("filteredByLastGame count: " + filteredByLastGame.Count);

            foreach (var houses in filteredByLastGame)
            {
                var sb = new StringBuilder();

                var minusScore = houses.Sum(h => store.Games.Count(g => CheckSameHouse(h, g, sb)))*10
                                 + MinusMutual(houses, store.Games, sb)*5
                    ;

                result.Add(new Variant(houses, minusScore, sb));
            }

            var min = result.Min(x => x.MinusScore);

            var best = result.Where(x => x.MinusScore == min).ToList();

            Console.WriteLine("best count: " + best.Count);

            foreach (var item in best.Take(10))
            {
                foreach (var house in item.Houses.OrderBy(x=>x.HouseType))
                {
                    Console.WriteLine(house.HouseType + " " + house.Name);
                }

                Console.WriteLine(item.Sb.ToString());
                Console.WriteLine("_____________________________");
            }
        }

        private int MinusMutual(List<House> houses, List<Game> games, StringBuilder sb)
        {
            var result = 0;
            foreach (var game in games)
            {
                var pairs = Helper.GetPairs(game.Houses);
                result += Helper.GetPairs(houses).Count(p =>
                {
                    var b = pairs.SingleOrDefault(p2 => Same(p, p2)) != null;
                    if (b)
                    {
                        sb.AppendLine($"Repeat pair {p[0]}-{p[1]} with the game at {game.Date.ToShortDateString()}");
                    }
                    return b;
                });
            }
            return result;
        }

        private bool Same(string[] pair1, string[] pair2)
        {
            return pair1.Contains(pair2.First()) && pair1.Contains(pair2.Last());
        }

        private static bool CheckSameHouse(House house, Game game, StringBuilder sb = null)
        {
            var result = house.Name == game.Houses.Single(l=>l.HouseType == house.HouseType).Name;

            if (sb != null && result)
            {
                sb.AppendLine($"Repeat {house} with the game at {game.Date.ToShortDateString()}");
            }

            return result;
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
