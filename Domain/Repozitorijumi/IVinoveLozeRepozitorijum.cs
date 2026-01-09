using Domain.Enumeracije;
using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IVinoveLozeRepozitorijum
    {
        public VinovaLoza DodajVinovuLozu(VinovaLoza vinovaloza);
        public VinovaLoza PronadjiVinovuLozuPoId(long id);
        public bool AzurirajVinovuLozu(VinovaLoza vinovaloza);
        public IEnumerable<VinovaLoza> PronadjiVinoveLozePoNazivu(string naziv);
    }
}
