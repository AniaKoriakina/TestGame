
//using var game = new MyRoguelite.Game1();
//game.Run();
using Microsoft.Xna.Framework;
using MyRoguelite;

GameplayPresenter g = new GameplayPresenter(new Game1(), new GameCycle());
g.LaunchGame();
