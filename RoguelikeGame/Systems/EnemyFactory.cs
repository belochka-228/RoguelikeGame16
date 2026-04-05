using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoguelikeGame.Models;

namespace RoguelikeGame.Systems
{
    public class EnemyFactory
    {
        public Enemy CreateEnemy()
        {
            int type = RandomService.Next(0, 3);

            if (type == 0)
            {
                return new Enemy
                {
                    Name = "Гоблин",
                    HP = 30,
                    Attack = 12,
                    Defense = 3,
                    CritChance = 0.20,
                    Image = "/Assets/Enemies/goblin.png"
                };
            }

            if (type == 1)
            {
                return new Enemy
                {
                    Name = "Скелет",
                    HP = 40,
                    Attack = 10,
                    Defense = 5,
                    IgnoreArmor = true,
                    Image = "/Assets/Enemies/skeleton.png"
                };
            }

            return new Enemy
            {
                Name = "Маг",
                HP = 25,
                Attack = 15,
                Defense = 2,
                FreezeChance = 0.15,
                Image = "/Assets/Enemies/mage.png"
            };
        }
    }
}
