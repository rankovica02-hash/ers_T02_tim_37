using Domain.BazaPodataka;
using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;

namespace Database.Repozitorijumi
{
    public class PaletaRepozitorijum : IPaletaRepozitorijum
    {
        IBazaPodataka bazaPodataka;
        public PaletaRepozitorijum(IBazaPodataka baza)
        {
            bazaPodataka = baza;
        }
        
        public Paleta DodajPaletu(Paleta paleta)
        {
            try
            {
                paleta.Id = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                paleta.Sifra = $"PA-2025-{paleta.Id}";

                if (paleta.Status.Equals(default(TipStatusaPalete)))
                    paleta.Status = TipStatusaPalete.UPAKOVANA;

                bazaPodataka.Tabele.Palete.Add(paleta);
                bool uspesno = bazaPodataka.SacuvajPromene();

                if (uspesno)
                    return paleta;
                else
                    return new Paleta();
            
            }
            catch
            {
                return new Paleta();
            }
        }

        public Paleta PronadjiPaletuPoId(long id)
        {
            try
            {
                foreach(var paleta in bazaPodataka.Tabele.Palete)
                {
                    if (paleta.Id == id)
                        return paleta;
                }
                return new Paleta();
            }
            catch
            {
                return new Paleta();
            }
        }

        public IEnumerable<Paleta> PronadjiSvePalete()
        {
            try
            {
                return bazaPodataka.Tabele.Palete;
            }
            catch
            {
                return [];
            }
        }
        public IEnumerable<Paleta> PronadjiPaletePoStatusu(TipStatusaPalete status)
        {
            try
            {
                List<Paleta> rezultat = [];
                foreach(var paleta in bazaPodataka.Tabele.Palete)
                {
                    if (paleta.Status == status)
                        rezultat.Add(paleta);
                }
                return rezultat;
            }
            catch
            {
                return [];
            }
        }

        public bool AzurirajPaletu(Paleta paleta)
        {
            try
            {
                for (int i = 0; i < bazaPodataka.Tabele.Palete.Count; i++)
                {
                    if (bazaPodataka.Tabele.Palete[i].Id == paleta.Id)
                    {
                        bazaPodataka.Tabele.Palete[i] = paleta;
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


    }
}
