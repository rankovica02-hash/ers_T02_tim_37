namespace Domain.PomocneMetode.Vino
{
    public static class NasumicanNazivVinaHelper
    {
        private static readonly Random random = new();

        private static readonly List<string> Nazivi = new()
        {
            "Merlot",
            "Cabernet Sauvignon",
            "Chardonnay",
            "Sauvignon Blanc",
            "Pinot Noir",
            "Riesling",
            "Syrah",
            "Malbec",
            "Prokupac",
            "Tamjanika"
        };


        public static string GenerisiNasumicanNazivVina()
        {
            int index = random.Next(Nazivi.Count);
            return Nazivi[index];
        }
    }
}
