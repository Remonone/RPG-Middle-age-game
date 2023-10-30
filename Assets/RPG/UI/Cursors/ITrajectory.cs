using RPG.Control;

namespace RPG.UI.Cursors {
    public interface ITrajectory {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController invoker);
    }
}
