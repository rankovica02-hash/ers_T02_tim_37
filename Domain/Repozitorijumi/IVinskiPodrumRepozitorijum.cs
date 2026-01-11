using Domain.Enumeracije;
using Domain.Modeli;

namespace Domain.Repozitorijumi
{
    public interface IVinskiPodrumRepozitorijum
    {
       public VinskiPodrum DodajVinskiPodrum(VinskiPodrum podrum);
        public VinskiPodrum PronadjiVinskiPodrumPoId(long id);
        public IEnumerable<VinskiPodrum> PronadjiSveVinskePodrume();
        bool AzurirajVinskiPodrum(VinskiPodrum podrum);
    }
}
