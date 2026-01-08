using Domain.BazaPodataka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Database.BazaPodataka
{
    public class XMLBazaPodataka : IBazaPodataka
    {
        public TabeleBazaPodataka Tabele {  get; set; }

    public XMLBazaPodataka()
        {
            try
            {
                if (File.Exists("podaci.xml"))
                {
                    using StreamReader sr = new("podaci.xml");
                    string xml = sr.ReadToEnd();
                    XmlSerializer serializer = new XmlSerializer(typeof(TabeleBazaPodataka));
                    using StringReader citac = new(xml);

                    Tabele = (TabeleBazaPodataka?)serializer.Deserialize(citac) ?? new();
                    
                }
                else
                {
                    Tabele = new();
                }
            }
            catch
            {
                Tabele = new();
            }

        }
    public bool SacuvajPromene()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TabeleBazaPodataka));

                using StreamWriter sw = new("podaci.xml");

                serializer.Serialize(sw, Tabele);
                return true;
            }
            catch
            {
                return false; 
            }
        }

    }
}
