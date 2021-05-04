using System.Collections.Generic;
using System.ComponentModel;

namespace Hookah_Advisor
{
    public class Tobacco
    {
        private string id { get; set; }
        private string name { get; set; }
        private string description { get; set; }
        private List<string> categories{ get; set; }
        public List<string> tastes{ get; set; }
    }
}