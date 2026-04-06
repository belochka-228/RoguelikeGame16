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

            if (roll == 0)
            {
                return new Potion("/Assets/UI/potion.png");
            }

            if (roll == 1)
            {
                int weaponRoll = RandomService.Next(0, 3);

                if (weaponRoll == 0)
                    return new Weapon("Sword", 15 , "/Assets/Weapons/sword.png");

                if (weaponRoll == 1)
                    return new Weapon("Axe", 10, "/Assets/Weapons/axe.png");

                return new Weapon("Spear", 5, "/Assets/Weapons/spear.png");
            }

            int armorRoll = RandomService.Next(0, 2);

            if (armorRoll == 0)
                return new Armor("Light Armor", 20, "/Assets/Armors/lightArmor.png");

            return new Armor("Heavy Armor", 8, "/Assets/Armors/heavyArmor.png");
        }
    }
}