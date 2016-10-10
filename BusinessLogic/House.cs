using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BusinessLogic
{
    public class HouseDto
    {
        public House House { get; set; }

        public double WinsWith { get; set; }
    }
    public class House
    {
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HouseType HouseType { get; set; }

        public override string ToString()
        {
            return HouseType + " - " + Name;
        }
    }

    public class Game
    {
        public string GetHousePlayer(HouseType type)
        {
            return Houses.SingleOrDefault(x => x.HouseType == type)?.Name;
        }

        public List<House> Houses { get; set; }
        public DateTime Date { get; set; }
        public string Winner { get; set; }
        public WinType WinType { get; set; }
    }

    public class Helper
    {
        public static string[][] GetPairs(List<House> houses)
        {
            return new[]
            {
                new[] {GetName(HouseType.Baratheon, houses), GetName(HouseType.Stark, houses)},
                new[] {GetName(HouseType.Baratheon, houses), GetName(HouseType.Martell, houses)},
                new[] {GetName(HouseType.Tyrell, houses), GetName(HouseType.Martell, houses)},
                new[] {GetName(HouseType.Tyrell, houses), GetName(HouseType.Lannister, houses)},
                new[] {GetName(HouseType.Greyjoy, houses), GetName(HouseType.Lannister, houses)},
                new[] {GetName(HouseType.Greyjoy, houses), GetName(HouseType.Stark, houses)}
            };
        }

        public static string GetName(HouseType type, List<House> houses)
        {
            return houses.Single(x => x.HouseType == type).Name;
        }
    }
}