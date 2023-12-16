using RPG.Creatures.Controls;

namespace RPG.UI.Cursors {
    public interface ITrajectory {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController invoker);
    }
}
