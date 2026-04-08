using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeGame.Models
{
    //хранит состояние игрока
    public class Player
    {
        public int MaxHP = 100;
        public int HP = 100;
        public int BaseAttack = 40;   

        public Weapon Weapon; //предметы на начало игры
        public Armor Armor;
             
        public int TotalAttack => BaseAttack + (Weapon?.Attack ?? 0);//сила игрока +оружие

        public Player()//старт набор
        {
            Weapon = new Weapon("Sword", 2, "/Assets/Weapons/sword.png");
            Armor = new Armor("Light Armor", 2, "/Assets/Armors/lightArmor.png");
        }
    }
}  