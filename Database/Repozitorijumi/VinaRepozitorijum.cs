using Domain.BazaPodataka;
using Domain.Modeli;
using Domain.Enumeracije;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class VinaRepozitorijum : IVinaRepozitorijum
    {
        IBazaPodataka bazaPodataka;

        public VinaRepozitorijum(IBazaPodataka bazaPodataka)
        {
            this.bazaPodataka = bazaPodataka;
        }

        public Vino DodajVino(Vino vino)
        {
            try
            {
                vino.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                vino.Sifra = $"VN-2025-{vino.Id}";    //VN-2025-ID_VINA
                bazaPodataka.Tabele.Vina.Add(vino);
                bool sacuvano = bazaPodataka.SacuvajPromene();

                if (sacuvano)
                    return vino;
                else
                    return new Vino();
            }
            catch
            {
                return new Vino();
            }
        }

        public bool AzurirajVino(Vino vino)
        {
            try
            {
                for (int i = 0; i < bazaPodataka.Tabele.Vina.Count; i++)
                {
                    if (bazaPodataka.Tabele.Vina[i].Id == vino.Id)
                    {
                        bazaPodataka.Tabele.Vina[i] = vino;
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

        public Vino? PronadjiVinoPoId(long id)
        {
            try
            {
                foreach (Vino vino in bazaPodataka.Tabele.Vina)
                {
                    if (vino.Id == id)
                    {
                        return vino;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<Vino> PronadjiVinoPoNazivu(string naziv)
        {
            try
            {
                List<Vino> rezultat = new List<Vino>();

                if (naziv == null)
                    return rezultat;

                foreach (Vino vino in bazaPodataka.Tabele.Vina)
                {
                    if (vino.Naziv != null && vino.Naziv.ToLower() == naziv.ToLower())
                    {
                        rezultat.Add(vino);
                    }
                }

                return rezultat;
            }
            catch
            {
                return new List<Vino>();
            }
        }

        public IEnumerable<Vino> PronadjiVinaPoKategoriji(KategorijaVina kategorija)
        {
            try
            {
                List<Vino> rezultat = new List<Vino>();

                foreach (Vino vino in bazaPodataka.Tabele.Vina)
                {
                    if (vino.Kategorija == kategorija)
                    {
                        rezultat.Add(vino);
                    }
                }

                return rezultat;
            }
            catch
            {
                return new List<Vino>();
            }
        }

    }
}
