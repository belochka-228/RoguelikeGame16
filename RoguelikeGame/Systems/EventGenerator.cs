using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RoguelikeGame.Models;

namespace RoguelikeGame.Systems
{
    public class EventGenerator
    {
        public EventType GenerateEvent(int floor)
        {
            // каждые 10 ходов появляется босс
            if (floor % 10 == 0)
            {
                return EventType.Boss;
            }

            int roll = RandomService.Next(0, 100);

            if (roll < 50)
                return EventType.Enemy;

            return EventType.Chest;
        }
    }
}
