using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeGame.Models
{
    public class Weapon : Item
    {
        public int Attack;

        public Weapon(string name, int attack, string image)
            : base(name, image)
        {
            Attack = attack;
        }
    }
}