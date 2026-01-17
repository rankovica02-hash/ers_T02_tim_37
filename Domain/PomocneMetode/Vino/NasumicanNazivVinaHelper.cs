namespace Domain.PomocneMetode.Vino
{
    public static class NasumicanNazivVinaHelper
    {
        private static readonly Random random = new();

        public static readonly List<string> Nazivi = new()
        {
            "Merlot",
            "Cabernet Sauvignon",
            "Chardonnay",
            "Sauvignon Blanc",
            "Pinot Noir",
            "Pinot Grigo",
            "Pinot Blanc",
            "Riesling",
            "Syrah",
            "Malbec",
            "Prokupac",
            "Tamjanika",
            "Kadarka",
            "Tri Morave",
            "Eclater",
            "4 konja debela"
        };


        public static string GenerisiNasumicanNazivVina()
        {
            int index = random.Next(Nazivi.Count);
            return Nazivi[index];
        }
    }
}
