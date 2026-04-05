using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeGame.Models
{
    public class Enemy
    {
        public string Name;
        public int HP;
        public int Attack;
        public int Defense;

        public double CritChance;
        public double FreezeChance;

        public bool IgnoreArmor;

        public string Image;
    }
}
