
//using var game = new MyRoguelite.Game1();
//game.Run();
using Microsoft.Xna.Framework;
using MyRoguelite;
using MyRoguelite.Model;
using MyRoguelite.View;

GameplayPresenter g = new GameplayPresenter(new Game1(), new GameCycle());
g.LaunchGame();
