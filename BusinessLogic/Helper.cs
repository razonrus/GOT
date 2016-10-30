using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic
{
    public class Helper
    {
        public static string[][] GetNeighbors(List<House> houses)
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
            return houses.Single(x => x.HouseType == type).PlayerName;
        }
    }
}