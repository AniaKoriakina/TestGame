using Microsoft.Xna.Framework;
using System;
using MonoGame.Extended.Content;
using MonoGame.Extended.Sprites;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyRoguelite.Objects;
using MonoGame.Extended;
using static MyRoguelite.Model.IModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyRoguelite.View;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;
using Point = System.Drawing.Point;
using MessageBox = System.Windows.Forms.MessageBox;
using Size = System.Drawing.Size;
using SharpDX.DirectWrite;

namespace MyRoguelite.Model
{
    public class GameCycle : IModel
    {
        private int _currentId;
        private int _tileSizeWidth = 137;
        private int _tileSizeHeight = 135;
        private int enemyCount = 0;
        private int allEnemyKilled = 0;

        private float enemySpawnTimer = 0f;
        private static float baseEnemySpawnInterval = 3f;
        private float enemySpawnInterval = baseEnemySpawnInterval;
        private float elapsedSeconds = 0f;
        private float intervalChangeTime = 30f;

        private bool isBulletSpawned;
        private bool hasIntervalChanged = false;
        private bool isGamePaused = false;
        private bool isUpgradeWindowActive = false;
        private bool isUpgradeWindowShown = false;
        private static bool upgradeDamageApplied = false;

        private char[,] _map = new char[14, 8];

        public int PlayerId { get; set; }
        public static string HealthText { get; set; }
        public static string EnemyCountText { get; set; }

        public Player LiteralyPlayer { get; set; }
        public Dictionary<int, IObject> Objects { get; set; }
        private List<int> enemiesToRemove = new List<int>();

        TimeSpan gameDuration = TimeSpan.Zero;

        private double elapsedTime = 0;
        private double speedIncreaseInterval = 20000;
        private float speedIncreaseAmount = 2;

        public static GameCycle gameCycle = new GameCycle();

        public event EventHandler<GameplayEventArgs> Updated = delegate { };

        private int baseBulletDamage = 6;
        public bool IsGamePaused { get { return isGamePaused; } }

        public void SetObjects(Dictionary<int, IObject> objects)
        {
            Objects = objects;
        }

        bool canPlayerMoveAndShoot = true;
        bool isGameOver = false;
        GameOverState gameOverState = new GameOverState();




