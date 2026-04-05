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

            if (type == 0)
            {
                return new Enemy
                {
                    Name = "ВВГ",
                    HP = (int)(30 * 2.0),
                    Attack = (int)(12 * 1.5),
                    Defense = (int)(3 * 1.2),
                    CritChance = 0.30,
                    Image = "/Assets/Bosses/vvg.png"
                };
            }

            if (type == 1)
            {
                return new Enemy
                {
                    Name = "Ковальский",
                    HP = (int)(40 * 2.5),
                    Attack = (int)(10 * 1.3),
                    Defense = (int)(5 * 1.4),
                    IgnoreArmor = true,
                    Image = "/Assets/Bosses/kovalsky.png"
                };
            }

            if (type == 2)
            {
                return new Enemy
                {
                    Name = "Архимаг C++",
                    HP = (int)(25 * 1.8),
                    Attack = (int)(15 * 1.6),
                    Defense = (int)(2 * 1.1),
                    FreezeChance = 0.25,
                    Image = "/Assets/Bosses/archmage.png"
                };
            }

            return new Enemy
            {
                Name = "Пестов C--",
                HP = (int)(40 * 1.3),
                Attack = (int)(10 * 1.8),
                Defense = (int)(5 * 0.6),
                FreezeChance = 0.15,
                IgnoreArmor = true,
                Image = "/Assets/Bosses/pestov.png"
            };
        }
    }
}