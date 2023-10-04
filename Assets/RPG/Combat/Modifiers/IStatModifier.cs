using RPG.Stats;

namespace RPG.Combat.Modifiers {
    public interface IStatModifier {
        float ReflectFlatStat(Stat stat);
        float ReflectPercentStat(Stat stat);
    }
}
