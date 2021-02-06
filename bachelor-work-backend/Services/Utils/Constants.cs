using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bachelor_work_backend.Services.Utils
{
    public  static class Constants
    {

        public static class Action
        {
            public const int BlockRequirementsCompleted = 1;
            public const int WaitingForAttendanceEvaluation = 2;
        }

        public static class Stag
        {
            public const string Aktivni = "A";
        }

        public static class StagRole
        {
            public const string Vyucujici = "VY";
            public const string Dekan = "DE";
            public const string ECTSKoordinátorPracoviste = "EP";
            public const string VedouciKatedry = "VK";

            public const string Student = "ST";

            public static readonly List<string> AdminRoles = new List<string> { Vyucujici, Dekan, ECTSKoordinátorPracoviste, VedouciKatedry };
            public static readonly List<string> StuentRoles = new List<string> { Student };
        }
    }
}
