using Domain.Modeli;
using NUnit.Framework;

namespace Tests.Domain
{
    [TestFixture]
    public class VinskiPodrumTests
    {
        [Test]
        [TestCase(1, "Podrum A", 12.5, 10)]
        [TestCase(2, "Podrum B", 15.0, 5)]
        [TestCase(3, "Lokalni Kelar", 18.0, 2)]
        public void KonstruktorOkej(long id, string naziv, double temperatura, int maxPaleta)
        {
            VinskiPodrum podrum = new VinskiPodrum(id, naziv, temperatura, maxPaleta);

            Assert.That(podrum, Is.Not.Null);
            Assert.That(podrum.Id, Is.EqualTo(id));
            Assert.That(podrum.Naziv, Is.EqualTo(naziv));
            Assert.That(podrum.TemperaturaSkladistenja, Is.EqualTo(temperatura));
            Assert.That(podrum.MaksimalanBrojPaleta, Is.EqualTo(maxPaleta));
        }

        [Test]
        public void DefaultKonstruktor_PostavljaPodrazumevaneVrednosti()
        {
            VinskiPodrum podrum = new VinskiPodrum();

            Assert.That(podrum.Id, Is.EqualTo(0));
            Assert.That(podrum.Naziv, Is.EqualTo(string.Empty));
            Assert.That(podrum.TemperaturaSkladistenja, Is.EqualTo(0));
            Assert.That(podrum.MaksimalanBrojPaleta, Is.EqualTo(0));
        }
    }
}
