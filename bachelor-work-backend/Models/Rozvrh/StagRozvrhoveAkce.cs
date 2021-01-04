using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models.Rozvrh
{
    public class StagRozvrhoveAkce
    {
        public int roakIdno { get; set; }
        public string nazev { get; set; }
        public string katedra { get; set; }
        public string predmet { get; set; }
        public object statut { get; set; }
        public int ucitIdno { get; set; }
        public Ucitel ucitel { get; set; }
        public string rok { get; set; }
        public string budova { get; set; }
        public string mistnost { get; set; }
        public int? kapacitaMistnosti { get; set; }
        public int planObsazeni { get; set; }
        public int obsazeni { get; set; }
        public string typAkce { get; set; }
        public string typAkceZkr { get; set; }
        public string semestr { get; set; }
        public string platnost { get; set; }
        public string den { get; set; }
        public string denZkr { get; set; }
        public object vyucJazyk { get; set; }
        public int hodinaOd { get; set; }
        public int hodinaDo { get; set; }
        public int? pocetVyucHodin { get; set; }
        public Hodinaskutod? hodinaSkutOd { get; set; }
        public Hodinaskutdo? hodinaSkutDo { get; set; }
        public int tydenOd { get; set; }
        public int tydenDo { get; set; }
        public string tyden { get; set; }
        public string tydenZkr { get; set; }
        public object grupIdno { get; set; }
        public string jeNadrazena { get; set; }
        public string maNadrazenou { get; set; }
        public string kontakt { get; set; }
        public object krouzky { get; set; }
        public string casovaRada { get; set; }
        public object datum { get; set; }
        public Datumod datumOd { get; set; }
        public Datumdo datumDo { get; set; }
        public string druhAkce { get; set; }
        public string vsichniUciteleUcitIdno { get; set; }
        public string vsichniUciteleJmenaTituly { get; set; }
        public string vsichniUciteleJmenaTitulySPodily { get; set; }
        public string vsichniUcitelePrijmeni { get; set; }
        public int referencedIdno { get; set; }
        public object poznamkaRozvrhare { get; set; }
        public object nekonaSe { get; set; }
        public string owner { get; set; }
        public object zakazaneAkce { get; set; }

        public class Ucitel
        {
            public int ucitIdno { get; set; }
            public string jmeno { get; set; }
            public string prijmeni { get; set; }
            public string titulPred { get; set; }
            public string titulZa { get; set; }
            public string platnost { get; set; }
            public string zamestnanec { get; set; }
            public int podilNaVyuce { get; set; }
        }

        public class Hodinaskutod
        {
            public string value { get; set; }
        }

        public class Hodinaskutdo
        {
            public string value { get; set; }
        }

        public class Datumod
        {
            public string value { get; set; }
        }

        public class Datumdo
        {
            public string value { get; set; }
        }

    }
}
