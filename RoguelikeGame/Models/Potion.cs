using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeGame.Models
{
    public class Potion : Item
    {
        public Potion(string image)
            : base("Health Potion", image)
        {
        }
    }
}
 