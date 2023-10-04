using RPG.Combat.Modifiers;
using RPG.Stats;

namespace RPG.Combat.Buffs {
    public abstract class Buff : IStatModifier {
        
        /// <summary>
        /// Method invokes each frame to handle effects from buff
        /// </summary>
        public abstract void TickBuff();
        /// <summary>
        /// Method which cast when entity interact with another entity
        /// </summary>
        public abstract void CastBuff(Health health);

        public abstract float ReflectFlatStat(Stat stat);
        public abstract float ReflectPercentStat(Stat stat);
    }
}
