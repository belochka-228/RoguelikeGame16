using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoguelikeGame.Models;

namespace RoguelikeGame.Systems
{
    public class CombatSystem
    {
        public int PlayerAttack(Player player, Enemy enemy)
        {
            int damage = player.TotalAttack - enemy.Defense;   // ← изменено

            if (damage < 1)
                damage = 1;

            enemy.HP -= damage;
            return damage;
        }

        public int EnemyAttack(Player player, Enemy enemy)
        {
            int defense = enemy.IgnoreArmor ? 0 : player.Armor.Defense;

            int damage = enemy.Attack - defense;

            if (damage < 1)
                damage = 1;

            player.HP -= damage;
            return damage;
        }
    }
}