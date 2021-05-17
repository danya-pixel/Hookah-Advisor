using System.Collections.Generic;
using System.ComponentModel;

namespace Hookah_Advisor
{
    public class Tobacco
    {
        private int id { get; set; }
        public string name { get; set; }
        private string description { get; set; }
        private List<string> categories { get; set; }
        public List<string> tastes { get; set; }

        public Tobacco(int id, string name, string description, List<string> categories, List<string> tastes)
        {
            this.id = id;
            this.name = name;
            this.description = description;
            this.categories = categories;
            this.tastes = tastes;
        }
    }
}