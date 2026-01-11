using Domain.Enumeracije;

namespace Domain.Modeli
{
    public class VinskiPodrum
    {
        public long Id { get; set; } = 0;
        public string Naziv {  get; set; } = string.Empty;
        public double TemperaturaSkladistenja { get; set; }
        public int MaksimalanBrojPaleta { get; set; }

        public VinskiPodrum() { }

        public VinskiPodrum (long id, string naziv, double temperaturaSkladistenja, int maksimalanBrojPaleta)
        {
            Id = id;
            Naziv = naziv;
            TemperaturaSkladistenja = temperaturaSkladistenja;
            MaksimalanBrojPaleta = maksimalanBrojPaleta;
        }
    }
}
