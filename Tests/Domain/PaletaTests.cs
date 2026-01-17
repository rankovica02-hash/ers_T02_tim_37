using Domain.Enumeracije;
using Domain.Modeli;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class PaletaTests
    {
        [Test]
        [TestCase(1, "PL-001", "Rim", 10, TipStatusaPalete.UPAKOVANA)]
        [TestCase(2, "PL-002", "Firenca", 20, TipStatusaPalete.OTPREMLJENA)]
        [TestCase(3, "PL-003", "Milano", 30, TipStatusaPalete.UPAKOVANA)]
        public void KonstruktorOkej(long id, string sifra, string adresaOdredista, long vinskiPodrumId, TipStatusaPalete status)
        {
            List<long> vinaIds = new() { 101, 102, 103 };

            Paleta paleta = new(id, sifra, adresaOdredista, vinskiPodrumId, vinaIds, status);

            Assert.That(paleta, Is.Not.Null);
            Assert.That(paleta.Id, Is.EqualTo(id));
            Assert.That(paleta.Sifra, Is.EqualTo(sifra));
            Assert.That(paleta.AdresaOdredista, Is.EqualTo(adresaOdredista));
            Assert.That(paleta.VinskiPodrumId, Is.EqualTo(vinskiPodrumId));
            Assert.That(paleta.Status, Is.EqualTo(status));

            Assert.That(paleta.VinaIds, Is.Not.Null);
            Assert.That(paleta.VinaIds.Count, Is.EqualTo(vinaIds.Count));
        }

        [Test]
        public void Konstruktor_NullListaVina_KreiraPraznuListu()
        {
            Paleta paleta = new(1, "PL-004", "Napulj", 5, null, TipStatusaPalete.UPAKOVANA);

            Assert.That(paleta.VinaIds, Is.Not.Null);
            Assert.That(paleta.VinaIds.Count, Is.EqualTo(0));
        }

        [Test]
        public void DefaultKonstruktor_PostavljaPodrazumevaneVrednosti()
        {
            Paleta paleta = new Paleta();

            Assert.That(paleta.Id, Is.EqualTo(0));
            Assert.That(paleta.Sifra, Is.EqualTo(string.Empty));
            Assert.That(paleta.AdresaOdredista, Is.EqualTo(string.Empty));
            Assert.That(paleta.VinskiPodrumId, Is.EqualTo(0));
            Assert.That(paleta.VinaIds, Is.Not.Null);
            Assert.That(paleta.VinaIds.Count, Is.EqualTo(0));
        }
    }
}
