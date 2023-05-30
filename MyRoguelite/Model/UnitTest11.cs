using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using MyRoguelite.Objects;
using MyRoguelite.View;
using System;
using static MyRoguelite.Model.IModel;

namespace MyRoguelite.Model
{
    [TestClass]
    public class UnitTest11
    {
        [TestMethod]
        public void InitializeTest()
        {
            var gameCycle = new GameCycle();
            gameCycle.Initialize();

            Assert.IsNotNull(gameCycle.Objects);
            Assert.IsNotNull(gameCycle.LiteralyPlayer);
            Assert.AreEqual(100, gameCycle.LiteralyPlayer.Health);
            Assert.AreEqual(1, gameCycle.LiteralyPlayer.ImageId);
            Assert.AreEqual("Health 100%", GameCycle.HealthText);
            Assert.AreEqual("Total: 0", GameCycle.EnemyCountText);
        }


        [TestMethod]
        public void MovePlayerTest()
        {
            var gameCycle = new GameCycle();
            gameCycle.Initialize();
            Player p = gameCycle.LiteralyPlayer;
            var pos = p.Pos;
            gameCycle.MovePlayer(Direction.right);
            Assert.AreEqual(new Vector2((pos.X + p.Speed), pos.Y), p.Pos);
        }

      //  [TestMethod]

    }
}