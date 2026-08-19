
namespace ArTiX.Utils
{
    public abstract class GameState
    {
        public abstract void EnterState();

        public abstract void Update();

        public abstract void ExitState();
    }
}