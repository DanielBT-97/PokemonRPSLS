using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLD.Pkmn {
    public class CombatCreator : MonoBehaviour
    {
        #region Type Definitions
        #endregion
        
        #region Events
        #endregion

        #region Constants
        #endregion

        #region Serialized Fields
        public TurnController _turnController = null;
        public Entity[] _possibleEntities_Rock;
        public Entity[] _possibleEntities_Paper;
        public Entity[] _possibleEntities_Scisor;
        public Entity[] _possibleEntities_Lizard;
        public Entity[] _possibleEntities_Spock;
        #endregion

        #region Standard Attributes
        #endregion

        #region Consultors and Modifiers
        #endregion

        #region API Methods
        /*
        The combat creation menu is a simulation of the random encounters in pokemon that will feed the entities for combat to the CombatCreator script in order to start with the following steps.
        The combat creation menu can change but its output should always be the same: two or more entity objects.

        This script is the one responsible for creating the combat and starting it once it is ready to start.
        It receives two entities and fills the necessary information on the combat displays and feeds the information to the turn controller.
        The turn controller will then be in charge of the game loop:
            - Both players select an attack
            - Turn Controller computes the attacks effects --> For now the effects are computed instantly. What I can do is modify the base Attacks class to have different API calls for each of the stages --> Attack damage -> Healing effects -> Status Application -> ...
                => This way I can manage the displays (text) so that each stage is done one by one instead of being processed in bulk. (Doing this will be easier to introduce animations)
            - Displays are updated for new health. Attacks are updated for PP left.
            - Turn finishes and restarts the attack selection.
        Turn controller has exit conditions:
            + One of the pokemons faints.
            + Exit combat button. (run away)
            --> Once one of these exit conditions are met, the combat is reset to default to wait for the next one, and we go back to the combat creation menu.
        
        */

        public void SetupCombat(Entity userEntity, Entity oponentEntity) {
            _turnController.SetEntitiesToCombat(userEntity, oponentEntity);
        }
        #endregion

        #region Unity Lifecycle
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            SetupCombat(_turnController._entitiesInCombat[0], _turnController._entitiesInCombat[1]);
        }
        #endregion

        #region Unity Callback
        #endregion

        #region Other methods
        #endregion
    }
}
