using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Moq;
using NUnit.Framework;
using Services.PaletaServisi;

namespace Tests.Services.PaletaServisi
{
    [TestFixture]
    public class PaletaServisTests
    {
        private Mock<IPaletaRepozitorijum> _repo = null!;
        private Mock<ILoggerServis> _logger = null!;
        private PaletaServis _servis = null!;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<IPaletaRepozitorijum>();
            _logger = new Mock<ILoggerServis>();
            _servis = new PaletaServis(_repo.Object, _logger.Object);
        }

        // --------------------------------------------------
        // KREIRANJE PALETE
        // --------------------------------------------------

        [Test]
        public void KreiranjePalete_RepoVratiValidnuPaletu_VracaPaletuILogujeInfo()
        {
            _repo
                .Setup(r => r.DodajPaletu(It.IsAny<Paleta>()))
                .Returns((Paleta p) =>
                {
                    p.Id = 1;
                    p.Sifra = "PAL-001";
                    return p;
                });

            Paleta paleta = _servis.KreiranjePalete("Adresa", 10);

            Assert.That(paleta.Id, Is.EqualTo(1));
            Assert.That(paleta.Status, Is.EqualTo(TipStatusaPalete.UPAKOVANA));

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    "Kreirana paleta: PAL-001"
                ),
                Times.Once);
        }

        [Test]
        public void KreiranjePalete_RepoVratiPaletuBezId_VracaPraznuILogujeError()
        {
            _repo
                .Setup(r => r.DodajPaletu(It.IsAny<Paleta>()))
                .Returns(new Paleta());

            Paleta paleta = _servis.KreiranjePalete("Adresa", 1);

            Assert.That(paleta.Id, Is.EqualTo(0));

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.ERROR,
                    "Neuspešno kreiranje palete."
                ),
                Times.Once);
        }

        // --------------------------------------------------
        // PRIKAZ PALETA PO STATUSU
        // --------------------------------------------------

        [Test]
        public void PrikazPaletaPoStatusu_VracaListeIzRepoa()
        {
            var lista = new List<Paleta>
            {
                new Paleta { Id = 1, Status = TipStatusaPalete.UPAKOVANA },
                new Paleta { Id = 2, Status = TipStatusaPalete.UPAKOVANA }
            };

            _repo
                .Setup(r => r.PronadjiPaletePoStatusu(TipStatusaPalete.UPAKOVANA))
                .Returns(lista);

            var rezultat = _servis.PrikazPaletaPoStatusu(TipStatusaPalete.UPAKOVANA);

            Assert.That(rezultat.Count(), Is.EqualTo(2));
        }

        // --------------------------------------------------
        // OTPREMA PALETA
        // --------------------------------------------------

        [Test]
        public void OtpremiPalete_NevalidanBroj_VracaPraznuListuILogujeWarning()
        {
            var rezultat = _servis.OtpremiPalete(0);

            Assert.That(rezultat, Is.Empty);

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.WARNING,
                    "Otpremanje paleta: broj paleta nije validan."
                ),
                Times.Once);
        }

        [Test]
        public void OtpremiPalete_NemaUpakovanihPaleta_VracaPraznuListuILogujeWarning()
        {
            _repo
                .Setup(r => r.PronadjiPaletePoStatusu(TipStatusaPalete.UPAKOVANA))
                .Returns(new List<Paleta>());

            var rezultat = _servis.OtpremiPalete(3);

            Assert.That(rezultat, Is.Empty);

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.WARNING,
                    "Otpremanje paleta: nema UPAKOVANIH paleta."
                ),
                Times.Once);
        }

        [Test]
        public void OtpremiPalete_DovoljnoUpakovanih_PromeniStatusILogujeInfo()
        {
            var palete = new List<Paleta>
            {
                new Paleta { Id = 1, Status = TipStatusaPalete.UPAKOVANA },
                new Paleta { Id = 2, Status = TipStatusaPalete.UPAKOVANA }
            };

            _repo
                .Setup(r => r.PronadjiPaletePoStatusu(TipStatusaPalete.UPAKOVANA))
                .Returns(palete);

            _repo
                .Setup(r => r.AzurirajPaletu(It.IsAny<Paleta>()))
                .Returns(true);

            var rezultat = _servis.OtpremiPalete(2);

            Assert.That(rezultat.Count, Is.EqualTo(2));
            Assert.That(rezultat.All(p => p.Status == TipStatusaPalete.OTPREMLJENA));

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    "Otpremanje paleta: otpremljeno 2 paleta."
                ),
                Times.Once);
        }

        // --------------------------------------------------
        // PRIKAZ SVIH PALETA
        // --------------------------------------------------

        [Test]
        public void PrikazSvihPaleta_VracaSveIzRepoa()
        {
            _repo
                .Setup(r => r.PronadjiSvePalete())
                .Returns(new List<Paleta>
                {
                    new Paleta { Id = 1 },
                    new Paleta { Id = 2 }
                });

            var rezultat = _servis.PrikazSvihPaleta();

            Assert.That(rezultat.Count(), Is.EqualTo(2));
        }
    }
}
