using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Moq;
using NUnit.Framework;
using Services.AutenftikacioniServisi;

namespace Tests.Services.AutenftikacioniServisi
{
    [TestFixture]
    public class AutentifikacioniServisTests
    {
        private Mock<IKorisniciRepozitorijum> _korisniciRepo = null!;
        private Mock<ILoggerServis> _logger = null!;
        private AutentifikacioniServis _servis = null!; //null su samo da C# ne bi bacao warning, bice inicijalizovano u Setup

        [SetUp]
        public void Setup()
        {
            _korisniciRepo = new Mock<IKorisniciRepozitorijum>();
            _logger = new Mock<ILoggerServis>();
            _servis = new AutentifikacioniServis(_korisniciRepo.Object, _logger.Object);
        }

        // --------------------
        // PRIJAVA
        // --------------------

        [Test]
        [TestCase("mare123", "sifra123")]
        [TestCase("pera321", "sifra321")]
        public void Prijava_IspravniPodaci_VracaTrueIKorisnika(string korisnickoIme, string lozinka)
        {
            var pronadjen = new Korisnik(korisnickoIme, lozinka, "Ime Prezime", TipKorisnika.GlavniEnolog);

            _korisniciRepo
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu(korisnickoIme))
                .Returns(pronadjen);

            var (ok, korisnik) = _servis.Prijava(korisnickoIme, lozinka);

            Assert.That(ok, Is.True);
            Assert.That(korisnik, Is.Not.Null);
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(korisnickoIme));
            Assert.That(korisnik.Lozinka, Is.EqualTo(lozinka));

