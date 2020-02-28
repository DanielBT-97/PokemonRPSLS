using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLD.Pkmn {
    /// <summary>
    /// This class stores and manages all the information about the entity's attacks.
    /// It has the array of attacks that the entity can use and feeds the information to the turn controller on command.
    /// </summary>
    public class EntityAttacksManager : MonoBehaviour
    {
        //TODO: PASS REGION INTO A DIFFERENT SCRIPT INSIDE THE SAME NAMESPACE AND CREATE IN THE SAME SCRIPT HEREDING CLASSES THAT OVERRIDE AttackEffect IN ORDER TO HAVE NORMAL ATTACKS, HEALINGS AND STATUS ONLY.
        #region Type Definitions
        [Serializable]
        public enum Type {
            Rock, Paper, Scisor, Lizard, Spock
        }

        [Serializable]
        public enum StatusEffectType {
            Burn, Poinon, Confusion, Bleed, Paralyze, Sleep
        }

        [Serializable]
        public class Attack {
            public int power;
            public float accuracy;
            public int staminaCost;
            public int totalPP;
            public int currentPP;
            public int numberOfTargets;
            public Type type;
            public bool isSpecialAttack;

            public int inflictStatusMask;
            public float inflictStatusProbability;

            /// <summary>
            /// Method that manages all the attack's effects such as damage, healing, etc...
            /// This method can be overriten in order to create specific attack effects.
            /// The default one checks for status application effects and manages the attack's damage (Basic attack).
            /// </summary>
            /// <param name="userEntityManager"></param>
            /// <param name="targetEntityManager"></param>
            public virtual void AttackEffect(Entity userEntityManager, Entity targetEntityManager) {
                if(inflictStatusMask != 0) {
                    float randomChance = UnityEngine.Random.Range(0f, 1f);
                    if(randomChance <= inflictStatusProbability) {
                        //Apply Status Effect
                        targetEntityManager.ApplyStatusEffect(inflictStatusMask);
                    }
                }

                targetEntityManager.Hit(this, userEntityManager.EntityStats);
            }
        }
        
        [Serializable]
        public class AttackConfiguration {
            public int _power;
            public float _accuracy;
            public int _maxPP;
            public int _numberOfTargets;
            public Type type;
            public bool isSpecialAttack;

            public StatusEffectType inflictStatusMask;
            public float inflictStatusProbability;
        }
        #endregion
        
        #region Events
        #endregion

        #region Constants
        #endregion

        #region Serialized Fields
        public AttackConfiguration[] _attacksConfigs = new AttackConfiguration[4];  //Basic constant configurations for each attack set in the inspector that is then translated into the Attack class that holds updated info, such as current PP.
        public List<Attack> _unlearnedAttacks;
        #endregion

        #region Standard Attributes
        private Attack[] _attacks = new Attack[4];  //Updated info of all attacks.
        private Attack _attackToUse = null;
        private Entity _targetEntity = null;
        #endregion

        #region Consultors and Modifiers
        public Attack AttackToUse {
            get { return _attackToUse; }
            set { _attackToUse = value; }
        }

        public Entity EntityToAttack {
            get { return _targetEntity; }
            set { _targetEntity = value; }
        }

        public EntityAttacksManager.Attack[] GetAttacks() {
            return _attacks;
        }
        #endregion

        #region API Methods
        public bool TryUseAttack(int attackIndex) {
            bool canAttack = true;

            if(_attacks[attackIndex].currentPP <= 0) {
                canAttack = false;  //Attack does not have enough PP so do nothing.
            } else {
                WillUseAttack(attackIndex); //Can use the selected attack so we lower PP and set AttackToUse
            }

            return canAttack;
        }
        #endregion

        #region Unity Lifecycle
        #endregion

        #region Unity Callback
        #endregion

        #region Other methods
        private void SetAttacks() {
            //Translate AttackConfiguration array into Attack array. This way I can have different archetype of attacks (damaging, healing, status only) and use polymorphism.
        }

        /// <summary>
        /// This function only lowers the PP of the attack the entity will use and sets the AttackToUse to the one corresponding to the parameter.
        /// </summary>
        /// <param name="attackSelectedIndex">Index of the attack the entity will use.</param>
        private void WillUseAttack(int attackSelectedIndex) {
            --_attacks[attackSelectedIndex].currentPP;

            //Clamp to 0
            if(_attacks[attackSelectedIndex].currentPP < 0) _attacks[attackSelectedIndex].currentPP = 0;    //This is just a security measure for the PP to not go down to 0. (All PP related checks look for [<= 0] instead [= 0] just in case)

            AttackToUse = _attacks[attackSelectedIndex];
        }
        #endregion

    }
}