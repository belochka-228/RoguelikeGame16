using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoguelikeGame.Models
{
    public abstract class Item
    {
        public string Name;
        public string Image;

        public Item(string name, string image)
        {
            Name = name;
            Image = image;
        }
    }
}