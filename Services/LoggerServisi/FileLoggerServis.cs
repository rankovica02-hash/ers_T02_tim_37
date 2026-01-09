using Domain.Enumeracije;
using Domain.Servisi;
using System.Globalization;

namespace Services.LoggerServisi
{
    public class FileLoggerServis(string putanja = "log.txt") : ILoggerServis
    {
        private string _putanja = putanja;

        public bool EvidentirajDogadjaj(TipEvidencije tip, string poruka)
        {
            try
            {
                string datum = DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
                using StreamWriter sw = new(_putanja, append: true);
                sw.Write($"[{tip}]: {DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture)} - {poruka}\n");

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
