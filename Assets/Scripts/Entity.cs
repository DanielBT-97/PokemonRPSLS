using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity has basic stats like level, hp, etc... but also has a mask for its crrent status effects that is used when calculating damage.
/// </summary>
namespace VLD.Pkmn {
    public class Entity : MonoBehaviour
    {
        #region Type Definitions
        [Serializable]
        public class Stats {
            public EntityAttacksManager.Type entityType;
            public float level;
            public float health;
            public float stamina;
            public float speed;
            public float attack;
            public float specialAttack;
            public float defense;
            public float specialDefense;

            public int currentStatusEffects = 0;
        }

        public static class StatusEffects {
            public static int Burn = 1;
            public static int Confusion = 1 << 1;
            public static int Toxic = 1 << 2;
            public static int Bleed = 1 << 3;
            public static int Paralyzed = 1 << 4;
            public static int Sleep = 1 << 5;
        }
        #endregion
        
        #region Events
        #endregion

        #region Constants
        private float CRITCHANCE = 0.1f;
        #endregion

        #region Serialized Fields
        public Stats _entityStats;
        #endregion

        #region Standard Attributes
        #endregion

        #region Consultors and Modifiers
        #endregion

        #region API Methods
        public float Hit(EntityAttacksManager.Attack attackReceived, Stats attackerStats) {
            float damage = DamageFormula(attackReceived, attackerStats);

            _entityStats.health = Mathf.Clamp(_entityStats.health - damage, 0, 500);
            
            return _entityStats.health;
        }

        public void ApplyStatusEffect(int statusToApply) {
            _entityStats.currentStatusEffects ^= statusToApply;
        }

        public void ClearStatusEffect(int statusToDisable) {
            _entityStats.currentStatusEffects &= ~(statusToDisable);
        }
        #endregion

        #region Unity Lifecycle
        #endregion

        #region Unity Callback
        #endregion

        #region Other methods
        private float TypeMatchupMultiplier(EntityAttacksManager.Type attacker) {
            float matchupMultiplier = 1;
            switch (attacker) {
                case EntityAttacksManager.Type.Rock:
                    switch (_entityStats.entityType)
                    {
                        case EntityAttacksManager.Type.Paper:
                            matchupMultiplier = 0.5f;
                            break;
                        case EntityAttacksManager.Type.Scisor:
                            matchupMultiplier = 2f;
                            break;
                        case EntityAttacksManager.Type.Lizard:
                            matchupMultiplier = 2f;
                            break;
                        case EntityAttacksManager.Type.Spock:
                            matchupMultiplier = 0.5f;
                            break;
                    }
                    break;
                case EntityAttacksManager.Type.Paper:
                    switch (_entityStats.entityType)
                    {
                        case EntityAttacksManager.Type.Rock:
                            matchupMultiplier = 2f;
                            break;
                        case EntityAttacksManager.Type.Scisor:
                            matchupMultiplier = 0.5f;
                            break;
                        case EntityAttacksManager.Type.Lizard:
                            matchupMultiplier = 0.5f;
                            break;
                        case EntityAttacksManager.Type.Spock:
                            matchupMultiplier = 2f;
                            break;
                    }
                    break;
                case EntityAttacksManager.Type.Scisor:
                    switch (_entityStats.entityType)
                    {
                        case EntityAttacksManager.Type.Rock:
                            matchupMultiplier = 0.5f;
                            break;
                        case EntityAttacksManager.Type.Paper:
                            matchupMultiplier = 2f;
                            break;
                        case EntityAttacksManager.Type.Lizard:
                            matchupMultiplier = 2f;
                            break;
                        case EntityAttacksManager.Type.Spock:
                            matchupMultiplier = 0.5f;
                            break;
                    }
                    break;
                case EntityAttacksManager.Type.Lizard:
                    switch (_entityStats.entityType)
                    {
                        case EntityAttacksManager.Type.Rock:
                            matchupMultiplier = 0.5f;
                            break;
                        case EntityAttacksManager.Type.Paper:
                            matchupMultiplier = 2f;
                            break;
                        case EntityAttacksManager.Type.Scisor:
                            matchupMultiplier = 0.5f;
                            break;
                        case EntityAttacksManager.Type.Spock:
                            matchupMultiplier = 2f;
                            break;
                    }
                    break;
                case EntityAttacksManager.Type.Spock:
                    switch (_entityStats.entityType)
                    {
                        case EntityAttacksManager.Type.Rock:
                            matchupMultiplier = 2f;
                            break;
                        case EntityAttacksManager.Type.Paper:
                            matchupMultiplier = 0.5f;
                            break;
                        case EntityAttacksManager.Type.Scisor:
                            matchupMultiplier = 2f;
                            break;
                        case EntityAttacksManager.Type.Lizard:
                            matchupMultiplier = 0.5f;
                            break;
                    }
                    break;
            }

            return matchupMultiplier;
        }

        /// <summary>
        /// Pokemon formula taken from the bulbapedia (https://bulbapedia.bulbagarden.net/wiki/Damage)
        /// </summary>
        /// <param name="attackReceived">Attack received from the attacker.</param>
        /// <param name="attackerStats">Attacker's stats.</param>
        /// <returns>HP's damage received.</returns>
        private float DamageFormula (VLD.Pkmn.EntityAttacksManager.Attack attackReceived, Stats attackerStats) {
            float calculatedDamage = 0f;

            float crit = (UnityEngine.Random.Range(0f, 1f) > 0.1f) ? 1f : 1.5f;     //Crit multiplier with a 10% chance.
            int stab = (attackerStats.entityType == attackReceived.type) ? 2 : 1;   //STAB multiplier depends on whether the attacker and its attack are of the same type.
            float typeMatchupModifier = TypeMatchupMultiplier(attackerStats.entityType);    //Type matchup multiplier depends on whether the attack's type and its target's has an advantage/disadvantage.
            float burnModifier = (((attackerStats.currentStatusEffects & StatusEffects.Burn) == 0) && !attackReceived.isSpecialAttack) ? 1 : 0.5f;  //If the oponent is burned and the attack is physical the damage is halved.
            float numberOfTargetsModifier = (attackReceived.numberOfTargets == 1) ? 1 : 0.75f;     //If the number of targets is bigger than one the damage is less 25% less in both targets.

            float modifier = numberOfTargetsModifier * crit * stab * typeMatchupModifier * burnModifier;   //Multiplier calculations using the previous calculated modifiers

            //Pokemon damage formula (Divided in parts for easier "readability"):
            float levelDependentValue = ((2 * attackerStats.level) / 5) + 2;    //Part of the damage calculation that depends on the attacker's level.
            float attackVdefense = (attackReceived.isSpecialAttack ? (attackerStats.specialAttack / _entityStats.specialDefense) : (attackerStats.attack / _entityStats.defense));  //Part of the calculation that reduces damage based on attacker's attack and target's defense (Uses att/spatt and def/spdef depending on attack).
            calculatedDamage = (( ((levelDependentValue * attackReceived.power * attackVdefense) / 50) + 2 ) * modifier);   //Complete damage formula calculation.

            return calculatedDamage;
        }

        private void DebugBitMask(int mask) {
            Debug.Log(Convert.ToString(mask, 2));
        }
        #endregion

    }
}
