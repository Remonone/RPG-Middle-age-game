﻿
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPG.Saving {
    public interface ISaveable {
        /// <summary>
        /// Override to return a JToken representing the state of the IJsonSaveable
        /// </summary>
        /// <returns>A JToken</returns>
        JToken CaptureAsJToken();
        /// <summary>
        /// Restore the state of the component using the information in JToken.
        /// </summary>
        /// <param name="state">A JToken object representing the state of the module</param>
        void RestoreFromJToken(JToken state);
    }
}
