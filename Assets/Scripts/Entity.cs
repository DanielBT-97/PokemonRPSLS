using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An entity has basic stats like level, hp, etc... but also has a mask for its crrent status effects that is used when calculating damage.
/// 
/// Another aproach would be to have an entity database with every pokemon base stats and have this script use a simple string for the pokemon name/id
/// and use that name to go look for the base stats in that database instead of having multiple instances of this script will all the information and functionality all in one.
/// </summary>
namespace VLD.Pkmn {
    public class Entity : MonoBehaviour
    {
        #region Type Definitions
        [Serializable]
        public class Stats {
            public Attacks.Type entityType;
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
            public static int Poison = 1 << 2;
            public static int Bleed = 1 << 3;
            public static int Paralyze = 1 << 4;
            public static int Sleep = 1 << 5;
        }
        #endregion

        #region Events
        #endregion

        #region Constants
        #endregion

        #region Serialized Fields
        [SerializeField] private EntityAttacksManager _attacksManager = null;
        [SerializeField] private Stats _entityBaseStats = default;
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
            get { return _entityBaseStats; }
        }

        public int Health {
            get { return _currentHealth; }
            set { _currentHealth = value; }
        }

        public int MaxHealth {
            get { return _maxHealth; }
        }

        public bool HasSelectedAttack {
            get { return _hasSelectedAttack; }
            set { _hasSelectedAttack = value; }
        }
        #endregion

        #region API Methods
        public void ResetForCombat() {
            _hasSelectedAttack = false;
            AttacksManager.AttackToUse = null;
            AttacksManager.EntityToAttack = null;
            _maxHealth = CalculateHealth(_entityBaseStats.health, _entityBaseStats.level);   //TODO: move this to a StatInit() funciton that is called once the entity's info is been set.
            Health = _maxHealth;
            AttacksManager.SettupAttacks();
        }

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
        public int Hit(Attacks.Attack attackReceived, Stats attackerStats) {
            int damage = DamageFormula(attackReceived, attackerStats);
            return ReduceHealth(damage);
        }

        /// <summary>
        /// API method used to reduce the entity's health directly.
        /// Should be used when receiving damage by status effects.
        /// </summary>
        /// <param name="damage"></param>
        /// <returns></returns>
        public int ReduceHealth(int damage) {
            Health = Health - damage;
            if(Health < 0) Health = 0;
            return Health;
        }

        public int RestoreHealth(int hpRestored) {
            int newHealth = Health + hpRestored;
            if(newHealth > MaxHealth) newHealth = MaxHealth;

            Health = newHealth;
            return Health;
        }

        //For some reason this is toggling the bits
        public void ApplyStatusEffect(int statusToApply) {
            _entityBaseStats.currentStatusEffects ^= statusToApply;
        }

        public void ClearStatusEffect(int statusToDisable) {
            _entityBaseStats.currentStatusEffects &= ~(statusToDisable);
        }

        public void ClearAllStatusEffects() {
            ClearStatusEffect(Entity.StatusEffects.Bleed | Entity.StatusEffects.Burn | Entity.StatusEffects.Confusion | Entity.StatusEffects.Paralyze | Entity.StatusEffects.Sleep | Entity.StatusEffects.Poison);
        }
        #endregion

        #region Unity Lifecycle
        private void Start() {
            _maxHealth = CalculateHealth(_entityBaseStats.health, _entityBaseStats.level);   //TODO: move this to a StatInit() funciton that is called once the entity's info is been set.
            Health = _maxHealth;
        }
        #endregion

        #region Unity Callback
        #endregion

