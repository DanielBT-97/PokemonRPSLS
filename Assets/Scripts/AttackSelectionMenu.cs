using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLD.Pkmn {
    public class AttackSelectionMenu : MonoBehaviour
    {
        [SerializeField] private int _userEntityIndex = 0;
        [SerializeField] private TurnController _turnController = null;

        public void SelectAttack(int attackSelectedIndex) {
            _turnController.SelectAttack(_userEntityIndex, attackSelectedIndex);
        }
    }
}
