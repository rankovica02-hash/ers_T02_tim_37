using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class VinskiPodrumiRepozitorijum : IVinskiPodrumRepozitorijum
    {
        IBazaPodataka bazaPodataka;

        public VinskiPodrumiRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }

        public VinskiPodrum DodajVinskiPodrum(VinskiPodrum podrum)
        {
            try
            {
                podrum.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                bazaPodataka.Tabele.VinskiPodrumi.Add(podrum);
                bool sacuvano = bazaPodataka.SacuvajPromene();

                if (sacuvano)
                    return podrum;
                else
                    return new VinskiPodrum();
            }
            catch
            {
                return new VinskiPodrum(); 
            }
        }

        public bool AzurirajVinskiPodrum(VinskiPodrum podrum)
        {
            try
            {
                for(int i = 0; i< bazaPodataka.Tabele.VinskiPodrumi.Count; i++)
                {
                    if (bazaPodataka.Tabele.VinskiPodrumi[i].Id == podrum.Id)
                    {
                        bazaPodataka.Tabele.VinskiPodrumi[i] = podrum;
                        return bazaPodataka.SacuvajPromene();
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public VinskiPodrum PronadjiVinskiPodrumPoId(long id)
        {
            try
            {
                foreach (VinskiPodrum podrum in bazaPodataka.Tabele.VinskiPodrumi)
                {
                    if(podrum.Id == id)
                    {
                        return podrum;
                    }
                }
               return new VinskiPodrum();
            }
            catch
            {
                return new VinskiPodrum();
            }
        }

        public IEnumerable<VinskiPodrum> PronadjiSveVinskePodrume()
        {
            try
            {
                return bazaPodataka.Tabele.VinskiPodrumi;
            }
            catch
            {
                return new List<VinskiPodrum>();
            }
        }
    }
}
