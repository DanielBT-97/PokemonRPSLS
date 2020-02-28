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
        public GameObject[] _possibleEntities;
        public Entity[] _entitiesInCombat;
        #endregion

        #region Standard Attributes
        private int _entitiesReady = 0; //Number of entities that have selected an action this turn. Used to determine when to procede to turn computation.
        #endregion

        #region Consultors and Modifiers
        #endregion

        #region API Methods
        public void SelectAttack(int userEntityIndex, int selectedAttackIndex) {
            if(_entitiesInCombat[userEntityIndex].TryUseAttack(selectedAttackIndex)) {  //Attack is available
                _entitiesInCombat[userEntityIndex].HasSelectedAttack = true;    //Tell the entity it has selected an available attack.
                SetTarget(userEntityIndex);   //Tell the entity it's intended target for this turn. Set target first before setting it as ready since once both are ready it will inmediatelly compute the turns outcome.
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
            EntityAttacksManager.Attack[] entityAttacks = entityToPopulate.AttacksManager.GetAttacks();
        }

        private void EntityIsReady() {
            ++_entitiesReady;
            if(_entitiesReady == _entitiesInCombat.Length) {
                //All entities are ready (selected an action for this turn)
                ComputeTurn();
            }
        }

        /// <summary>
        /// Temporary method to decide the attacks target, for now the game is based around 1v1 so there will only be one possible target.
        /// For the buffing/healing effects the AttackEffect override will take care of targeting itself instead of the target.
        /// </summary>
        /// <param name="userEntityIndex"></param>
        private void SetTarget(int userEntityIndex) {
            int targetIndex = (userEntityIndex + 1) % _entitiesInCombat.Length;
            _entitiesInCombat[userEntityIndex].SetAttacksTarget(_entitiesInCombat[targetIndex]);   //This is a temporary line that decides the attacks target
        }

        /// <summary>
        /// This is called once all entities in play have decided their actions.
        /// </summary>
        private void ComputeTurn() {
            //Go through the entities' attacks in order of speed.
            //Call ComputeEntityAttack(entity), which will do its attack effect, one by one.
            //TODO: IT'S IN HERE THAT THE TURN'S ANIMATION/TEXT WILL BE DISPLAYED --> MIGHT BE GOOD IDEA TO CREATE A COROUTINE FOR THE TURN'S ACTION SO THAT WE CAN DISPLAY A TEXT, WAIT A FEW SECONDS AND CONTINUE WITH THE TURN'S COMPUTATIONS.

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
            }
        }
        #endregion

    }
}