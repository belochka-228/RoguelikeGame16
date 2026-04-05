using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeGame.Models
{
    public class Armor : Item
    {
        public int Defense;

        public Armor(string name, int defense, string image)
            : base(name, image)
        {
            Defense = defense;
        }
    }
}
