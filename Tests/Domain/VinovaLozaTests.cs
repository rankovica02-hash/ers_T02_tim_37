using Domain.Enumeracije;
using Domain.Modeli;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class VinovaLozaTests
    {
        [Test]
        [TestCase(1, "Cabernet Sauvignon", 22.5f, 2022, "Toskana", FazaZrelosti.POSADJENA)]
        [TestCase(2, "Merlot", 18.0f, 2021, "Pijemont", FazaZrelosti.CVETA)]
        [TestCase(3, "Sangiovese", 20.2f, 2023, "Umbrija", FazaZrelosti.ZRENJE)]
        [TestCase(1, "Cabernet Sauvignon", 22.5f, 2022, "Toskana", FazaZrelosti.SPREMNA_ZA_BERBU)]
        [TestCase(2, "Merlot", 18.0f, 2021, "Pijemont", FazaZrelosti.OBRANA)]

        public void KonstruktorOkej(long id, string naziv, float nivoSecera, int godinaProizvodnje, string regionUzgoja, FazaZrelosti faza)
        {
            VinovaLoza loza = new(id, naziv, nivoSecera, godinaProizvodnje, regionUzgoja, faza);

            Assert.That(loza, Is.Not.Null);
            Assert.That(loza.Id, Is.EqualTo(id));
            Assert.That(loza.Naziv, Is.EqualTo(naziv));
            Assert.That(loza.NivoSecera, Is.EqualTo(nivoSecera));
            Assert.That(loza.GodinaProizvodnje, Is.EqualTo(godinaProizvodnje));
            Assert.That(loza.RegionUzgoja, Is.EqualTo(regionUzgoja));
            Assert.That(loza.Faza, Is.EqualTo(faza));
        }

        [Test]
        public void DefaultKonstruktor_PostavljaPodrazumevaneVrednosti()    //test za podrazumevani konstruktor
        {
            VinovaLoza loza = new VinovaLoza();

            Assert.That(loza.Id, Is.EqualTo(0));
            Assert.That(loza.Naziv, Is.EqualTo(string.Empty));
            Assert.That(loza.NivoSecera, Is.EqualTo(0));
            Assert.That(loza.GodinaProizvodnje, Is.EqualTo(0));
            Assert.That(loza.RegionUzgoja, Is.EqualTo(string.Empty));
        }

    }
}
