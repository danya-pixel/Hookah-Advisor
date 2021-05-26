using System.Collections.Generic;
using System.ComponentModel;

namespace Hookah_Advisor
{
    public class Tobacco
    {
        public int Id { get; }
        public string Brand { get; }
        public string Name { get; } //should be public for Serializer
        public string Description { get; }
        public List<string> Categories { get; }
        public List<string> Tastes { get; }

        public Tobacco(int id, string brand, string name, string description, List<string> categories,
            List<string> tastes)
        {
            Id = id;
            Brand = brand;
            Name = name;
            Description = description;
            Categories = categories;
            Tastes = tastes;
        }

        public override string ToString()
        {
            return $"{Brand}: {Name}";
        }
    }
}