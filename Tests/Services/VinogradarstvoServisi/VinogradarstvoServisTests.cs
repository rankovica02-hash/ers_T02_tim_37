using Domain.Enumeracije;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Moq;
using NUnit.Framework;
using Services.VinogradarstvoServisi;
using System.Collections.Generic;

namespace Tests.Services.VinogradarstvoServisi
{
    [TestFixture]
    public class VinogradarstvoServisTests
    {
        private Mock<IVinoveLozeRepozitorijum> _repo = null!;
        private Mock<ILoggerServis> _logger = null!;
        private VinogradarstvoServis _servis = null!;

        [SetUp]
        public void Setup()
        {
            _repo = new Mock<IVinoveLozeRepozitorijum>();
            _logger = new Mock<ILoggerServis>();
            _servis = new VinogradarstvoServis(_repo.Object, _logger.Object);
        }

        // --------------------------------------------------
        // POSADI NOVU LOZU
        // --------------------------------------------------

        [Test]
        public void PosadiNovuLozu_PrazanNaziv_VracaPraznuLozuILogujeWarning()
        {
            VinovaLoza loza = _servis.PosadiNovuLozu("");

            Assert.That(loza.Id, Is.EqualTo(0));

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.WARNING,
                    "Pokušaj sadnje loze sa praznim nazivom sorte."
                ),
                Times.Once);

            _repo.Verify(r => r.DodajVinovuLozu(It.IsAny<VinovaLoza>()), Times.Never);
        }

        [Test]
        public void PosadiNovuLozu_IspravanNaziv_VracaLozuILogujeInfo()
        {
            _repo
                .Setup(r => r.DodajVinovuLozu(It.IsAny<VinovaLoza>()))
                .Returns((VinovaLoza v) =>
                {
                    v.Id = 1;
                    return v;
                });

            VinovaLoza loza = _servis.PosadiNovuLozu("Merlot");

            Assert.That(loza.Id, Is.EqualTo(1));
            Assert.That(loza.Faza, Is.EqualTo(FazaZrelosti.POSADJENA));

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    It.Is<string>(s => s.Contains("Zasađena nova loza"))
                ),
                Times.Once);
        }

        // --------------------------------------------------
        // PROMENI NIVO SECERA
        // --------------------------------------------------

        [Test]
        public void PromeniNivoSecera_NepostojecaLoza_VracaFalseILogujeWarning()
        {
            _repo
                .Setup(r => r.PronadjiVinovuLozuPoId(1))
                .Returns(new VinovaLoza());

            bool ok = _servis.PromeniNivoSeceraZaProcenat(1, 10);

            Assert.That(ok, Is.False);

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.WARNING,
                    "Ne postoji vinova loza sa tim ID-em."
                ),
                Times.Once);
        }

        [Test]
        public void PromeniNivoSecera_IspravnaLoza_AzuriraIVracaTrue()
        {
            VinovaLoza loza = new VinovaLoza(1, "Cabernet", 20f, 2024, "Region", FazaZrelosti.CVETA);

            _repo
                .Setup(r => r.PronadjiVinovuLozuPoId(1))
                .Returns(loza);

            _repo
                .Setup(r => r.AzurirajVinovuLozu(It.IsAny<VinovaLoza>()))
                .Returns(true);

            bool ok = _servis.PromeniNivoSeceraZaProcenat(1, 10);

            Assert.That(ok, Is.True);
            Assert.That(loza.NivoSecera, Is.GreaterThan(20f));

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    It.IsAny<string>()
                ),
                Times.Once);
        }

        // --------------------------------------------------
        // OBERI LOZE
        // --------------------------------------------------

        [Test]
        public void OberiLoze_NeispravanUlaz_VracaPraznuListuILogujeWarning()
        {
            var rezultat = _servis.OberiLozeJedneSorte("", 0);

            Assert.That(rezultat, Is.Empty);

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.WARNING,
                    It.IsAny<string>()
                ),
                Times.Once);
        }

        [Test]
        public void OberiLoze_DovoljnoLoza_VracaTacanBrojIPromeniFazu()
        {
            var loze = new List<VinovaLoza>
            {
                new VinovaLoza(1, "Merlot", 20, 2024, "Region", FazaZrelosti.SPREMNA_ZA_BERBU),
                new VinovaLoza(2, "Merlot", 21, 2024, "Region", FazaZrelosti.SPREMNA_ZA_BERBU),
                new VinovaLoza(3, "Merlot", 22, 2024, "Region", FazaZrelosti.SPREMNA_ZA_BERBU),
            };

            _repo
                .Setup(r => r.PronadjiVinoveLozePoNazivu("Merlot"))
                .Returns(loze);

            _repo
                .Setup(r => r.AzurirajVinovuLozu(It.IsAny<VinovaLoza>()))
                .Returns(true);

            var rezultat = _servis.OberiLozeJedneSorte("Merlot", 2);

            Assert.That(rezultat.Count(), Is.EqualTo(2));
            Assert.That(rezultat.All(l => l.Faza == FazaZrelosti.OBRANA));

            _logger.Verify(l =>
                l.EvidentirajDogadjaj(
                    TipEvidencije.INFO,
                    It.Is<string>(s => s.Contains("Uspešno obrane"))
                ),
                Times.Once);
        }
    }
}
