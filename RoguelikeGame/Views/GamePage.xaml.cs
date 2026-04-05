using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RoguelikeGame.Models;
using RoguelikeGame.Systems;

namespace RoguelikeGame.Views
{
    public partial class GamePage : Window
    {
        Player player = new Player();

        Enemy currentEnemy;

        EnemyFactory enemyFactory = new EnemyFactory();
        BossFactory bossFactory = new BossFactory();
        CombatSystem combatSystem = new CombatSystem();
        EventGenerator eventGenerator = new EventGenerator();
        LootSystem lootSystem = new LootSystem();

        int floor = 1;

        bool playerFrozen = false;

        public GamePage()
        {
            InitializeComponent();

            UpdateUI();
            NextTurn();
        }

        void UpdateUI()
        {
            HpText.Text = $"HP: {player.HP}/{player.MaxHP}";
            WeaponText.Text = $"Weapon: {player.Weapon.Name}";
            ArmorText.Text = $"Armor: {player.Armor.Name}";
            FloorText.Text = $"Floor: {floor}";
        }

        void Log(string text)
        {
            LogBox.AppendText(text + "\n");
            LogBox.ScrollToEnd();
        }

        void NextTurn()
        {
            var ev = eventGenerator.GenerateEvent(floor);

            if (ev == EventType.Boss)
            {
                currentEnemy = bossFactory.CreateBoss();
                Log($"Появился босс: {currentEnemy.Name}");
            }
            else if (ev == EventType.Enemy)
            {
                currentEnemy = enemyFactory.CreateEnemy();
                Log($"Вы встретили врага: {currentEnemy.Name}");
            }
            else
            {
                EnemyImage.Source = new BitmapImage(new System.Uri("/Assets/UI/chest.png", System.UriKind.Relative));

                Log("Вы нашли сундук");

                var item = lootSystem.GenerateLoot();

                if (item is Potion)
                {
                    player.HP = player.MaxHP;
                    Log("Вы выпили лечебное зелье и полностью восстановили HP");
                }
                else if (item is Weapon w)
                {
                    Log($"Найдено оружие: {w.Name} (+{w.Attack} атаки)");

                    player.Weapon = w;
                }
                else if (item is Armor a)
                {
                    Log($"Найдена броня: {a.Name} (+{a.Defense} защиты)");

                    player.Armor = a;
                }

                floor++;
                UpdateUI();
                return;
            }

            EnemyImage.Source = new BitmapImage(new System.Uri(currentEnemy.Image, System.UriKind.Relative));
        }

        private void Attack_Click(object sender, RoutedEventArgs e)
        {
            if (currentEnemy == null)
                return;

            int damage = combatSystem.PlayerAttack(player, currentEnemy);

            Log($"Вы нанесли {damage} урона");

            if (currentEnemy.HP <= 0)
            {
                Log("Враг побеждён");

                floor++;
                UpdateUI();
                NextTurn();
                return;
            }

            EnemyTurn();
        }

        private void Defense_Click(object sender, RoutedEventArgs e)
        {
            double dodge = RandomService.Chance();

            if (dodge < 0.4)
            {
                Log("Вы уклонились от атаки!");
                return;
            }

            EnemyTurn(true);
        }

        void EnemyTurn(bool defending = false)
        {
            if (playerFrozen)
            {
                Log("Вы заморожены и пропускаете ход!");
                playerFrozen = false;
                return;
            }

            int damage;

            if (defending)
            {
                int block = RandomService.Next(70, 101);
                int reduced = player.Armor.Defense * block / 100;

                damage = currentEnemy.Attack - reduced;

                if (damage < 0)
                    damage = 0;
            }
            else
            {
                damage = combatSystem.EnemyAttack(player, currentEnemy);
            }

            if (currentEnemy.CritChance > 0 && RandomService.Chance() < currentEnemy.CritChance)
            {
                damage *= 2;
                Log("Критический удар!");
            }

            if (currentEnemy.FreezeChance > 0 && RandomService.Chance() < currentEnemy.FreezeChance)
            {
                playerFrozen = true;
                Log("Вы заморожены!");
            }

            player.HP -= damage;

            Log($"Враг наносит {damage} урона");

            if (player.HP <= 0)
            {
                GameOver over = new GameOver();
                over.Show();

                this.Close();
                return;
            }

            UpdateUI();
        }
    }
}