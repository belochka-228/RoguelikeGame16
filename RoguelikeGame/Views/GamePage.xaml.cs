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
    // главное игровое окно, логика боя, сундуков и т.д.
    public partial class GamePage : Window
    {
        // игрок и список врагов 
        private readonly Player player = new Player();
        private readonly List<Enemy> enemies = new List<Enemy>();

        // нужные системы для создания врагов, расчёта боя и т.д.
        private readonly EnemyFactory enemyFactory = new EnemyFactory();
        private readonly BossFactory bossFactory = new BossFactory();
        private readonly CombatSystem combat = new CombatSystem();
        private readonly EventGenerator eventGenerator = new EventGenerator();
        private readonly LootSystem lootSystem = new LootSystem();

        private bool attackMode = false; // режим выбора цели
        private int floor = 1;           // текущий этаж

        public GamePage()
        {
            InitializeComponent();
            UpdateUI();    // показать здоровье, оружие, броню
            NextTurn();    // начать первый ход
        }

        // обновление интерфейса: полоску здоровья, иконки, характеристики и этаж
        private void UpdateUI()
        {
            // настройка полосы здоровья
            HpBar.Maximum = player.MaxHP;
            HpBar.Value = player.HP;
            HpText.Text = $"{player.HP} / {player.MaxHP}";

            // картинки оружия и брони
            WeaponImage.Source = new BitmapImage(new Uri(player.Weapon.Image, UriKind.Relative));
            ArmorImage.Source = new BitmapImage(new Uri(player.Armor.Image, UriKind.Relative));
            // текст под иконками с характеристиками
            WeaponStats.Text = $"{player.Weapon.Name}  |  Урон: +{player.Weapon.Attack}";
            ArmorStats.Text = $"{player.Armor.Name}  |  Защита: +{player.Armor.Defense}";

            // показываем этаж
            FloorText.Text = $"Этаж: {floor}";
        }

        // добавляет сообщение в журнал событий 
        private void Log(string text)
        {
            LogBox.AppendText(text + "\n");
            LogBox.ScrollToEnd();
        }

        // начинает следующий ход, генерирует событие
        private void NextTurn()//если сундук вызывает SpawnChest(), иначе врга
        {
            var ev = eventGenerator.GenerateEvent(floor);
            if (ev == EventType.Chest)
            {
                SpawnChest();   // если сундук, показываем его
                return;
            }
            // иначе спавним врагов 
            SpawnEnemies(ev == EventType.Boss);// очищаем список и вызываем врагов
            DrawEnemies();      // отрисовываем их на экране
        }

        // создаёт сундук на поле, при клике открывается
        private void SpawnChest()
        {
            EnemyPanel.Items.Clear();
            Image chest = new Image
            {
                Width = 400,
                Source = new BitmapImage(new Uri("/Assets/UI/chest.png", UriKind.Relative))
            };
            chest.MouseLeftButtonDown += OpenChest; // открытие
            EnemyPanel.Items.Add(chest);
            Log("Вы нашли сундук!");
        }

        // создаёт врагов
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

        // рисует всех врагов с их характеристиками 
        // каждый враг рамка с картинкой и текстом
        private void DrawEnemies()
        {
            EnemyPanel.Items.Clear();

            foreach (var enemy in enemies)
            {
                Border border = new Border
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(5),
                    Background = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)),
                    Margin = new Thickness(20),
                    Padding = new Thickness(15),
                    Width = 250,
                    Height = 380,
                };

                StackPanel panel = new StackPanel(); // вертикальное расположение

                // картинка врага
                Image img = new Image
                {
                    Width = 250,
                    Height = 240,
                    Stretch = Stretch.Uniform,
                    Source = new BitmapImage(new Uri(enemy.Image, UriKind.Relative))
                };

                // подсветка рамки красным при наведении, если активен режим атаки
                img.MouseEnter += (s, e) =>
                {
                    if (attackMode) border.BorderBrush = Brushes.Red;
                };
                img.MouseLeave += (s, e) =>
                {
                    border.BorderBrush = Brushes.Gray;
                };
                // клик по врагу - атака 
                img.MouseLeftButtonDown += (s, e) =>
                {
                    if (!attackMode) return;
                    PlayerAttack(enemy);
                };

                // имя врага 
                TextBlock nameBlock = new TextBlock
                {
                    Text = enemy.Name,
                    Foreground = Brushes.Yellow,
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 5)
                };

                // текущее здоровье
                TextBlock hp = new TextBlock
                {
                    Text = $" Здоровье: {enemy.HP}",
                    Foreground = Brushes.White,
                    FontSize = 16,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // атака и защита врага
                TextBlock stats = new TextBlock
                {
                    Text = $" Урон: {enemy.Attack}    Защита: {enemy.Defense}",
                    Foreground = Brushes.LightGray,
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                // собираем всё вместе
                panel.Children.Add(img);
                panel.Children.Add(nameBlock);
                panel.Children.Add(hp);
                panel.Children.Add(stats);

                border.Child = panel;
                EnemyPanel.Items.Add(border);
            }
        }

        // обр кнопки "Атака", включает режим выбора цели
        private void Attack_Click(object sender, RoutedEventArgs e)
        {
            attackMode = true;
            Log("Нажмите на врага для атаки.");
        }

        // атака игрока по выбранному врагу, считает урон, убирает мёртвых
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

            // если врагов не осталось, переходим на след
            if (enemies.Count == 0)
            {
                floor++;
                UpdateUI();
                NextTurn();
                return;
            }
            // иначе ход переходит к врагам
            EnemyTurn();
        }

        // обр кнопки "Защита", запускает ход врагов с защитой
        private void Defense_Click(object sender, RoutedEventArgs e)
        {
            EnemyTurn(true);
        }

        // ход врагов, если defending = true, игрок защищается
        private void EnemyTurn(bool defending = false)
        {
            foreach (var enemy in enemies)
            {
                // базовый урон от врага (с учётом брони или её игнора)
                int damage = combat.EnemyAttack(player, enemy);

                if (defending)
                {
                    // 40% шанс полностью уклониться
                    if (RandomService.Chance() < 0.4)
                    {
                        Log("Вы уклонились от атаки!");
                        continue;
                    }
                    // блок: снижаем урон на 70-100%
                    int block = RandomService.Next(70, 101);
                    damage = damage * (100 - block) / 100;
                }

                // критический удар
                if (enemy.CritChance > 0 && RandomService.Chance() < enemy.CritChance)
                {
                    damage *= 2;
                    Log($"{enemy.Name} наносит критический удар!");
                }

                // заморозка 
                if (enemy.FreezeChance > 0 && RandomService.Chance() < enemy.FreezeChance)
                {
                    Log($"{enemy.Name} использует заморозку!");
                }

                // применяем урон к игроку
                player.HP -= damage;
                Log($"{enemy.Name} наносит {damage} урона");

                // проверка смерти
                if (player.HP <= 0)
                {
                    GameOver over = new GameOver();
                    over.Show();
                    Close();
                    return;
                }
            }
            UpdateUI(); // обновляем полоску здоровья
        }

        // открытие сундука: генерирует предмет, спрашивает, забрать ли и показывает картинку
        private async void OpenChest(object sender, EventArgs e)
        {
            EnemyPanel.Items.Clear(); // убираем сундук
            Item item = lootSystem.GenerateLoot();//ген предмет

            Image img = new Image
            {
                Width = 350,
                Height = 350,
                Stretch = Stretch.Uniform
            };

            bool isPotion = item is Potion;
            bool isWeapon = item is Weapon;
            bool isArmor = item is Armor;
            bool takeItem = false; 

            // зелье 
            if (isPotion)
            {
                img.Source = new BitmapImage(new Uri("/Assets/UI/potion.png", UriKind.Relative));
                string message = "Вы нашли лечебное зелье! Оно полностью восстановит ваше здоровье.\n\nЗабрать?";
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
            // оружие
            else if (isWeapon)
            {
                Weapon w = (Weapon)item;
                img.Source = new BitmapImage(new Uri(w.Image, UriKind.Relative));
                string message = $"Найдено оружие: {w.Name}\nУрон: +{w.Attack}\n\nВаше текущее оружие: {player.Weapon.Name} (+{player.Weapon.Attack})\n\nЗаменить?";
                if (MessageBox.Show(message, "Сундук", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    player.Weapon = w;
                    Log($"Вы экипировали {w.Name} (+{w.Attack} урона).");
                    takeItem = true;
                }
                else
                {
                    Log("Вы оставили старое оружие.");
                }
            }
            // бронька
            else if (isArmor)
            {
                Armor a = (Armor)item;
                img.Source = new BitmapImage(new Uri(a.Image, UriKind.Relative));
                string message = $"Найдена броня: {a.Name}\nЗащита: +{a.Defense}\n\nВаша текущая броня: {player.Armor.Name} (+{player.Armor.Defense})\n\nЗаменить?";
                if (MessageBox.Show(message, "Сундук", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    player.Armor = a;
                    Log($"Вы экипировали {a.Name} (+{a.Defense} защиты).");
                    takeItem = true;
                }
                else
                {
                    Log("Вы оставили старую броню.");
                }
            }

            // если взяли предмет - покажем его картинку на секунду
            if (takeItem)
            {
                EnemyPanel.Items.Add(img);
                await Task.Delay(800);
            }

            // переход на следующий этаж 
            floor++;
            UpdateUI();
            await Task.Delay(800);
            NextTurn();
        }
    }
}