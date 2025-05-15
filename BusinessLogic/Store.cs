using System;
using System.Collections.Generic;
using System.Linq;

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
                    Date = new DateTime(2025, 1, 31),
                    Winner = Players.Maxim,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Maxim,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Dima_M,
                            HouseType = HouseType.Greyjoy
                        },
                        new House
                        {
                            PlayerName = Players.Stas,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Misha,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Sasha,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Valya,
                            HouseType = HouseType.Baratheon
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2025, 2, 7),
                    Winner = Players.Ruslan,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Stas,
                            HouseType = HouseType.Greyjoy
                        },
                        new House
                        {
                            PlayerName = Players.Misha,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Maxim,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Dima_M,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Valya,
                            HouseType = HouseType.Baratheon
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2025, 5, 7),
                    Winner = Players.Ruslan,
                    WinType = WinType.Seven,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Dima_M,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Misha,
                            HouseType = HouseType.Greyjoy
                        },
                        new House
                        {
                            PlayerName = Players.Leha,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Sasha,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Maxim,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Baratheon
                        }
                    }
                }
            }
                .OrderBy(x => x.Date)
                .ToList();
        }

        public List<Game> Games { get; set; } 
    }

    public enum WinType
    {
        Seven,
        Score
    }
}
