using System.Collections.Generic;
using BusinessLogic;

namespace Tests
{
    public class PlayerStat

    {
        public string Player { get; set; }
        public Dictionary<HouseType, PlayerHouseStat> Houses { get; set; }
        public Dictionary<string, PlayerNeighborStat> Neighbors { get; set; }
        public Test.WinStat WinStat { get; set; }
        public Test.WinStat NeighborWinStat { get; set; }
    }
}