
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Movement.Patrols {
    public abstract class Patrol : MonoBehaviour{
        
        public abstract Vector3 NextPoint();
        protected abstract void DrawGizmos();
        
        private void OnDrawGizmos() {
            DrawGizmos();
        }
    }
}
