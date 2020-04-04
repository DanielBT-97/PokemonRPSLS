using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLD.Pkmn {
    public class CombatStatDisplayManager : MonoBehaviour
    {
        [Serializable]
        public struct EntityInfoDisplay {
            public TMPro.TextMeshProUGUI nameAndLevel;
            public TMPro.TextMeshProUGUI health;
        }

        [SerializeField] private EntityInfoDisplay _entityInfo = default;

        public EntityInfoDisplay EntityInfo {
            get { return _entityInfo; }
        }

        public void UpdateInfo(Entity entity) {
            _entityInfo.health.text = entity.Health + "/" + entity.MaxHealth;
            _entityInfo.nameAndLevel.text = entity.EntityStats.entityType.ToString() + "  " + entity.EntityStats.level;
        }
    }
}
