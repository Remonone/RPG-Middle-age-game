using System;
using UnityEngine;
using Random = System.Random;

namespace Movement.Patrols {
    
    public class AreaPatrol : Patrol{
        
        [SerializeField] private float _rangeX;
        [SerializeField] private float _rangeZ;
        

        public override Vector3 NextPoint() {
            Random random = new Random();
            return new Vector3((float)random.NextDouble() * _rangeX, 1, (float)random.NextDouble() * _rangeZ);
        }
        protected override void DrawGizmos() {
            // TODO
        }
    }
}