            _logger.Verify(l => l.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    $"Korisnik '{korisnickoIme}' je uspešno prijavljen."
                ),
                Times.Once);
        }

        [Test]
        [TestCase("nepostoji", "123")]
        [TestCase("ghost", "pass")]
        public void Prijava_KorisnikNePostoji_VracaFalseIPraznogKorisnika(string korisnickoIme, string lozinka)
        {
            // repo vraca "praznog" korisnika (signal da korisnik ne postoji)
            _korisniciRepo
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu(korisnickoIme))
                .Returns(new Korisnik());

            var (ok, korisnik) = _servis.Prijava(korisnickoIme, lozinka);

            Assert.That(ok, Is.False);
            Assert.That(korisnik, Is.Not.Null);
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(string.Empty));

            _logger.Verify(l => l.EvidentirajDogadjaj(
                    TipEvidencije.WARNING,
                    $"Neuspešna prijava za korisnika '{korisnickoIme}'."
                ),
                Times.Once);
        }

        [Test]
        [TestCase("mare123", "pogresna")]
        [TestCase("pera321", "xyz")]
        public void Prijava_PogresnaLozinka_VracaFalseIPraznogKorisnika(string korisnickoIme, string pogresnaLozinka)
        {
            // korisnik postoji ali mu je lozinka drugacija
            var pronadjen = new Korisnik(korisnickoIme, "ispravna", "Ime Prezime", TipKorisnika.KelarMajstor);

            _korisniciRepo
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu(korisnickoIme))
                .Returns(pronadjen);

            var (ok, korisnik) = _servis.Prijava(korisnickoIme, pogresnaLozinka);

            Assert.That(ok, Is.False);
            Assert.That(korisnik, Is.Not.Null);
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(string.Empty));

            _logger.Verify(l => l.EvidentirajDogadjaj(
                    TipEvidencije.WARNING,
                    $"Neuspešna prijava za korisnika '{korisnickoIme}'."
                ),
                Times.Once);
        }

        [Test]
        public void Prijava_RepoBaciIzuzetak_VracaFalseIPraznogKorisnikaILogujeError()
        {
            string korisnickoIme = "mare123";
            string lozinka = "sifra123";

            _korisniciRepo
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu(korisnickoIme))
                .Throws(new Exception("boom"));

            var (ok, korisnik) = _servis.Prijava(korisnickoIme, lozinka);

            Assert.That(ok, Is.False);
            Assert.That(korisnik, Is.Not.Null);
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(string.Empty));

            _logger.Verify(l => l.EvidentirajDogadjaj(
                    TipEvidencije.ERROR,
                    $"Greska tokom prijave korisnika '{korisnickoIme}'."
                ),
                Times.Once);
        }

        // --------------------
        // REGISTRACIJA
        // --------------------

        [Test]
        public void Registracija_NevalidanUnos_VracaFalseIPraznogKorisnika_NeZoveRepo()
        {
            // prazan korisnicko ime / lozinka / imeprezime
            var novi = new Korisnik("", "123", "Ime Prezime", TipKorisnika.GlavniEnolog);

            var (ok, korisnik) = _servis.Registracija(novi);

            Assert.That(ok, Is.False);
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(string.Empty));

            // ne treba ni da proverava postojanje, ni da dodaje
            _korisniciRepo.Verify(r => r.PronadjiKorisnikaPoKorisnickomImenu(It.IsAny<string>()), Times.Never);
            _korisniciRepo.Verify(r => r.DodajKorisnika(It.IsAny<Korisnik>()), Times.Never);
        }

        [Test]
        public void Registracija_KorisnikVecPostoji_VracaFalseIPraznogKorisnikaILogujeWarning()
        {
            var novi = new Korisnik("mare123", "sifra123", "Marko Markovic", TipKorisnika.GlavniEnolog);

            _korisniciRepo
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu(novi.KorisnickoIme))
                .Returns(new Korisnik("mare123", "x", "Vec Postoji", TipKorisnika.KelarMajstor));

            var (ok, korisnik) = _servis.Registracija(novi);

            Assert.That(ok, Is.False);
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(string.Empty));

            _logger.Verify(l => l.EvidentirajDogadjaj(
                    TipEvidencije.WARNING,
                    $"Neuspešna registracija - korisnik '{novi.KorisnickoIme}' već postoji."
                ),
                Times.Once);

            _korisniciRepo.Verify(r => r.DodajKorisnika(It.IsAny<Korisnik>()), Times.Never);
        }

        [Test]
        public void Registracija_NoviKorisnik_VracaTrueIDodatogKorisnikaILogujeInfo()
        {
            var novi = new Korisnik("novi1", "pass", "Novi Korisnik", TipKorisnika.KelarMajstor);

            _korisniciRepo
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu(novi.KorisnickoIme))
                .Returns(new Korisnik()); // ne postoji

            _korisniciRepo
                .Setup(r => r.DodajKorisnika(novi))
                .Returns(novi);

            var (ok, korisnik) = _servis.Registracija(novi);

            Assert.That(ok, Is.True);
            Assert.That(korisnik, Is.Not.Null);
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(novi.KorisnickoIme));

            _logger.Verify(l => l.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    $"Korisnik '{novi.KorisnickoIme}' je uspešno registrovan."
                ),
                Times.Once);

            _korisniciRepo.Verify(r => r.DodajKorisnika(novi), Times.Once);
        }

        [Test]
        public void Registracija_RepoBaciIzuzetak_VracaFalseILogujeError()
        {
            var novi = new Korisnik("novi1", "pass", "Novi Korisnik", TipKorisnika.GlavniEnolog);

            _korisniciRepo
                .Setup(r => r.PronadjiKorisnikaPoKorisnickomImenu(novi.KorisnickoIme))
                .Throws(new Exception("boom"));

            var (ok, korisnik) = _servis.Registracija(novi);

            Assert.That(ok, Is.False);
            Assert.That(korisnik.KorisnickoIme, Is.EqualTo(string.Empty));

            _logger.Verify(l => l.EvidentirajDogadjaj(
                    TipEvidencije.ERROR,
                    $"Greska tokom registracije korisnika '{novi.KorisnickoIme}'."
                ),
                Times.Once);
        }
    }
}
