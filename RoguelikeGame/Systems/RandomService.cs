using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeGame.Systems
{
    public static class RandomService
    {
        private static Random random = new Random();

        public static int Next(int min, int max)
        {
            return random.Next(min, max);
        }

        public static double Chance()
        {
            return random.NextDouble();
        }
    }
}
