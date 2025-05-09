﻿using System;
using UnityEngine;

namespace RPG.Utils {
    
    [Serializable]
    public class Cooldown {
        [SerializeField] private float _timeToReset = 1f;
        
        public float SetTimeToReset { get; init; }

        private float _cooldownTime = -1f;

        public bool IsAvailable => _cooldownTime < Time.time;

        public void Reset() => _cooldownTime = Time.time + _timeToReset;
    }
}
