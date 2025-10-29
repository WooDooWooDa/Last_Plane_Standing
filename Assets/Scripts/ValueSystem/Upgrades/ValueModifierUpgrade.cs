using System;
using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using ValueSystem.Modifiers;

namespace ValueSystem.Upgrades
{
    [CreateAssetMenu(fileName = "newValueModifierUpgrade", menuName = "SO/Modifiers/ModifierUpgrade", order = 99)]
    public class ValueModifierUpgrade : ScriptableObject
    {
        [SerializeField] private string upgradeName;
        [SerializeField] private string upgradeDescription;
        
        [SerializeField] private int currentLevel = 0;
        [SerializeField] private ValueModifier<float> modifierUpgraded;
        [SerializeField] private List<float> modifierUpgradeValues = new();

        private int maxLevel => modifierUpgradeValues.Count;

        [Button, ShowIf(nameof(canLevelUp))]
        public void LevelUp()
        {
            if (modifierUpgradeValues.Count <= 0 || !canLevelUp) return;
            currentLevel++;
            
            ApplyUpgradeToModifier();
        }

        [Button, ShowIf(nameof(hasModifier))]
        public void ResetUpgrade()
        {
            currentLevel = 0;
            modifierUpgraded?.ResetModifier();
        }
        
        private bool canLevelUp => hasModifier && currentLevel < maxLevel;
        private bool hasModifier => modifierUpgraded is not null;

        private void ApplyUpgradeToModifier()
        {
            if (currentLevel > 0)
                modifierUpgraded.SetModifierValue(modifierUpgradeValues[currentLevel - 1]);
        }
        
        private void OnValidate()
        {
            if (modifierUpgraded == null)
            {
                Debug.LogWarning("ValueModifierUpgrade needs a value modifier to be upgraded");
                return;
            }

            if (currentLevel >= maxLevel)
            {
                Debug.LogWarning("Invalid level... Resetting upgrade");
                ResetUpgrade();
                return;
            }
            
            ApplyUpgradeToModifier();
        }
    }
}