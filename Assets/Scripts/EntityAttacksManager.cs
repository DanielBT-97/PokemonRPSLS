using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLD.Pkmn {
    public class EntityAttacksManager : MonoBehaviour
    {
        #region Type Definitions
        [Serializable]
        public enum Type {
            Rock, Paper, Scisor, Lizard, Spock
        }

        public class Attack {
            public float power;
            public float accuracy;
            public int numberOfTargets;
            public Type type;
            public bool isSpecialAttack;

            public int inflictStatusMask;
            public float inflictStatusProbability;
        }
        #endregion
        
        #region Events
        #endregion

        #region Constants
        #endregion

        #region Serialized Fields
        #endregion

        #region Standard Attributes
        #endregion

        #region Consultors and Modifiers
        #endregion

        #region API Methods
        #endregion

        #region Unity Lifecycle
        #endregion

        #region Unity Callback
        #endregion

        #region Other methods
        #endregion

    }
}