using System;
using System.Collections.Generic;
using System.Text;

namespace ML.Net
{
    [Serializable]
    public class GameResult
    {
        public string code { get; set; }
        public string IsWinner { get; set; }
        public string HaveArmor { get; set; }
        public string HaveWeapon { get; set; }
        public string HaveBoots { get; set; }
    }

    [Serializable]
    public class OutputResult
    {
        public string code { get; set; }
        public string HaveArmor { get; set; }
        public string HaveWeapon { get; set; }
        public string HaveBoots { get; set; }
    }
}
