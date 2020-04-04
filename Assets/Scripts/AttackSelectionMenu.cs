using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLD.Pkmn {
    public class AttackSelectionMenu : MonoBehaviour
    {
        [Serializable]
        public struct AttackButton {
            public TMPro.TextMeshProUGUI _attackName;
            public TMPro.TextMeshProUGUI _attackPP;
            public UnityEngine.UI.Image _buttonImage;
        }

        [SerializeField] private AttackButton[] _attackButtons = default;
        [SerializeField] private int _userEntityIndex = 0;
        [SerializeField] private TurnController _turnController = null;

        public void SelectAttack(int attackSelectedIndex) {
            _turnController.SelectAttack(_userEntityIndex, attackSelectedIndex);
        }

        public AttackButton GetButton(int buttonIndex) {
            return _attackButtons[buttonIndex];
        }

        public void UpdateAttacksInfo(Attacks.Attack[] attacks) {
            for(int i = 0; i < attacks.Length; ++i) {
                _attackButtons[i]._attackName.text = attacks[i].name;
                _attackButtons[i]._attackPP.text = attacks[i].currentPP + "/" + attacks[i].totalPP;
                _attackButtons[i]._buttonImage.color = GlobalInformation.TypeColors[(int)attacks[i].type];
            }
        }
    }
}
