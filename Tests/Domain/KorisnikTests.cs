using Domain.Enumeracije;
using Domain.Modeli;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class KorisnikTests
    {
        [Test]
        [TestCase("micko", "123", "Micko Lepojevic", TipKorisnika.KelarMajstor)]
        [TestCase("boban", "123", "Boban Peric", TipKorisnika.GlavniEnolog)]
        [TestCase("goran", "123", "Goran Micovic", TipKorisnika.KelarMajstor)]
        public void KonstruktorOkej(string korisnickoIme, string lozinka, string imePrezime, TipKorisnika uloga)
        {
            // Kreiranje potrebnih promenljivih za test
            Korisnik korisnik = new(korisnickoIme, lozinka, imePrezime, uloga);

            // Provera da li je ocekivani rezultat jednak rezultatu testa
            Assert.That(korisnik, Is.Not.Null);
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(korisnickoIme));
            Assert.That(korisnik.Lozinka, Is.EqualTo(lozinka));
            Assert.That(korisnik.ImePrezime, Is.EqualTo(imePrezime));
            Assert.That(korisnik.Uloga, Is.EqualTo(uloga));
            Assert.That(korisnik.Id, Is.EqualTo(0));    //podrazumevani ID je 0
        }

        [Test]
        public void DefaultKonstruktor_PostavljaPodrazumevaneVrednosti()    //testitanje konstruktora bez parametara
        {
            Korisnik korisnik = new Korisnik();

            Assert.That(korisnik.Id, Is.EqualTo(0));
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(string.Empty));
            Assert.That(korisnik.Lozinka, Is.EqualTo(string.Empty));
            Assert.That(korisnik.ImePrezime, Is.EqualTo(string.Empty));
        }

    }
}
