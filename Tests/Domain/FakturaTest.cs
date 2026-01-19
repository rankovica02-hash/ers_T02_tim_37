using Domain.Enumeracije;
using Domain.Modeli;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests.Domain
{
    [TestFixture]
    public class FakturaTests
    {
        [Test]
        public void DefaultKonstruktor_PostavljaPodrazumevaneVrednosti()
        {
            Faktura faktura = new Faktura();

            Assert.That(faktura, Is.Not.Null);
            Assert.That(faktura.Id, Is.EqualTo(0));
            Assert.That(faktura.Stavke, Is.Not.Null);
            Assert.That(faktura.Stavke.Count, Is.EqualTo(0));

            Assert.That(faktura.DatumIzdavanja, Is.Not.EqualTo(default(DateTime)));
        }

        [Test]
        public void UkupanIznos_BezStavki_JeNula()
        {
            Faktura faktura = new Faktura();

            Assert.That(faktura.UkupanIznos, Is.EqualTo(0m));
        }

        [Test]
        public void UkupanIznos_RacunaSeIspravnoSaJednomStavkom()
        {
            Faktura faktura = new Faktura();
            faktura.Stavke.Add(new StavkeFakture(
                vinoId: 1,
                naziv: "Vranac",
                kolicina: 2,
                cenaPoFlasi: 1500m
            ));

            Assert.That(faktura.UkupanIznos, Is.EqualTo(3000m));
        }

        [Test]
        public void UkupanIznos_RacunaSeIspravnoSaViseStavki()
        {
            Faktura faktura = new Faktura();
            faktura.Stavke = new List<StavkeFakture>
            {
                new StavkeFakture(1, "Vranac", 2, 1000m),   // 2000
                new StavkeFakture(2, "Cabernet", 1, 2500m) // 2500
            };

            Assert.That(faktura.UkupanIznos, Is.EqualTo(4500m));
        }
    }
}
