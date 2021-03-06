﻿using System;
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
                    Date = new DateTime(2016, 8, 12),
                    Winner = Players.Ruslan,
                    WinType = WinType.Seven,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Greyjoy
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Baratheon
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 8, 19),
                    Winner = Players.Semen,
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
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Greyjoy
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Levch,
                            HouseType = HouseType.Baratheon
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 8, 26),
                    Winner = Players.Ruslan,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 9, 2),
                    Winner = Players.Semen,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 9, 8),
                    Winner = Players.Ruslan,
                    WinType = WinType.Seven,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 9, 14),
                    Winner = Players.Igor,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 9, 23),
                    Winner = Players.Gleb,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 9, 24),
                    Winner = Players.Gleb,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Dimon,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 9, 30),
                    Winner = Players.Ruslan,
                    WinType = WinType.Seven,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 10, 7),
                    Winner = Players.Igor,
                    WinType = WinType.Seven,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 10, 9),
                    Winner = Players.Serega,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Edele,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 10, 14),
                    Winner = Players.Ruslan,
                    WinType = WinType.Seven,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 10, 21),
                    Winner = Players.Serega,
                    WinType = WinType.Score,
                    WithRandomCards = true,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Greyjoy
                        }
                    }
                },
                new Game
                {
                    Date = new DateTime(2016, 10, 27),
                    Winner = Players.Serega,
                    WinType = WinType.Score,
                    Houses = new List<House>
                    {
                        new House
                        {
                            PlayerName = Players.Gleb,
                            HouseType = HouseType.Lannister
                        },
                        new House
                        {
                            PlayerName = Players.Igor,
                            HouseType = HouseType.Tyrell
                        },
                        new House
                        {
                            PlayerName = Players.Serega,
                            HouseType = HouseType.Martell
                        },
                        new House
                        {
                            PlayerName = Players.Anotron,
                            HouseType = HouseType.Baratheon
                        },
                        new House
                        {
                            PlayerName = Players.Ruslan,
                            HouseType = HouseType.Stark
                        },
                        new House
                        {
                            PlayerName = Players.Semen,
                            HouseType = HouseType.Greyjoy
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
