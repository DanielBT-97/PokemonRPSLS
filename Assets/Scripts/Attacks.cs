using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLD.Pkmn {
    public class Attacks : MonoBehaviour
    {
        #region Type Definitions
            [Serializable]
            public enum AttackArchetype {
                Normal, PureStatus, Healing
            }

            [Serializable]
            public enum Type {
                Rock = 0, Paper = 1, Scisor = 2, Lizard = 3, Spock = 4
            }

            [Serializable]
            public enum StatusEffectType {
                Burn, Poison, Confusion, Bleed, Paralyze, Sleep
            }

            [Serializable]
            public class Attack {
                public string name;
                public int power;
                public float accuracy;
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
                        if(randomChance <= inflictStatusProbability || inflictStatusProbability >= 1) {
                            //Apply Status Effect
                            targetEntityManager.ApplyStatusEffect(inflictStatusMask);
                        }
                    }

                    targetEntityManager.Hit(this, userEntityManager.EntityStats);
                }
            }

            public class HealingAttack : Attack {
                public bool clearAllStatusEffects = false;
                public int hpToRestore = 0;

                public override void AttackEffect(Entity userEntityManager, Entity targetEntityManager) {
                    if(clearAllStatusEffects) {
                        userEntityManager.ClearAllStatusEffects();
                    }
                    userEntityManager.RestoreHealth(hpToRestore);
                }
            }
            
            [Serializable]
            public class AttackConfiguration {
                public string name;
                public AttackArchetype archetype;
                public int power;   //Power can be used for healing only attacks as the amount of health to be restored.
                public float accuracy;
                public int maxPP;
                public int numberOfTargets;
                public Type type;
                public bool isSpecialAttack;    //This bool is used for healing only attacks as the bool used for ClearAllStatusEffects.

                public StatusEffectType inflictStatusMask;
                public float inflictStatusProbability;
            }
        #endregion
    }
}
