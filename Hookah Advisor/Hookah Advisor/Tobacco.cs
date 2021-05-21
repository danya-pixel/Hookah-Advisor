using System.Collections.Generic;
using System.ComponentModel;

namespace Hookah_Advisor
{
    public class Tobacco
    {
        public int id { get; set; }
        public string brand { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<string> categories { get; set; }
        public List<string> tastes { get; set; }

        public Tobacco(int id, string brand, string name, string description, List<string> categories,
            List<string> tastes)
        {
            this.id = id;
            this.brand = brand;
            this.name = name;
            this.description = description;
            this.categories = categories;
            this.tastes = tastes;
        }

        public Tobacco()
        {
        }
    }
}