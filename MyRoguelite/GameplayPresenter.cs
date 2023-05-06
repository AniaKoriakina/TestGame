using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRoguelite
{
    public class GameplayPresenter // управляет взаимодействием 
    {
        private IView _gameplayView = null;
        private IModel _gameplayModel = null;

        public GameplayPresenter(IView gameplayView, IModel gameplayModel) 
        {
            _gameplayView = gameplayView;
            _gameplayModel = gameplayModel;

            _gameplayView.CycleFinished += ViewModelUpdate; // событие вызывается, когда игровой цикл завершается 
            _gameplayView.PlayerMoved += ViewModelMovePlayer; //событие вызывается, когда игрок двигается
            _gameplayModel.Updated += ModelViewUpdate; //событие вызывается, когда игровое состояние обновляется
            _gameplayModel.Initialize();

        }

        private void ViewModelMovePlayer(object sender, ControlsEventArgs e) 
        {
            _gameplayModel.MovePlayer(e.Direction);
        }

        private void ModelViewUpdate(object sender, GameplayEventArgs e)
        {
            _gameplayView.LoadGCParameters(e.Objects);
        }

        private void ViewModelUpdate(object sender, EventArgs e)
        {
            _gameplayModel.Update();
        }

        public void LaunchGame()
        {
            _gameplayView.Run();
        }
    }
}
