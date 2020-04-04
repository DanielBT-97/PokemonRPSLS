using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLD.Pkmn {
    public class TurnController : MonoBehaviour
    {
        #region Type Definitions
        #endregion
        
        #region Events
        #endregion

        #region Constants
        #endregion

        #region Serialized Fields
        public Entity _defaultEntity = null;
        public Entity[] _entitiesInCombat = new Entity[2];  //This should change to a list in case I want to implement more than a 1v1.
        public UnityEngine.UI.Slider[] _hpSlider = new UnityEngine.UI.Slider[2];
        public AttackSelectionMenu[] _attackSelectionMenus = new AttackSelectionMenu[2];
        public CombatStatDisplayManager[] _entityInfoDiaplyers = new CombatStatDisplayManager[2];
        #endregion

        #region Standard Attributes
        private int _entitiesReady = 0; //Number of entities that have selected an action this turn. Used to determine when to procede to turn computation.
        #endregion

        #region Consultors and Modifiers
        #endregion

        #region API Methods
        public void SetEntitiesToCombat(Entity player, Entity oponent) {
            player.ResetForCombat();
            oponent.ResetForCombat();
            _entitiesInCombat[0] = player;
            _entitiesInCombat[1] = oponent;
            
            FillEntitiesInfo();
            NextTurn();
        }

        public void StartCombat() {
            
        }

        public void CombatFinished() {
            _entitiesInCombat[0] = _defaultEntity;
            _entitiesInCombat[1] = _defaultEntity;
        }

        public void SelectAttack(int userEntityIndex, int selectedAttackIndex) {
            if(_entitiesInCombat[userEntityIndex].TryUseAttack(selectedAttackIndex)) {  //Attack is available
                HideAttackSelectionMenu(userEntityIndex);
                _entitiesInCombat[userEntityIndex].HasSelectedAttack = true;    //Tell the entity it has selected an available attack.
                SetTarget(userEntityIndex);   //Tell the entity it's intended target for this turn. Set target first before setting it as ready since once both are ready it will immediatelly compute the turns outcome.
                EntityIsReady();    //The entity decided on an action.

                //Disable selection menu. (Selection menu could potentially manage when an attack is not available anymore in order to disable the button functionality)
                //Display waiting for other player to decide.
            } else {
                //Attack isn't available
                    //Since the attack is not available we do nothing
                    //TODO: Maybe display a notification that says not enough PP?
            }
        }
        #endregion

        #region Unity Lifecycle
        #endregion

        #region Unity Callback
        #endregion

        #region Other methods
        private void PopulateEntityInformation(Entity entityToPopulate) {
            //Use GetAttacks and Stats to populate HP and attacks info on both screens.
            Attacks.Attack[] entityAttacks = entityToPopulate.AttacksManager.GetAttacks();
        }

        private void EntityIsReady() {
            ++_entitiesReady;
            if(_entitiesReady == _entitiesInCombat.Length) {
                //All entities are ready (selected an action for this turn)
                ComputeTurn();
            }
        }

        /// <summary>
        /// Temporary method to decide the attacks target, for now the game is based around 1v1 so there will only be one possible target -> Index + 1.
        /// This should be changed for a selection sys
        /// For the buffing/healing effects the AttackEffect override will take care of targeting itself instead of the target.
        /// </summary>
        /// <param name="userEntityIndex">The entity we want to give a target to.</param>
        private void SetTarget(int userEntityIndex) {
            int targetIndex = (userEntityIndex + 1) % _entitiesInCombat.Length; //Loops around the number of targets. Since its a 1v1 we only need to target the next one in the array (0 or 1)
            _entitiesInCombat[userEntityIndex].SetAttacksTarget(_entitiesInCombat[targetIndex]);   //This is a temporary line that decides the attacks target
        }

        /// <summary>
        /// This is called once all entities in play have decided their actions.
        /// </summary>
        private void ComputeTurn() {
            //Go through the entities' attacks in order of speed.
            for(int i = 0; i < _entitiesInCombat.Length; ++i) {
                ComputeEntityAttack(_entitiesInCombat[i]);
            }
            //Call ComputeEntityAttack(entity), which will do its attack effect, one by one.
            //TODO: IT'S IN HERE THAT THE TURN'S ANIMATION/TEXT WILL BE DISPLAYED --> MIGHT BE GOOD IDEA TO CREATE A COROUTINE FOR THE TURN'S ACTION SO THAT WE CAN DISPLAY A TEXT, WAIT A FEW SECONDS AND CONTINUE WITH THE TURN'S COMPUTATIONS.

            DisplayNewHealth();

            //Once everything is done go to next turn.
            NextTurn();
        }

        private void ComputeEntityAttack(Entity user) {
            user.Attack();
        }

        private void NextTurn() {
            _entitiesReady = 0;

            //Call all entities ResetForTurn().
            for(int i = 0; i < _entitiesInCombat.Length; ++i) {
                _entitiesInCombat[i].ResetForTurn();
                
                //Pop the attack selection menu back on.
                ShowAttackSelectionMenu(i);
            }

            FillAttacksInfo();
        }

        [ContextMenu("HealthDisplay")]
        private void DisplayNewHealth() {
            for(int i = 0; i < _entitiesInCombat.Length; ++i) {
                float newValue = _entitiesInCombat[i].Health / (float)(_entitiesInCombat[i].MaxHealth);
                _hpSlider[i].value = newValue;
                _entityInfoDiaplyers[i].EntityInfo.health.text = _entitiesInCombat[i].Health + "/" + _entitiesInCombat[i].MaxHealth;
            }
        }

        [ContextMenu("HitRandom")]
        private void HitRandomEntity() {
            int randomIndex = UnityEngine.Random.Range(0, 2);
            _entitiesInCombat[randomIndex].ReduceHealth(Mathf.FloorToInt(_entitiesInCombat[randomIndex].MaxHealth * 0.1f));
            DisplayNewHealth();
        }
        #endregion

        #region Menus Managers & Info Displays
        private void FillEntitiesInfo() {
            for(int i = 0; i < _entityInfoDiaplyers.Length; ++i) {
                _entityInfoDiaplyers[i].UpdateInfo(_entitiesInCombat[i]);
            }
        }

        private void FillAttacksInfo() {
            for(int userIndex = 0; userIndex < _entitiesInCombat.Length; ++userIndex) {
                _attackSelectionMenus[userIndex].UpdateAttacksInfo(_entitiesInCombat[userIndex].AttacksManager.GetAttacks());
            }
        }

        private void HideAttackSelectionMenu(int userIndex) {
            _attackSelectionMenus[userIndex].gameObject.SetActive(false);
        }

        private void ShowAttackSelectionMenu(int userIndex) {
            _attackSelectionMenus[userIndex].gameObject.SetActive(true);
        }
        #endregion
    }
}