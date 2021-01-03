using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models.Student
{
    public class StagStudent
    {
        public string osCislo { get; set; }
        public string jmeno { get; set; }
        public string prijmeni { get; set; }
        public object titulPred { get; set; }
        public object titulZa { get; set; }
        public string stav { get; set; }
        public object userName { get; set; }
        public string stprIdno { get; set; }
        public string nazevSp { get; set; }
        public string fakultaSp { get; set; }
        public string kodSp { get; set; }
        public string formaSp { get; set; }
        public string typSp { get; set; }
        public string typSpKey { get; set; }
        public string mistoVyuky { get; set; }
        public string rocnik { get; set; }
        public string financovani { get; set; }
        public string oborKomb { get; set; }
        public string oborIdnos { get; set; }
        public string email { get; set; }
        public object maxDobaDatum { get; set; }
        public object simsP58 { get; set; }
        public object simsP59 { get; set; }
        public object cisloKarty { get; set; }
        public string pohlavi { get; set; }
        public string rozvrhovyKrouzek { get; set; }
        public object studijniKruh { get; set; }
        public string evidovanBankovniUcet { get; set; }
        public string statutPredmetu { get; set; }
    }
}
