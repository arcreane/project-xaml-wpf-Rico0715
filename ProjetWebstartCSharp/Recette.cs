using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetWebstartCSharp
{
    
    
        public class Recette
        {
        public int Id { get; set; }
        public int RecetteId { get; set; }

        public string Nom { get; set; }
        public List<string> Ingredients { get; set; }
        public string IngredientsDisplay => string.Join(", ", Ingredients);

        public List<int> Quantites { get; set; }

        public string Description { get; set; }

        public string CheminImage { get; set; }

        public Recette()
            {

            Ingredients = new List<string>();
            
        }

            
        }
    
}