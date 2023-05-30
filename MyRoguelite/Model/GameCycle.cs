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

namespace MyRoguelite.Model
{
    public class GameCycle : IModel
    {
        private int _currentId;


        private int _tileSizeWidth = 137;
        private int _tileSizeHeight = 135;

        public event EventHandler<GameplayEventArgs> Updated = delegate { };

        //    private static Vector2 _pos = new Vector2(900, 500);
        public int PlayerId { get; set; }
        private char[,] _map = new char[14, 8];

        public Dictionary<int, IObject> Objects { get; set; }

        public Player LiteralyPlayer { get; set; }
        private int enemyCount = 0;
        private int maxEnemyCount = 10;
        private int allEnemyKilled = 0;

        private bool isBulletSpawned;
        private List<int> enemiesToRemove = new List<int>();

        public static string HealthText { get; set; }
        public static string EnemyCountText { get; set; }

        private float enemySpawnTimer = 0f;
        private static float baseEnemySpawnInterval = 3f;
        private float enemySpawnInterval = baseEnemySpawnInterval;
        private float elapsedSeconds = 0f;
        private float intervalChangeTime = 30f;
        private bool hasIntervalChanged = false;

        private bool isGamePaused = false;
        public static bool upgradeDamageApplied = false;
        private bool isUpgradeWindowActive = false;

        private int baseBulletDamage = 10;

        public void SetObjects(Dictionary<int, IObject> objects)
        {
            Objects = objects;
        }


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
                elapsedTime += gameTime.ElapsedGameTime;
            }
            foreach (var obj in Objects)
            {
                Objects[obj.Key].Update();
            }

            LiteralyPlayer.Update();

            ColliderForHealthDown();

            TimerForSpawnEnemy(gameTime);

            ColliderForBulletDamage();

            if (LiteralyPlayer.IsDead()) { Environment.Exit(0); }

            var obj2 = new Dictionary<int, IObject>(Objects)
            {
                 { PlayerId, LiteralyPlayer }
            };

            Updated.Invoke(this, new GameplayEventArgs
            {
                Objects = obj2,
            });

            if (isUpgradeWindowActive)
            {
                ShowUpgradeWindow();
            }
            else
            {
                UpdateEnemies();
                UpdateBullets();
            }
        }

        private void ShowUpgradeWindow()
        {
            Console.WriteLine("=== Окно Апгрейда ===");
            Console.WriteLine("Выберите апгрейд:");
            Console.WriteLine("1. Повысить MaxHealth");
            Console.WriteLine("2. Повысить Speed");
            Console.WriteLine("3. Повысить Damage");
            string input = Console.ReadLine();
            int upgradeChoice;
            if (int.TryParse(input, out upgradeChoice))
            {
                switch (upgradeChoice)
                {
                    case 1:
                        UpgradeHealth();
                        isUpgradeWindowActive = false; 
                        isGamePaused = false;
                        break;
                    case 2:
                        UpgradeSpeed();
                        isUpgradeWindowActive = false; 
                        isGamePaused = false;
                        break;
                    case 3:
                        baseBulletDamage += 100;
                        upgradeDamageApplied = true;
                        isUpgradeWindowActive = false;
                        isGamePaused = false;
                        break;
                    default:
                        Console.WriteLine("Неверный ввод");
                        break;
                }
            }
            if (allEnemyKilled % 10 == 0)
            {
                isUpgradeWindowActive = false;
                isGamePaused = false;
            }
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
                                    if (allEnemyKilled % 10 == 0 && !isUpgradeWindowActive)
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

        public void UpgradeHealth()
        {
            LiteralyPlayer.Health += 10;
        }
        public void UpgradeSpeed()
        {
            LiteralyPlayer.Speed += 0.1f;
        }

        private void ColliderForHealthDown()
        {
            foreach (var obj in Objects)
            {
                if (obj.Value is Enemy enemy)
                {
                    if (Collider.IsCollided(LiteralyPlayer.Collider, enemy.Collider))
                    {
                        LiteralyPlayer.Health -= 0.1f;
                        HealthText = "Health " + Math.Round(LiteralyPlayer.Health).ToString() + "%";
                        if (LiteralyPlayer.Health < 0)
                        {
                            LiteralyPlayer.IsDead();
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
        }//интервал появления врагов

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

            if (_map[x, y] != 'E' && _map[x, y] != 'W' && _map[x, y] != 'P' )
            { 
                Enemy enemy = new Enemy(new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2), 2);
                enemy.Pos = new Vector2(x * _tileSizeWidth + _tileSizeWidth / 2, y * _tileSizeHeight + _tileSizeHeight / 2);
                enemy.ImageId = 3;
                enemy.EnemyHealthText = enemy.Health.ToString();
                enemy.Speed = 1;
                enemy.Size = new Vector2(135, 137);
                EnemyCountText = "Total: " + allEnemyKilled.ToString();
                Objects.Add(_currentId, enemy);
                _currentId++;
                enemyCount++;
                if (enemy.IsDead())
                {
                    enemyCount--;
                }
            }

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
                        Wall w = new Wall(new Vector2(135,137));
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
                        HealthText = "Health "+player.Health.ToString()+"%";
                        LiteralyPlayer.Health = player.Health;
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
