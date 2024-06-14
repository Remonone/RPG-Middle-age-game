using RPG.Creatures.Player;

namespace RPG.Core.Cursors {
    public interface ITrajectory {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController invoker);
    }
}
