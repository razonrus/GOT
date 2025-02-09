using System.Collections.Generic;

namespace BusinessLogic
{
    public static class Players
    {
        public static string Ruslan = "Ruslan";
        public static string Anotron = "AnotrOn";
        public static string Semen = "Semen";
        public static string Gleb = "Gleb";
        public static string Igor = "Igor";
        public static string Levch = "Levch";
        public static string Serega = "Serega";
        public static string Dimon = "Dimon";
        public static string Edele = "Edele";

        public static string Maxim = "Maxim";
        public static string Dima_M = "Dima_M";
        public static string Valya = "Valya";
        public static string Sasha = "Sasha";
        public static string Misha = "Misha";
        public static string Stas = "Stas";

        public static List<string> All()
        {
            return new List<string>
            {
                Ruslan,
                Anotron,
                Semen,
                Gleb,
                Igor,
                Levch,
                Serega,
                Dimon,
                Edele,
                Maxim,
                Valya,
                Dima_M,
                Sasha,
                Misha,
                Stas
            };
        }
    }
}
