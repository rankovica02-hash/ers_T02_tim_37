namespace Domain.PomocneMetode.VinovaLoza
{
    public static class NasumicanNazivVinoveLozeHelper
    {
        private static readonly Random random = new();

        private static readonly List<string> Nazivi = new()
        {
            "Prokupac",
            "Vranac",
            "Tamjanika",
            "Cabernet Sauvignon",
            "Merlot",
            "Chardonnay",
            "Sauvignon Blanc",
            "Riesling",
            "Pinot Noir",
            "Muscat"
        };


        public static string GenerisiNasumicanNazivVinoveLoze()
        {
            int index = random.Next(Nazivi.Count);
            return Nazivi[index];
        }
    }
}
