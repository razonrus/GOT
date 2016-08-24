using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class Test
    {
        public class Variant
        {
            private readonly List<House> houses1;
            private readonly int minusScore1;

            public List<House> houses
            {
                get { return houses1; }
            }

            public int minusScore
            {
                get { return minusScore1; }
            }

            public Variant(List<House> houses, int minusScore)
            {
                houses1 = houses;
                minusScore1 = minusScore;
            }
        }

        [Test]
        public void M1()
        {
            var store = new Store();

            var players = new List<string>()
            {
                Players.Ruslan,
                Players.Gleb,
                Players.Semen,
                Players.Anatron,
                Players.Serega,
                Players.Igor
            };

            
            var result = new List<Variant>();
            var permutations = GetPermutations(players, players.Count).ToList();

            Console.WriteLine("permutations count: " + permutations.Count);

            var filteredByLastGame = permutations.Select(p => p.Select((n, i) =>
                new House
                {
                    Name = n,
                    HouseType = (HouseType) i
                }).ToList())
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
                var minusScore = houses.Sum(h => store.Games.Count(g => CheckSameHouse(h, g)))*10
                                 + MinusMutual(houses, store.Games)*5
                    ;

                result.Add(new Variant(houses, minusScore));
            }

            var min = result.Min(x => x.minusScore);

            var best = result.Where(x => x.minusScore == min).ToList();

            Console.WriteLine("best count: " + best.Count);

            foreach (var item in best.Take(10))
            {
                foreach (var house in item.houses)
                {
                    Console.WriteLine(house.HouseType + " " + house.Name);
                }

                Console.WriteLine("_____________________________");
            }
        }

        private int MinusMutual(List<House> houses, List<Game> games)
        {
            var result = 0;
            foreach (var game in games)
            {
                var pairs = Helper.GetPairs(game.Houses);
                result += Helper.GetPairs(houses).Count(p => pairs.SingleOrDefault(p2 => Same(p, p2)) != null);
            }
            return result;
        }

        private bool Same(string[] pair1, string[] pair2)
        {
            return pair1.Contains(pair2.First()) && pair1.Contains(pair2.Last());
        }

        private static bool CheckSameHouse(House x, Game lastGame)
        {
            return x.Name == lastGame.Houses.Single(l=>l.HouseType == x.HouseType).Name;
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