        public void DrawHealthBars(SpriteBatch spriteBatch)
        {
            foreach (var obj in Objects)
            {
                if (obj.Value is Enemy enemy)
                {
                    var healthBarPosition = enemy.Pos - new Vector2(30, 60);
                    var healthBarWidth = 50f;
                    var healthBarHeight = 5f;
                    var healthBarFillWidth = healthBarWidth * (enemy.Health / enemy.MaxHealth);
                    spriteBatch.DrawRectangle(healthBarPosition, new Vector2(healthBarWidth, healthBarHeight), Color.Black);
                    spriteBatch.DrawRectangle(healthBarPosition, new Vector2(healthBarFillWidth, healthBarHeight), Color.Red);
                }
            }
        }
        public void Update()
        {

            var gameTime = Game1.GameTime;

            if (!isGamePaused && !isUpgradeWindowActive)
            {
                elapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            gameDuration += gameTime.ElapsedGameTime;
            TimeSpan fiveMinutes = TimeSpan.FromMinutes(5);

            if (gameDuration >= fiveMinutes)
            {
                ShowVictoryWindow(); 
                isGamePaused = true; 
            }

            foreach (var obj in Objects)
            {
                if (!isUpgradeWindowActive)
                {
                    Objects[obj.Key].Update();
                }
            }

            if (!isUpgradeWindowActive)
            {
                TimerForSpawnEnemy(gameTime);
                ColliderForHealthDown();
            }

            LiteralyPlayer.Update();
            ColliderForBulletDamage();

            if (LiteralyPlayer.IsDead() && !isGameOver)
            {
                canPlayerMoveAndShoot = false;
                gameOverState.ShowGameOverForm();
            }

            var obj2 = new Dictionary<int, IObject>(Objects)
            {
                { PlayerId, LiteralyPlayer }
            };

            Updated.Invoke(this, new GameplayEventArgs
            {
                Objects = obj2,
            });
            

            if (!isUpgradeWindowShown && isUpgradeWindowActive)
            {
                isUpgradeWindowShown = true;
                ShowUpgradeWindow();
            }
            else if (!isUpgradeWindowActive)
            {
                UpdateEnemies();
                UpdateBullets();
            }

            if (isGamePaused)
            {
                return;
            }
        }

        private void ShowVictoryWindow()
        {
            MessageBox.Show("Вы победили!", "Победа", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Environment.Exit(0);
        }

        private void ShowUpgradeWindow()
        {
            isUpgradeWindowActive = true;
            isGamePaused = true;
            Form upgradeForm = new Form();

            upgradeForm.FormBorderStyle = FormBorderStyle.None;
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;
            upgradeForm.StartPosition = FormStartPosition.Manual;
            upgradeForm.Left = (screenWidth - upgradeForm.Width) / 2;
            upgradeForm.Top = (screenHeight - upgradeForm.Height) / 2;

            upgradeForm.Text = "Окно Апгрейда";

            Label label = new Label();
            label.Text = "Апгрейд:";
            label.Location = new System.Drawing.Point(50, 10);
            upgradeForm.Controls.Add(label);

            Button button1 = new Button();
            button1.Text = "MaxHealth";
            button1.Location = new System.Drawing.Point(10, 40);
            button1.Click += (sender, e) =>
            {
                UpgradeHealth();
                isUpgradeWindowActive = false;
                isGamePaused = false;
                upgradeForm.Close();
            };
            upgradeForm.Controls.Add(button1);

            Button button2 = new Button();
            button2.Text = "Speed";
            button2.Location = new System.Drawing.Point(10, 70);
            button2.Click += (sender, e) =>
            {
                UpgradeSpeed();
                isUpgradeWindowActive = false;
                isGamePaused = false;
                upgradeForm.Close();
            };
            upgradeForm.Controls.Add(button2);

            Button button3 = new Button();
            button3.Text = "Damage";
            button3.Location = new System.Drawing.Point(10, 100);
            button3.Click += (sender, e) =>
            {
                baseBulletDamage += 1;
                upgradeDamageApplied = true;
                isUpgradeWindowActive = false;
                isGamePaused = false;
                upgradeForm.Close();
            };
            upgradeForm.Controls.Add(button3);

            Button button4 = new Button();
            button4.Text = "FullHealth";
            button4.Location = new System.Drawing.Point(10, 130);
            button4.Click += (sender, e) =>
            {
                UpgradeFullHealth();
                upgradeDamageApplied = true;
                isUpgradeWindowActive = false;
                isGamePaused = false;
                upgradeForm.Close();
            };
            upgradeForm.Controls.Add(button4);

            upgradeForm.FormClosed += (sender, e) =>
            {
                isUpgradeWindowShown = false;
                isUpgradeWindowActive = false;
                isGamePaused = true;
            };

            upgradeForm.ShowDialog();
        }

        private void ColliderForBulletDamage()
        {
            foreach (var bulletObj in Objects)
            {
                if (bulletObj.Value is Bullets bullet)
                {
                    foreach (var enemyObj in Objects)
                    {
                        if (enemyObj.Value is Enemy enemy)
                        {
                            if (Collider.IsCollided(bullet.Collider, enemy.Collider))
                            {
                                enemy.Health -= bullet.Damage;
                                if (enemy.Health <= 0)
                                {
                                    enemy.Health = 0;
                                    Objects.Remove(enemyObj.Key);
                                    enemy.IsDead();
                                    enemiesToRemove.Add(enemyObj.Key);
                                    allEnemyKilled++;
                                    UpdateEnemyCountText();
                                    if (allEnemyKilled % 5 == 0 && !isUpgradeWindowActive && allEnemyKilled < 30)
                                    {
                                        isUpgradeWindowActive = true;
                                        isGamePaused = true;
                                    }
                                    if (allEnemyKilled >= 20 && allEnemyKilled % 10 == 0 && !isUpgradeWindowActive)
                                    {
                                        isUpgradeWindowActive = true;
                                        isGamePaused = true;
                                    }
                                }
                                Objects.Remove(bulletObj.Key);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private float maxHealth;

        public void UpgradeHealth()
        {
            LiteralyPlayer.Health += 10;
            maxHealth += 10;
            UpdateHealthText();
        }

        public void UpgradeFullHealth()
        {
            LiteralyPlayer.Health = maxHealth;
            UpdateHealthText(); 
        }

        private void UpdateHealthText()
        {
            if (LiteralyPlayer.Health >= 0)
                HealthText = "Health " + Math.Round(LiteralyPlayer.Health).ToString() + "%";
            else
                HealthText = "Health 0%";
        }

        public void UpgradeSpeed()
        {
            LiteralyPlayer.Speed += 0.3f;
        }


        private void ColliderForHealthDown()
        {
            foreach (var obj in Objects)
            {
                if (obj.Value is Enemy enemy)
                {
                    if (Collider.IsCollided(LiteralyPlayer.Collider, enemy.Collider))
                    {
                        float healthBefore = LiteralyPlayer.Health;
                        LiteralyPlayer.Health -= 0.09f;
                        if (LiteralyPlayer.Health < 0)
                        {
                            LiteralyPlayer.IsDead();
                        }
                        if (LiteralyPlayer.Health != healthBefore)
                        {
                            if (LiteralyPlayer.Health >= 0)
                                HealthText = "Health " + Math.Round(LiteralyPlayer.Health).ToString() + "%";
                        }
                    }
                }
            }
        }

        private void TimerForSpawnEnemy(GameTime gameTime)
        {
            elapsedSeconds += (float)gameTime.TotalGameTime.TotalSeconds;
            if (!hasIntervalChanged && elapsedSeconds >= intervalChangeTime)
            {
                hasIntervalChanged = true;
                enemySpawnInterval = 1f;
            }

            enemySpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (enemySpawnTimer >= enemySpawnInterval)
            {
                enemySpawnTimer = 0f;
                GenerateEnemies();
            }
        }

        public void Initialize()
        {
            Objects = new Dictionary<int, IObject>();
            _map[6, 3] = 'P';
            _map[7, 3] = 'E';
            for (int x = 0; x < _map.GetLength(0); x++)
            {
                for (int y = 0; y < _map.GetLength(1); y++)
                {
                    if (x == 0 || y == 0 || x == _map.GetLength(0) - 1 || y == _map.GetLength(1) - 1)
                    {
                        _map[x, y] = 'W';
                    }
                }
            }
            _currentId = 1;
            bool isPlacedPlayer = false;
            CreateWall();
            GenerateEnemies();
            if (!isPlacedPlayer)
            {
                isPlacedPlayer = CleatePlayer(isPlacedPlayer);
            }

        }




        public void CreateBullet(Vector2 playerPosition, Vector2 mousePosition)
        {
            Vector2 direction = mousePosition - playerPosition;
            direction.Normalize();
            float bulletSpeed = 7f;
            Vector2 velocity = direction * bulletSpeed;
            var bullet = new Bullets(playerPosition, velocity, 1);
            bullet.ImageId = 4;
            bullet.Damage = baseBulletDamage;
            Objects.Add(_currentId, bullet);
            _currentId++;

        }

        private void UpdateBullets()
        {

            if (Mouse.GetState().LeftButton == ButtonState.Pressed && !isBulletSpawned)
            {
                Vector2 mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                CreateBullet(LiteralyPlayer.Pos, mousePosition);
                isBulletSpawned = true;
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                isBulletSpawned = false;
            }

            foreach (var obj in Objects)
            {
                if (obj.Value is Bullets bullet)
                {
                    bullet.Update();
                }
            }

        }

        private void GenerateEnemies()
        {
            Random random = new Random();

            int x = random.Next(_map.GetLength(0));
            int y = random.Next(_map.GetLength(1));

            if (_map[x, y] != 'E' && _map[x, y] != 'W' && _map[x, y] != 'P')
            {
                Enemy enemy = new Enemy(new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2), 2);
                enemy.Pos = new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2);
                enemy.ImageId = 3;
                enemy.EnemyHealthText = enemy.Health.ToString();
                enemy.Speed = 1;

                if (elapsedTime >= speedIncreaseInterval)
                {
                    enemy.Speed += speedIncreaseAmount;
                }
                enemy.Size = new Vector2(135, 137);
                
                Objects.Add(_currentId, enemy);
                _currentId++;
                enemyCount++;
                if (enemy.IsDead())
                {
                    enemyCount--;
                }
            }

        }

        private void UpdateEnemyCountText()
        {
            EnemyCountText = "Total: " + allEnemyKilled.ToString();
        }

        private void UpdateEnemies()
        {
            Vector2 playerPosition = LiteralyPlayer.Pos;



            foreach (var obj in Objects)
            {
                if (obj.Value is Enemy enemy)
                {
                    Vector2 direction = Vector2.Normalize(playerPosition - enemy.Pos);
                    enemy.Pos += direction * enemy.Speed;

                    foreach (var otherObj in Objects)
                    {
                        if (otherObj.Value is Enemy otherEnemy && enemy != otherEnemy)
                        {
                            if (enemy.Collider.Boundary.Intersects(otherEnemy.Collider.Boundary))
                            {
                                Vector2 repulsionDirection = Vector2.Normalize(enemy.Pos - otherEnemy.Pos);
                                float repulsionDistance = 0.1f;
                                enemy.Pos += repulsionDirection * repulsionDistance;
                                otherEnemy.Pos -= repulsionDirection * repulsionDistance;
                            }
                        }
                    }
                }
            }

            foreach (int enemyId in enemiesToRemove)
            {
                Objects.Remove(enemyId);
                enemyCount--;
            }
            enemiesToRemove.Clear();

            //if (enemyCount < maxEnemyCount)
            //{
            //    GenerateEnemies();
            //}
        }

        private void CreateWall()
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                for (int x = 0; x < _map.GetLength(0); x++)
                {
                    if (_map[x, y] == 'W')
                    {
                        Wall w = new Wall(new Vector2(135, 137));
                        w.Pos = new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2 + 2, y * _tileSizeHeight + _tileSizeHeight / 2 + 2);
                        w.ImageId = 2;
                        Objects.Add(_currentId, w);
                        _currentId++;
                    }
                }
            }
        }

        private bool CleatePlayer(bool isPlacedPlayer)
        {
            for (int y = 0; y < _map.GetLength(1); y++)
            {
                for (int x = 0; x < _map.GetLength(0); x++)
                {
                    if (_map[x, y] == 'P')
                    {
                        Player player = new Player(new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2),
                            new Vector2(135, 137), 100);
                        player.ImageId = 1;
                        player.Pos = new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2,
                            y * _tileSizeHeight + _tileSizeHeight / 2);
                        player.Speed = 7;
                        PlayerId = _currentId++;


                        LiteralyPlayer = player;
                        HealthText = "Health " + player.Health.ToString() + "%";
                        LiteralyPlayer.Health = player.Health;
                        maxHealth = LiteralyPlayer.Health;
                        isPlacedPlayer = true;
                        break;
                    }
                }

                if (isPlacedPlayer)
                {
                    break;
                }
            }

            return isPlacedPlayer;
        }

        public void MovePlayer(Direction dir)
        {
            Player p = LiteralyPlayer;
            Vector2 playerInitPos = p.Pos;
            Vector2 newPos = p.Pos;

            switch (dir)
            {
                case Direction.up:
                    {
                        newPos += new Vector2(0, -p.Speed);
                        break;
                    }
                case Direction.down:
                    {
                        newPos += new Vector2(0, p.Speed);
                        break;
                    }
                case Direction.right:
                    {
                        newPos += new Vector2(p.Speed, 0);
                        break;
                    }
                case Direction.left:
                    {
                        newPos += new Vector2(-p.Speed, 0);
                        break;
                    }
                case Direction.upRight:
                    {
                        newPos += new Vector2(p.Speed - 2, -p.Speed + 2);
                        break;
                    }
                case Direction.upLeft:
                    {
                        newPos += new Vector2(-p.Speed + 2, -p.Speed + 2);
                        break;
                    }
                case Direction.downRight:
                    {
                        newPos += new Vector2(p.Speed - 2, p.Speed - 2);
                        break;
                    }
                case Direction.downLeft:
                    {
                        newPos += new Vector2(-p.Speed + 2, p.Speed - 2);
                        break;
                    }
                case Direction.None:
                    {
                        break;
                    }
            }
            int x = (int)Math.Round(newPos.X / _tileSizeWidth);
            int y = (int)Math.Round(newPos.Y / _tileSizeHeight);
            Collider newCollider = new Collider((int)newPos.X, (int)newPos.Y, (int)p.Size.X, (int)p.Size.Y);
            bool collided = false;

            if (playerInitPos != newPos)
            {
                Dictionary<int, ISolid> solidObjects = Objects.Where(i => i.Value is ISolid)
                    .ToDictionary(i => i.Key, i => (ISolid)i.Value);

                foreach (var solidObject in solidObjects)
                {
                    Collider collider = solidObjects[solidObject.Key].Collider;

                    if (Collider.IsCollided(newCollider, collider))
                    {
                        collided = true;

                        bool collideX = Math.Abs(newPos.X - collider.Boundary.X) < Math.Abs(newPos.Y - collider.Boundary.Y);
                        bool collideY = !collideX;

                        if (collideX)
                        {
                            Vector2 returnVec = new Vector2(p.Pos.X - (playerInitPos.X + 1), p.Pos.Y - (playerInitPos.Y - 1));
                            p.Pos = new Vector2(newPos.X - returnVec.X, p.Pos.Y);
                            p.MoveCollider(p.Pos - returnVec);
                        }
                        else if (collideY)
                        {
                            Vector2 returnVec = new Vector2(p.Pos.X - (playerInitPos.X + 1), p.Pos.Y - (playerInitPos.Y - 1));
                            p.Pos = new Vector2(p.Pos.X, newPos.Y - returnVec.Y);
                            p.MoveCollider(p.Pos - returnVec);
                        }
                    }
                }
            }

            if (!collided)
            {
                p.Pos = newPos;
                p.MoveCollider(p.Pos);
            }
        }
    }

}
