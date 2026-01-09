using Domain.BazaPodataka;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class VinoveLozeRepozitorijum : IVinoveLozeRepozitorijum
    {
        IBazaPodataka bazaPodataka;

        public VinoveLozeRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }

        public VinovaLoza DodajVinovuLozu(VinovaLoza vinovaloza)
        {
            try
            {
                vinovaloza.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                bazaPodataka.Tabele.VinoveLoze.Add(vinovaloza);
                bool sacuvano = bazaPodataka.SacuvajPromene();

                if (sacuvano)
                    return vinovaloza;
                else
                    return new VinovaLoza();
            }
            catch
            {
                return new VinovaLoza();
            }
        }

        public bool AzurirajVinovuLozu(VinovaLoza vinovaloza)
        {
            try
            {
                for (int i = 0; i < bazaPodataka.Tabele.VinoveLoze.Count; i++)
                {
                    if (bazaPodataka.Tabele.VinoveLoze[i].Id == vinovaloza.Id)
                    {
                        bazaPodataka.Tabele.VinoveLoze[i] = vinovaloza;
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

        public VinovaLoza PronadjiVinovuLozuPoId(long id)
        {
            try
            {
                foreach (VinovaLoza vinovaloza in bazaPodataka.Tabele.VinoveLoze)
                {
                    if (vinovaloza.Id == id)
                    {
                        return vinovaloza;
                    }
                }

                return new VinovaLoza();
            }
            catch
            {
                return new VinovaLoza();
            }
        }


        public IEnumerable<VinovaLoza> PronadjiVinoveLozePoNazivu(string naziv)
        {
            try
            {
                List<VinovaLoza> rezultat = new List<VinovaLoza>();

                if (naziv == null)
                    return rezultat;

                foreach (VinovaLoza vinovaloza in bazaPodataka.Tabele.VinoveLoze)
                {
                    if (vinovaloza.Naziv != null && vinovaloza.Naziv.ToLower() == naziv.ToLower())
                    {
                        rezultat.Add(vinovaloza);
                    }
                }

                return rezultat;
            }
            catch
            {
                return new List<VinovaLoza>();
            }
        }

    }
}
