using Domain.Enumeracije;
using Domain.Konstante;
using Domain.Modeli;
using Domain.Repozitorijumi;
using Domain.Servisi;
using Moq;
using NUnit.Framework;
using Services.Proizvodnja;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Services.Proizvodnja
{
    [TestFixture]
    public class ProizvodnjeVinaServisTests
    {
        private Mock<IVinogradarstvoServis> _lozaServis = null!;
        private Mock<IVinaRepozitorijum> _vinaRepo = null!;
        private Mock<ILoggerServis> _logger = null!;
        private ProizvodnjeVinaServis _servis = null!;

        [SetUp]
        public void Setup()
        {
            _lozaServis = new Mock<IVinogradarstvoServis>();
            _vinaRepo = new Mock<IVinaRepozitorijum>();
            _logger = new Mock<ILoggerServis>();

            _servis = new ProizvodnjeVinaServis(_lozaServis.Object, _vinaRepo.Object, _logger.Object);
        }


        [Test]
        public void PocetakFermentacije_BrojFlasaNula_VracaPraznoILogujeWarning()
        {
            var rez = _servis.PocetakFermentacije(KategorijaVina.KVALITETNO_VINO, 0, 0.75, "Merlot");

            Assert.That(rez, Is.Empty);

            _logger.Verify(l => l.EvidentirajDogadjaj(
                TipEvidencije.WARNING,
                "Broj flasa mora biti veci od nule."
            ), Times.Once);

            _lozaServis.Verify(x => x.OberiLozeJedneSorte(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _vinaRepo.Verify(x => x.DodajVino(It.IsAny<Vino>()), Times.Never);
        }

        [Test]
        public void PocetakFermentacije_PrazanNazivSorte_VracaPraznoILogujeWarning()
        {
            var rez = _servis.PocetakFermentacije(KategorijaVina.STOLNO_VINO, 5, 0.75, "");

            Assert.That(rez, Is.Empty);

            _logger.Verify(l => l.EvidentirajDogadjaj(TipEvidencije.WARNING,"Naziv sorte ne sme biti prazan."), Times.Once);

            _lozaServis.Verify(x => x.OberiLozeJedneSorte(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _vinaRepo.Verify(x => x.DodajVino(It.IsAny<Vino>()), Times.Never);
        }


        [Test]
        public void PocetakFermentacije_NemaDovoljnoLoza_SadiDokNeBudeDovoljno_IProizvedeVina()
        {
            // Tražimo 2 flaše od 0.75L => ukupno 1.5L
            // LitaraPoLozi = 1.2 => potrebanBrojLoza = ceil(1.5/1.2)=2
            int brojFlasa = 2;
            double zap = 0.75;
            string sorta = "Merlot";
            var kategorija = KategorijaVina.KVALITETNO_VINO;

            // Repo vraca 1 obranu lozu (fali jos 1)
            var obrana = new VinovaLoza(1, sorta, 20f, 2024, "Toskana", FazaZrelosti.OBRANA);

            _lozaServis
                .Setup(s => s.OberiLozeJedneSorte(sorta, It.IsAny<int>()))
                .Returns(new List<VinovaLoza> { obrana });

            // Kad fali loza, sadi novu
            var nova = new VinovaLoza(2, sorta, 19f, 2024, "Toskana", FazaZrelosti.POSADJENA);
            _lozaServis
                .Setup(s => s.PosadiNovuLozu(sorta))
                .Returns(nova);

            // DodajVino u repo – vrati vino sa Id != 0
            long idCounter = 100;
            _vinaRepo
                .Setup(r => r.DodajVino(It.IsAny<Vino>()))
                .Returns((Vino v) =>
                {
                    v.Id = idCounter++;
                    v.Sifra = $"VN-2025-{v.Id}";
                    return v;
                });

            var rez = _servis.PocetakFermentacije(kategorija, brojFlasa, zap, sorta).ToList();

            Assert.That(rez.Count, Is.EqualTo(2));
            Assert.That(rez.All(v => v.Id != 0), Is.True);
            Assert.That(rez.All(v => v.Kategorija == kategorija), Is.True);
            Assert.That(rez.All(v => v.Zapremina == zap), Is.True);

            // pošto je fali loza, treba bar jednom da pozove sadnju
            _lozaServis.Verify(s => s.PosadiNovuLozu(sorta), Times.AtLeastOnce);

            // treba da doda 2 vina u repo
            _vinaRepo.Verify(r => r.DodajVino(It.IsAny<Vino>()), Times.Exactly(2));
        }

        [Test]
        public void PocetakFermentacije_LozaImaPreviseSecera_PokreceBalansiranje()
        {
            int brojFlasa = 1;
            double zap = 0.75;
            string sorta = "Merlot";
            var kategorija = KategorijaVina.PREMIJUM_VINO;

            // Obrana loza sa secerom 26.4 (> 24)
            var obrana = new VinovaLoza(1, sorta, 26.4f, 2024, "Toskana", FazaZrelosti.OBRANA);

            _lozaServis
                .Setup(s => s.OberiLozeJedneSorte(sorta, It.IsAny<int>()))
                .Returns(new List<VinovaLoza> { obrana });

            // Nova loza za balansiranje (npr 20 Brix)
            var novaZaBalans = new VinovaLoza(99, sorta, 20.0f, 2024, "Toskana", FazaZrelosti.POSADJENA);

            // PosadiNovuLozu se poziva u while(fali loza) i/ili u balansiranju.
            // Ovde nam treba da vrati neku lozu za balansiranje.
            _lozaServis
                .Setup(s => s.PosadiNovuLozu(sorta))
                .Returns(novaZaBalans);

            _vinaRepo
                .Setup(r => r.DodajVino(It.IsAny<Vino>()))
                .Returns((Vino v) =>
                {
                    v.Id = 1;
                    return v;
                });

            var rez = _servis.PocetakFermentacije(kategorija, brojFlasa, zap, sorta).ToList();

            Assert.That(rez.Count, Is.EqualTo(1));

            _lozaServis.Verify(s => s.PromeniNivoSeceraZaProcenat(
                novaZaBalans.Id,
                It.Is<float>(p => p < 0) // treba da bude negativan procenat (smanjenje)
            ), Times.AtLeastOnce);
        }



        [Test]
        public void ZahtevajProizvedenaVina_ImaDovoljnoPostojecih_VracaBezFermentacije()
        {
            var kategorija = KategorijaVina.KVALITETNO_VINO;

            var postojece = new List<Vino>
            {
                new Vino(1, "A", kategorija, 0.75, "VN-2025-1", 1, DateTime.Now),
                new Vino(2, "B", kategorija, 0.75, "VN-2025-2", 1, DateTime.Now),
                new Vino(3, "C", kategorija, 1.5, "VN-2025-3", 1, DateTime.Now),
            };

            _vinaRepo
                .Setup(r => r.PronadjiVinaPoKategoriji(kategorija))
                .Returns(postojece);

            var rez = _servis.ZahtevajProizvedenaVina(kategorija, 2, 0.75, "Merlot").ToList();

            Assert.That(rez.Count, Is.EqualTo(2));
            Assert.That(rez.All(v => v.Zapremina == 0.75), Is.True);

            // Ne treba da zove fermentaciju jer ima dovoljno
            _lozaServis.Verify(s => s.OberiLozeJedneSorte(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _vinaRepo.Verify(r => r.DodajVino(It.IsAny<Vino>()), Times.Never);
        }

        [Test]
        public void ZahtevajProizvedenaVina_NemaDovoljnoPostojecih_DopunjavaFermentacijom()
        {
            var kategorija = KategorijaVina.STOLNO_VINO;

            // u repo ima samo 1 vino odgovarajuce zapremine
            var postojece = new List<Vino>
            {
                new Vino(1, "A", kategorija, 0.75, "VN-2025-1", 1, DateTime.Now),
                new Vino(2, "B", kategorija, 1.5, "VN-2025-2", 1, DateTime.Now),
            };

            _vinaRepo
                .Setup(r => r.PronadjiVinaPoKategoriji(kategorija))
                .Returns(postojece);

            // Fermentacija treba da doda jos 2 vina (trazimo ukupno 3)
            _lozaServis
                .Setup(s => s.OberiLozeJedneSorte("Merlot", It.IsAny<int>()))
                .Returns(new List<VinovaLoza>
                {
                    new VinovaLoza(10, "Merlot", 20f, 2024, "Toskana", FazaZrelosti.OBRANA),
                    new VinovaLoza(11, "Merlot", 21f, 2024, "Toskana", FazaZrelosti.OBRANA),
                });

            long idCounter = 100;
            _vinaRepo
                .Setup(r => r.DodajVino(It.IsAny<Vino>()))
                .Returns((Vino v) =>
                {
                    v.Id = idCounter++;
                    return v;
                });

            var rez = _servis.ZahtevajProizvedenaVina(kategorija, 3, 0.75, "Merlot").ToList();

            Assert.That(rez.Count, Is.EqualTo(3));
            Assert.That(rez.Count(v => v.Zapremina == 0.75), Is.EqualTo(3));

            // fermentacija mora da se desi (jer fali)
            _vinaRepo.Verify(r => r.DodajVino(It.IsAny<Vino>()), Times.AtLeastOnce);
        }
    }
}
