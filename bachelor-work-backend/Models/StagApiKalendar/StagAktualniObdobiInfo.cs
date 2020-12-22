using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Models.StagApiKalendar
{
    public class StagAktualniObdobiInfo
    {
        public string obdobi { get; set; }
        public string akademRok { get; set; }
        public string semestrInteligentne { get; set; }
        public string akademRokInteligentne { get; set; }
        public PosledniVyucovaciDenRoku posledniVyucovaciDenRoku { get; set; }
        public PosledniDenSemestruInteligentne posledniDenSemestruInteligentne { get; set; }
        public PosledniDenZimnihoZkouskoveho posledniDenZimnihoZkouskoveho { get; set; }
        public PosledniDenLetnihoZkouskoveho posledniDenLetnihoZkouskoveho { get; set; }
        public PrvniDenStavajicihoAkademickehoRoku prvniDenStavajicihoAkademickehoRoku { get; set; }
        public PosledniDenStavajicihoAkademickehoRoku posledniDenStavajicihoAkademickehoRoku { get; set; }
    }
}

public class PosledniVyucovaciDenRoku
{
    public string value { get; set; }
}

public class PosledniDenSemestruInteligentne
{
    public string value { get; set; }
}

public class PosledniDenZimnihoZkouskoveho
{
    public string value { get; set; }
}

public class PosledniDenLetnihoZkouskoveho
{
    public string value { get; set; }
}

public class PrvniDenStavajicihoAkademickehoRoku
{
    public string value { get; set; }
}

public class PosledniDenStavajicihoAkademickehoRoku
{
    public string value { get; set; }
}
