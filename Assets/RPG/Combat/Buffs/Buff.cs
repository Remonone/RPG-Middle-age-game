using System;
using RPG.Combat.Modifiers;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;

namespace RPG.Combat.Buffs {
    public abstract class Buff : ScriptableObject, IStatModifier {

        // TODO: Change condition of canceling from 2 cases to any cases
        protected float BuffDuration;
        protected bool IsBuffExpires;

        protected bool IsBuffStackable;
        protected int BuffMaxStacks;
        
        protected Cooldown TickToReset = new Cooldown {
            SetTimeToReset = 1f
        };

        public float TotalDuration => BuffDuration;
        public bool IsExpiring => IsBuffExpires;
        public int MaxStacks => BuffMaxStacks;
        public bool IsStackable => IsBuffStackable;

        public Cooldown BuffTick => TickToReset;
        

        /// <summary>
        /// Method invokes each second to handle effects from buff
        /// </summary>
        public abstract void TickBuff(int quantity);
        /// <summary>
        /// Method which cast when entity interact with another entity
        /// </summary>
        public abstract void CastBuff(int quantity, params object[] cast);

        public abstract float ReflectFlatStat(Stat stat);
        public abstract float ReflectPercentStat(Stat stat);

    }
}
