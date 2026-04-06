using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoguelikeGame.Models;

namespace RoguelikeGame.Systems
{
    public class BossFactory
    {
        public Enemy CreateBoss()
        {
            int type = RandomService.Next(0, 4);

            if (type == 0) // ВВГ
            {
                return new Enemy
                {
                    Name = "ВВГ",
                    HP = 60,
                    Attack = 18,
                    Defense = 5,
                    CritChance = 0.30,
                    Image = "/Assets/Bosses/vvg.png"
                };
            }

            if (type == 1) // Ковальский (игнорирует броню)
            {
                return new Enemy
                {
                    Name = "Ковальский",
                    HP = 100,
                    Attack = 13,
                    Defense = 7,
                    IgnoreArmor = true,
                    Image = "/Assets/Bosses/kovalsky.png"
                };
            }

            if (type == 2) // Архимаг C++ (заморозка)
            {
                return new Enemy
                {
                    Name = "Архимаг C++",
                    HP = 45,
                    Attack = 19,
                    Defense = 3,
                    FreezeChance = 0.25,
                    Image = "/Assets/Bosses/archmage.png"
                };
            }

            // Пестов C--
            return new Enemy
            {
                Name = "Пестов C--",
                HP = 52,
                Attack = 18,
                Defense = 3,
                FreezeChance = 0.15,
                IgnoreArmor = true,
                Image = "/Assets/Bosses/pestov.png"
            };
        }
    }
}