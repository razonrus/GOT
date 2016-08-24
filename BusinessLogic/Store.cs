using System;
using System.Collections.Generic;

namespace BusinessLogic
{
    public class Store
    {
        public Store()
        {
            Games = new List<Game>
            {
                new Game
                {
                    Date = new DateTime(2016, 8, 19),
                    Winner = Players.Semen,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            Name = Players.Ruslan,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            Name = Players.Gleb,
                            HouseType = HouseType.Greyjoy
                        },
                        new House
                        {
                            Name = Players.Igor,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            Name = Players.Semen,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            Name = Players.Serega,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            Name = Players.Levch,
                            HouseType = HouseType.Baratheon
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 8, 12),
                    Winner = Players.Ruslan,
                    WinType = WinType.Seven,
                    Houses = new List<House>
                    {
                        new House
                        {
                            Name = Players.Ruslan,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            Name = Players.Serega,
                            HouseType = HouseType.Greyjoy
                        },
                        new House
                        {
                            Name = Players.Gleb,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            Name = Players.Semen,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            Name = Players.Igor,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            Name = Players.Anatron,
                            HouseType = HouseType.Baratheon
                        }
                    }
                }
            };
        }

        public List<Game> Games { get; set; } 
    }

    public enum WinType
    {
        Seven,
        Score
    }
    
    public static class Players
    {
        public static string Ruslan = "Ruslan";
        public static string Anatron = "Anatron";
        public static string Semen = "Semen";
        public static string Gleb = "Gleb";
        public static string Igor = "Igor";
        public static string Levch = "Levch";
        public static string Serega = "Serega";
    }
}