        #region Other methods
        private bool CheckAttackAvailability(Attacks.Attack attack) {
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
        private float TypeMatchupModifier(Attacks.Type attacker) {
            float matchupMultiplier = 1;
            switch (attacker) {
                case Attacks.Type.Rock:
                    switch (_entityBaseStats.entityType)
                    {
                        case Attacks.Type.Paper:
                            matchupMultiplier = 0.5f;
                            break;
                        case Attacks.Type.Scisor:
                            matchupMultiplier = 2f;
                            break;
                        case Attacks.Type.Lizard:
                            matchupMultiplier = 2f;
                            break;
                        case Attacks.Type.Spock:
                            matchupMultiplier = 0.5f;
                            break;
                    }
                    break;
                case Attacks.Type.Paper:
                    switch (_entityBaseStats.entityType)
                    {
                        case Attacks.Type.Rock:
                            matchupMultiplier = 2f;
                            break;
                        case Attacks.Type.Scisor:
                            matchupMultiplier = 0.5f;
                            break;
                        case Attacks.Type.Lizard:
                            matchupMultiplier = 0.5f;
                            break;
                        case Attacks.Type.Spock:
                            matchupMultiplier = 2f;
                            break;
                    }
                    break;
                case Attacks.Type.Scisor:
                    switch (_entityBaseStats.entityType)
                    {
                        case Attacks.Type.Rock:
                            matchupMultiplier = 0.5f;
                            break;
                        case Attacks.Type.Paper:
                            matchupMultiplier = 2f;
                            break;
                        case Attacks.Type.Lizard:
                            matchupMultiplier = 2f;
                            break;
                        case Attacks.Type.Spock:
                            matchupMultiplier = 0.5f;
                            break;
                    }
                    break;
                case Attacks.Type.Lizard:
                    switch (_entityBaseStats.entityType)
                    {
                        case Attacks.Type.Rock:
                            matchupMultiplier = 0.5f;
                            break;
                        case Attacks.Type.Paper:
                            matchupMultiplier = 2f;
                            break;
                        case Attacks.Type.Scisor:
                            matchupMultiplier = 0.5f;
                            break;
                        case Attacks.Type.Spock:
                            matchupMultiplier = 2f;
                            break;
                    }
                    break;
                case Attacks.Type.Spock:
                    switch (_entityBaseStats.entityType)
                    {
                        case Attacks.Type.Rock:
                            matchupMultiplier = 2f;
                            break;
                        case Attacks.Type.Paper:
                            matchupMultiplier = 0.5f;
                            break;
                        case Attacks.Type.Scisor:
                            matchupMultiplier = 2f;
                            break;
                        case Attacks.Type.Lizard:
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
        private int DamageFormula (Attacks.Attack attackReceived, Stats attackerStats) {
            float calculatedDamage = 0f;

            float crit = (UnityEngine.Random.Range(0f, 1f) > GlobalInformation.CRITCHANCE) ? 1f : 1.5f;     //Crit multiplier with a 10% chance.
            int stab = (attackerStats.entityType == attackReceived.type) ? 2 : 1;   //STAB multiplier depends on whether the attacker and its attack are of the same type.
            float typeMatchupModifier = TypeMatchupModifier(attackerStats.entityType);    //Type matchup multiplier depends on whether the attack's type and its target's has an advantage/disadvantage.
            float burnModifier = (((attackerStats.currentStatusEffects & StatusEffects.Burn) == 0) && !attackReceived.isSpecialAttack) ? 1 : 0.5f;  //If the oponent is burned and the attack is physical the damage is halved.
            float numberOfTargetsModifier = (attackReceived.numberOfTargets == 1) ? 1 : 0.75f;     //If the number of targets is bigger than one the damage is less 25% less in both targets.

            float modifier = numberOfTargetsModifier * crit * stab * typeMatchupModifier * burnModifier;   //Multiplier calculations using the previous calculated modifiers

            //Pokemon damage formula (Divided in parts for easier "readability"):
            float levelDependentValue = ((2 * attackerStats.level) / 5) + 2;    //Part of the damage calculation that depends on the attacker's level.
            float attackVdefense = (attackReceived.isSpecialAttack ? (attackerStats.specialAttack / _entityBaseStats.specialDefense) : (attackerStats.attack / _entityBaseStats.defense));  //Part of the calculation that reduces damage based on attacker's attack and target's defense (Uses att/spatt and def/spdef depending on attack).
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
