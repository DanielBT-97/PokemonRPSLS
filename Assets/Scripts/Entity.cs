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
            public int level;
            public int health;
            public int speed;
            public int attack;
            public int specialAttack;
            public int defense;
            public int specialDefense;

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
        private const float CRITCHANCE = 0.1f;
        #endregion

        #region Serialized Fields
        [SerializeField] private EntityAttacksManager _attacksManager = null;
        [SerializeField] private Stats _entityStats;
        #endregion

        #region Standard Attributes
        private int _maxHealth = 0;
        private int _currentHealth = 0;
        private bool _hasSelectedAttack = false;
        #endregion

        #region Consultors and Modifiers
        public EntityAttacksManager AttacksManager {
            get { return _attacksManager; }
        }

        public Stats EntityStats {
            get { return _entityStats; }
        }

        public int Health {
            get { return _currentHealth; }
        }

        public bool HasSelectedAttack {
            get { return _hasSelectedAttack; }
            set { _hasSelectedAttack = value; }
        }
        #endregion

        #region API Methods
        public void ResetForTurn() {
            _hasSelectedAttack = false;
            AttacksManager.AttackToUse = null;
            AttacksManager.EntityToAttack = null;
        }

        public bool TryUseAttack(int attackIndex) {
            return AttacksManager.TryUseAttack(attackIndex);
        }

        public void SetAttacksTarget(Entity targetEntity) {
            AttacksManager.EntityToAttack = targetEntity;
        }

        public void Attack() {
            AttacksManager.AttackToUse.AttackEffect(this, AttacksManager.EntityToAttack);
        }
        #endregion
        
        #region API Attack Computation Methods
        /// <summary>
        /// API method used to reduce the entity's health when hit by an attack.
        /// Calculates the damage using the attacker's attack and stats.
        /// </summary>
        /// <param name="attackReceived"></param>
        /// <param name="attackerStats"></param>
        /// <returns></returns>
        public int Hit(EntityAttacksManager.Attack attackReceived, Stats attackerStats) {
            int damage = DamageFormula(attackReceived, attackerStats);
            _entityStats.health = _entityStats.health - damage;
            return _entityStats.health;
        }

        /// <summary>
        /// API method used to reduce the entity's health directly.
        /// Should be used when receiving damage by status effects.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public int ReduceHealth(int damage) {
            _entityStats.health = _entityStats.health - damage;
            return _entityStats.health;
        }

        public int RestoreHealth(int hpRestored) {
            int newHealth =  _entityStats.health + hpRestored;
            if(newHealth > _maxHealth) newHealth = _maxHealth;

            _entityStats.health = newHealth;
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
        private void Start() {
            CalculateHealth(_entityStats.health, _entityStats.level);
        }
        #endregion

        #region Unity Callback
        #endregion

        #region Other methods
        private bool CheckAttackAvailability(EntityAttacksManager.Attack attack) {
            bool canAttack = true;

            if(attack.currentPP <= 0) {
                canAttack = false;
            }

            return canAttack;
        }
        
        private void DebugBitMask(int mask) {
            Debug.Log(Convert.ToString(mask, 2));
        }
        #endregion

        #region Damage Calculations
        private float TypeMatchupModifier(EntityAttacksManager.Type attacker) {
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
        private int DamageFormula (VLD.Pkmn.EntityAttacksManager.Attack attackReceived, Stats attackerStats) {
            float calculatedDamage = 0f;

            float crit = (UnityEngine.Random.Range(0f, 1f) > 0.1f) ? 1f : 1.5f;     //Crit multiplier with a 10% chance.
            int stab = (attackerStats.entityType == attackReceived.type) ? 2 : 1;   //STAB multiplier depends on whether the attacker and its attack are of the same type.
            float typeMatchupModifier = TypeMatchupModifier(attackerStats.entityType);    //Type matchup multiplier depends on whether the attack's type and its target's has an advantage/disadvantage.
            float burnModifier = (((attackerStats.currentStatusEffects & StatusEffects.Burn) == 0) && !attackReceived.isSpecialAttack) ? 1 : 0.5f;  //If the oponent is burned and the attack is physical the damage is halved.
            float numberOfTargetsModifier = (attackReceived.numberOfTargets == 1) ? 1 : 0.75f;     //If the number of targets is bigger than one the damage is less 25% less in both targets.

            float modifier = numberOfTargetsModifier * crit * stab * typeMatchupModifier * burnModifier;   //Multiplier calculations using the previous calculated modifiers

            //Pokemon damage formula (Divided in parts for easier "readability"):
            float levelDependentValue = ((2 * attackerStats.level) / 5) + 2;    //Part of the damage calculation that depends on the attacker's level.
            float attackVdefense = (attackReceived.isSpecialAttack ? (attackerStats.specialAttack / _entityStats.specialDefense) : (attackerStats.attack / _entityStats.defense));  //Part of the calculation that reduces damage based on attacker's attack and target's defense (Uses att/spatt and def/spdef depending on attack).
            calculatedDamage = (( ((levelDependentValue * attackReceived.power * attackVdefense) / 50) + 2 ) * modifier);   //Complete damage formula calculation.
            
            return Mathf.FloorToInt(calculatedDamage);
        }
        #endregion

        #region StatCalculation
        //For now I use this simple health calculation, the rest of stats are set manually. Might use a randomized system for IVs and EVs in each battle and introduce pokemon's formulas.
        private int CalculateHealth(int healthStat, int level) {
            return Mathf.FloorToInt(( (healthStat * 2) / 100 ) + level + 10);
        }
        #endregion
    }
}
