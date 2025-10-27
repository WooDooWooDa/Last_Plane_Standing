using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using ValueSystem.Modifiers;

namespace ValueSystem.Upgrades
{
    [CreateAssetMenu(fileName = "newValueModifierUpgrade", menuName = "SO/Modifiers/Upgrade", order = 99)]
    public class ValueModifierUpgrade : ScriptableObject
    {
        [SerializeField] private string upgradeName;
        [SerializeField] private string upgradeDescription;
        
        [SerializeField] private int currentLevel = 0;
        [SerializeField] private ValueModifier<float> modifierUpgraded;
        [SerializeField] private List<float> modifierUpgradeValues = new() { 1f };

        private int maxLevel => modifierUpgradeValues.Count - 1;

        [Button, ShowIf(nameof(canLevelUp))]
        public void LevelUp()
        {
            if (modifierUpgradeValues.Count <= 0 || !canLevelUp) return;
            currentLevel++;
            
            modifierUpgraded.SetModifierValue(modifierUpgradeValues[currentLevel]);
        }

        [Button]
        public void ResetUpgrade()
        {
            currentLevel = 0;
            modifierUpgraded.SetModifierValue(modifierUpgradeValues[0]);
        }
        
        private bool canLevelUp => modifierUpgraded is not null && currentLevel < maxLevel;

        private void OnValidate()
        {
            if (modifierUpgraded == null)
            {
                Debug.LogWarning("ValueModifierUpgrade needs a value modifier to be upgraded");
                return;
            }
            
            modifierUpgraded.SetModifierValue(modifierUpgradeValues[currentLevel]);
        }
    }
}