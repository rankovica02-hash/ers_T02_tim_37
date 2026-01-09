namespace Domain.PomocneMetode.VinovaLoza
{
    public static class NasumicanRegionUzgojaHelper
    {
        private static readonly Random random = new();

        private static readonly List<string> Regioni = new()
        {
            "Šumadija",
            "Fruška Gora",
            "Župa",
            "Negotinska krajina",
            "Južna Srbija",
            "Zapadna Srbija",
            "Valjevski",
            "Metohija",
            "Banat"
        };

        public static string GenerisiNasumicanRegionUzgoja()
        {
            int index = random.Next(Regioni.Count);
            return Regioni[index];
        }
    }
}
