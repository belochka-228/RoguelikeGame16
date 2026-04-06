using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoguelikeGame.Models;

namespace RoguelikeGame.Systems
{
    public class LootSystem
    {
        public Item GenerateLoot()
        {
            int roll = RandomService.Next(0, 3);

            if (roll == 0) // Зелье
            {
                return new Potion("/Assets/UI/potion.png");
            }

            if (roll == 1) // Оружие
            {
                int weaponRoll = RandomService.Next(0, 3);

                if (weaponRoll == 0)
                    return new Weapon("Sword", 8, "/Assets/Weapons/sword.png");

                if (weaponRoll == 1)
                    return new Weapon("Axe", 12, "/Assets/Weapons/axe.png");

                return new Weapon("Spear", 6, "/Assets/Weapons/spear.png");
            }

            // Броня
            int armorRoll = RandomService.Next(0, 2);

            if (armorRoll == 0)
                return new Armor("Light Armor", 5, "/Assets/Armors/lightArmor.png");

            return new Armor("Heavy Armor", 9, "/Assets/Armors/heavyArmor.png");
        }
    }
}