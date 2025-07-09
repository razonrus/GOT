using System.Collections.Generic;

namespace BusinessLogic
{
    public static class Players
    {
        public static string Ruslan = "@ruslan.safin";

        public static string Anotron = "AnotrOn";
        public static string Semen = "Semen";
        public static string Gleb = "Gleb";
        public static string Igor = "Igor";
        public static string Levch = "Levch";
        public static string Serega = "Serega";
        public static string Dimon = "Dimon";
        public static string Edele = "Edele";

        public static string Maxim = "@maximmadasov";
        public static string Dima_M = "Dima_M";
        public static string Valya = "Valya";
        public static string Sasha = "Sasha";
        public static string Misha = "@aizek";
        public static string Stas = "Stas";
        public static string Leha = "@lavrovaleksey";
        public static string Andrey = "Andrey";
        public static string Serega_B = "@sergei.bondarenko";

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
                Stas,
                Leha,
                Andrey,
                Serega_B
            };
        }
    }
}
