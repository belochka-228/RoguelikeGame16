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
        private Player player = new Player();
        private List<Enemy> enemies = new List<Enemy>();

        private EnemyFactory enemyFactory = new EnemyFactory();
        private BossFactory bossFactory = new BossFactory();
        private CombatSystem combat = new CombatSystem();
        private EventGenerator eventGenerator = new EventGenerator();
        private LootSystem lootSystem = new LootSystem();

        private bool attackMode = false;
        private int floor = 1;

        public GamePage()
        {
            InitializeComponent();
            UpdateUI();
            NextTurn();
        }

        private void UpdateUI()
        {
            HpBar.Maximum = player.MaxHP;
            HpBar.Value = player.HP;
            HpText.Text = $"{player.HP} / {player.MaxHP}";

            WeaponImage.Source = new BitmapImage(new Uri(player.Weapon.Image, UriKind.Relative));
            ArmorImage.Source = new BitmapImage(new Uri(player.Armor.Image, UriKind.Relative));

            FloorText.Text = $"Этаж: {floor}";
        }

        private void Log(string text)
        {
            LogBox.AppendText(text + "\n");
            LogBox.ScrollToEnd();
        }

        private void NextTurn()
        {
            var ev = eventGenerator.GenerateEvent(floor);
            if (ev == EventType.Chest)
            {
                SpawnChest();
                return;
            }
            SpawnEnemies(ev == EventType.Boss);
            DrawEnemies();
        }

        private void SpawnChest()
        {
            EnemyPanel.Items.Clear();
            Image chest = new Image();
            chest.Width = 200;
            chest.Source = new BitmapImage(new Uri("/Assets/UI/chest.png", UriKind.Relative));
            chest.MouseLeftButtonDown += OpenChest;
            EnemyPanel.Items.Add(chest);
            Log("Вы нашли сундук!");
        }

        private void SpawnEnemies(bool boss)
        {
            enemies.Clear();
            if (boss)
            {
                enemies.Add(bossFactory.CreateBoss());
                Log("Появился БОСС!");
                return;
            }
            int count = RandomService.Next(1, 4);
            for (int i = 0; i < count; i++)
                enemies.Add(enemyFactory.CreateEnemy());
            Log($"Вы встретили {count} врагов!");
        }

        private void DrawEnemies()
        {
            EnemyPanel.Items.Clear();

            foreach (var enemy in enemies)
            {
                Border border = new Border();
                border.BorderBrush = Brushes.Gray;
                border.BorderThickness = new Thickness(2);
                border.CornerRadius = new CornerRadius(15);
                border.Background = new SolidColorBrush(Color.FromArgb(180, 0, 0, 0));
                border.Margin = new Thickness(20);
                border.Padding = new Thickness(15);
                border.Width = 220;

                StackPanel panel = new StackPanel();

                Image img = new Image();
                img.Width = 180;
                img.Height = 180;
                img.Stretch = Stretch.Uniform;
                img.Source = new BitmapImage(new Uri(enemy.Image, UriKind.Relative));

                img.MouseEnter += (s, e) =>
                {
                    if (attackMode) border.BorderBrush = Brushes.Red;
                };
                img.MouseLeave += (s, e) =>
                {
                    border.BorderBrush = Brushes.Gray;
                };
                img.MouseLeftButtonDown += (s, e) =>
                {
                    if (!attackMode) return;
                    PlayerAttack(enemy);
                };

                TextBlock nameBlock = new TextBlock();
                nameBlock.Text = enemy.Name;
                nameBlock.Foreground = Brushes.Yellow;
                nameBlock.FontSize = 18;
                nameBlock.FontWeight = FontWeights.Bold;
                nameBlock.HorizontalAlignment = HorizontalAlignment.Center;
                nameBlock.Margin = new Thickness(0, 10, 0, 5);

                TextBlock hp = new TextBlock();
                hp.Text = $" Здоровье: {enemy.HP}";
                hp.Foreground = Brushes.White;
                hp.FontSize = 14;
                hp.HorizontalAlignment = HorizontalAlignment.Center;

                TextBlock stats = new TextBlock();
                stats.Text = $" Урон: {enemy.Attack}    Защита: {enemy.Defense}";
                stats.Foreground = Brushes.LightGray;
                stats.FontSize = 12;
                stats.HorizontalAlignment = HorizontalAlignment.Center;

                panel.Children.Add(img);
                panel.Children.Add(nameBlock);
                panel.Children.Add(hp);
                panel.Children.Add(stats);

                border.Child = panel;
                EnemyPanel.Items.Add(border);
            }
        }

        private void Attack_Click(object sender, RoutedEventArgs e)
        {
            attackMode = true;
            Log("Нажмите на врага для атаки.");
        }

        private void PlayerAttack(Enemy enemy)
        {
            attackMode = false;
            int damage = combat.PlayerAttack(player, enemy);
            Log($"Вы нанесли {damage} урона врагу {enemy.Name}");

            if (enemy.HP <= 0)
            {
                Log($"{enemy.Name} побеждён!");
                enemies.Remove(enemy);
            }

            DrawEnemies();

            if (enemies.Count == 0)
            {
                floor++;
                UpdateUI();
                NextTurn();
                return;
            }
            EnemyTurn();
        }

        private void Defense_Click(object sender, RoutedEventArgs e)
        {
            EnemyTurn(true);
        }

        private void EnemyTurn(bool defending = false)
        {
            foreach (var enemy in enemies)
            {
                int damage = enemy.Attack - player.Armor.Defense;
                if (damage < 0) damage = 0;

                if (defending)
                {
                    if (RandomService.Chance() < 0.4)
                    {
                        Log("Вы уклонились от атаки!");
                        continue;
                    }
                    int block = RandomService.Next(70, 101);
                    damage = damage * (100 - block) / 100;
                }

                if (enemy.CritChance > 0 && RandomService.Chance() < enemy.CritChance)
                {
                    damage *= 2;
                    Log($"{enemy.Name} наносит критический удар!");
                }

                if (enemy.FreezeChance > 0 && RandomService.Chance() < enemy.FreezeChance)
                {
                    Log($"{enemy.Name} использует заморозку!");
                }

                player.HP -= damage;
                Log($"{enemy.Name} наносит {damage} урона");

                if (player.HP <= 0)
                {
                    GameOver over = new GameOver();
                    over.Show();
                    Close();
                    return;
                }
            }
            UpdateUI();
        }

        private async void OpenChest(object sender, EventArgs e)
        {
            // Убираем сундук
            EnemyPanel.Items.Clear();

            Item item = lootSystem.GenerateLoot();

            Image img = new Image();
            img.Width = 200;
            img.Height = 200;
            img.Stretch = Stretch.Uniform;

            bool isPotion = item is Potion;
            bool isWeapon = item is Weapon;
            bool isArmor = item is Armor;

            string message = "";
            bool takeItem = false;

            if (isPotion)
            {
                img.Source = new BitmapImage(new Uri("/Assets/UI/potion.png", UriKind.Relative));
                message = "Вы нашли лечебное зелье! Оно полностью восстановит ваше здоровье.\n\nЗабрать?";
                if (MessageBox.Show(message, "Сундук", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    player.HP = player.MaxHP;
                    Log("Вы выпили зелье. HP полностью восстановлено.");
                    takeItem = true;
                }
                else
                {
                    Log("Вы решили не брать зелье.");
                }
            }
            else if (isWeapon)
            {
                Weapon w = (Weapon)item;
                img.Source = new BitmapImage(new Uri(w.Image, UriKind.Relative));
                message = $"Найдено оружие: {w.Name}\nУрон: +{w.Attack}\n\nВаше текущее оружие: {player.Weapon.Name} (+{player.Weapon.Attack})\n\nЗаменить?";
                if (MessageBox.Show(message, "Сундук", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (w.Attack > player.Weapon.Attack)
                    {
                        player.Weapon = w;
                        Log($"Вы экипировали {w.Name} (+{w.Attack} урона).");
                        takeItem = true;
                    }
                    else
                    {
                        Log($"Оружие {w.Name} хуже текущего. Вы его выбросили.");
                    }
                }
                else
                {
                    Log("Вы оставили старое оружие.");
                }
            }
            else if (isArmor)
            {
                Armor a = (Armor)item;
                img.Source = new BitmapImage(new Uri(a.Image, UriKind.Relative));
                message = $"Найдена броня: {a.Name}\nЗащита: +{a.Defense}\n\nВаша текущая броня: {player.Armor.Name} (+{player.Armor.Defense})\n\nЗаменить?";
                if (MessageBox.Show(message, "Сундук", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (a.Defense > player.Armor.Defense)
                    {
                        player.Armor = a;
                        Log($"Вы экипировали {a.Name} (+{a.Defense} защиты).");
                        takeItem = true;
                    }
                    else
                    {
                        Log($"Броня {a.Name} хуже текущей. Вы её выбросили.");
                    }
                }
                else
                {
                    Log("Вы оставили старую броню.");
                }
            }

            if (takeItem)
            {
                EnemyPanel.Items.Add(img);
                await Task.Delay(1500);
            }

            floor++;
            UpdateUI();

            await Task.Delay(800);
            NextTurn();
        }
    }
}