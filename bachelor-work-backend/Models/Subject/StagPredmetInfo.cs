using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models.Subject
{
    public class StagPredmetInfo
    {
        public string katedra { get; set; }
        public string zkratka { get; set; }
        public string rok { get; set; }
        public string nazev { get; set; }
        public string nazevDlouhy { get; set; }
        public string vyukaZS { get; set; }
        public string vyukaLS { get; set; }
        public int kreditu { get; set; }
        public string viceZapis { get; set; }
        public int minObsazeni { get; set; }
        public string garanti { get; set; }
        public string garantiSPodily { get; set; }
        public string garantiUcitIdno { get; set; }
        public string prednasejici { get; set; }
        public string prednasejiciSPodily { get; set; }
        public string prednasejiciUcitIdno { get; set; }
        public string cvicici { get; set; }
        public string cviciciSPodily { get; set; }
        public string cviciciUcitIdno { get; set; }
        public string seminarici { get; set; }
        public string seminariciSPodily { get; set; }
        public string seminariciUcitIdno { get; set; }
        public string podminujiciPredmety { get; set; }
        public string vylucujiciPredmety { get; set; }
        public string podminujePredmety { get; set; }
        public string literatura { get; set; }
        public string nahrazPredmety { get; set; }
        public string metodyVyucovaci { get; set; }
        public string metodyHodnotici { get; set; }
        public string akreditovan { get; set; }
        public int jednotekPrednasek { get; set; }
        public string jednotkaPrednasky { get; set; }
        public int jednotekCviceni { get; set; }
        public string jednotkaCviceni { get; set; }
        public int jednotekSeminare { get; set; }
        public string jednotkaSeminare { get; set; }
        public string anotace { get; set; }
        public string typZkousky { get; set; }
        public string maZapocetPredZk { get; set; }
        public string formaZkousky { get; set; }
        public string pozadavky { get; set; }
        public string prehledLatky { get; set; }
        public string predpoklady { get; set; }
        public string ziskaneZpusobilosti { get; set; }
        public string casovaNarocnost { get; set; }
        public object predmetUrl { get; set; }
        public string vyucovaciJazyky { get; set; }
        public object poznamka { get; set; }
        public string ectsZobrazit { get; set; }
        public string ectsAkreditace { get; set; }
        public string ectsNabizetUPrijezdu { get; set; }
        public object poznamkaVerejna { get; set; }
        public object skupinaAkreditace { get; set; }
        public object skupinaAkreditaceKey { get; set; }
        public string zarazenDoPrezencnihoStudia { get; set; }
        public string zarazenDoKombinovanehoStudia { get; set; }
        public object studijniOpory { get; set; }
        public string praxePocetDnu { get; set; }
        public object urovenNastavena { get; set; }
        public string urovenVypoctena { get; set; }
        public string automatickyUznavatZppZk { get; set; }
        public object hodZaSemKombForma { get; set; }

    }
}
