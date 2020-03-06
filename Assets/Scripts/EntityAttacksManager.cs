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
        #region Events
        #endregion

        #region Constants
        #endregion

        #region Serialized Fields
        public Attacks.AttackConfiguration[] _attacksConfigs = new Attacks.AttackConfiguration[4];  //Basic constant configurations for each attack set in the inspector that is then translated into the Attack class that holds updated info, such as current PP.
        public List<Attacks.Attack> _unlearnedAttacks;
        #endregion

        #region Standard Attributes
        private Attacks.Attack[] _attacks = new Attacks.Attack[4];  //Updated info of all attacks.
        private Attacks.Attack _attackToUse = null;
        private Entity _targetEntity = null;
        #endregion

        #region Consultors and Modifiers
        public Attacks.Attack AttackToUse {
            get { return _attackToUse; }
            set { _attackToUse = value; }
        }

        public Entity EntityToAttack {
            get { return _targetEntity; }
            set { _targetEntity = value; }
        }

        public Attacks.Attack[] GetAttacks() {
            return _attacks;
        }
        #endregion

        #region API Methods
        public void SettupAttacks() {
            SetAttacks();
        }

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
            Debug.Log("SetAttacks");
            //Translate AttackConfiguration array into Attack array. This way I can have different archetype of attacks (damaging, healing, status only) and use polymorphism.
            for (int i = 0; i < _attacksConfigs.Length; i++) {
                switch (_attacksConfigs[i].archetype) {
                    case Attacks.AttackArchetype.Normal: {
                        Attacks.Attack tempAttack = new Attacks.Attack();
                        tempAttack.power = _attacksConfigs[i].power;
                        tempAttack.accuracy = _attacksConfigs[i].accuracy;
                        tempAttack.totalPP = tempAttack.currentPP = _attacksConfigs[i].maxPP;
                        tempAttack.numberOfTargets = _attacksConfigs[i].numberOfTargets;
                        tempAttack.type = _attacksConfigs[i].type;
                        tempAttack.isSpecialAttack = _attacksConfigs[i].isSpecialAttack;
                        tempAttack.inflictStatusMask = GetStatusEffectMaskFromType(_attacksConfigs[i].inflictStatusMask);
                        tempAttack.inflictStatusProbability = _attacksConfigs[i].inflictStatusProbability;

                        _attacks[i] = tempAttack;
                    }
                    break;

                    case Attacks.AttackArchetype.PureStatus: {
                        Attacks.Attack tempAtt = new Attacks.Attack();
                        tempAtt.power = 0;
                        tempAtt.accuracy = _attacksConfigs[i].accuracy;
                        tempAtt.totalPP = tempAtt.currentPP = _attacksConfigs[i].maxPP;
                        tempAtt.numberOfTargets = _attacksConfigs[i].numberOfTargets;
                        tempAtt.type = _attacksConfigs[i].type;
                        tempAtt.isSpecialAttack = false;
                        tempAtt.inflictStatusMask = GetStatusEffectMaskFromType(_attacksConfigs[i].inflictStatusMask);
                        tempAtt.inflictStatusProbability = _attacksConfigs[i].inflictStatusProbability;

                        _attacks[i] = tempAtt;
                    }
                    break;

                    case Attacks.AttackArchetype.Healing: {
                        Attacks.HealingAttack tempHealAttack = new Attacks.HealingAttack();
                        tempHealAttack.clearAllStatusEffects = _attacksConfigs[i].isSpecialAttack;
                        tempHealAttack.hpToRestore = _attacksConfigs[i].power;

                        tempHealAttack.power = 0;
                        tempHealAttack.accuracy = _attacksConfigs[i].accuracy;
                        tempHealAttack.totalPP = tempHealAttack.currentPP = _attacksConfigs[i].maxPP;
                        tempHealAttack.numberOfTargets = _attacksConfigs[i].numberOfTargets;
                        tempHealAttack.type = _attacksConfigs[i].type;
                        tempHealAttack.isSpecialAttack = false;
                        tempHealAttack.inflictStatusMask = GetStatusEffectMaskFromType(_attacksConfigs[i].inflictStatusMask);
                        tempHealAttack.inflictStatusProbability = _attacksConfigs[i].inflictStatusProbability;
                        _attacks[i] = tempHealAttack;
                    }
                    break;
                }
                }
        }

        private int GetStatusEffectMaskFromType(Attacks.StatusEffectType type) {
            int statusMask = 0;
            switch (type) {
                case Attacks.StatusEffectType.Bleed:
                statusMask = Entity.StatusEffects.Bleed;
                break;

                case Attacks.StatusEffectType.Burn:
                statusMask = Entity.StatusEffects.Burn;
                break;

                case Attacks.StatusEffectType.Confusion:
                statusMask = Entity.StatusEffects.Confusion;
                break;

                case Attacks.StatusEffectType.Paralyze:
                statusMask = Entity.StatusEffects.Paralyze;
                break;

                case Attacks.StatusEffectType.Poison:
                statusMask = Entity.StatusEffects.Poison;
                break;

                case Attacks.StatusEffectType.Sleep:
                statusMask = Entity.StatusEffects.Sleep;
                break;
            }
            return statusMask;
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