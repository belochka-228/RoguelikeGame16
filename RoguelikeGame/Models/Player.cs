using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeGame.Models
{
    public class Player
    {
        public int MaxHP = 100;
        public int HP = 100;
        public int BaseAttack = 3; 

        public Weapon Weapon;
        public Armor Armor;

        public int TotalAttack => BaseAttack + (Weapon?.Attack ?? 0);

        public Player()
        {
            Weapon = new Weapon("Sword", 6, "/Assets/Weapons/sword.png");
            Armor = new Armor("Light Armor", 4, "/Assets/Armors/lightArmor.png");
        }
    }
}