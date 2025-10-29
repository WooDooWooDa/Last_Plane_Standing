using UnityEngine;

namespace ValueSystem.Modifiers
{
    [CreateAssetMenu(fileName = "newAddModifier", menuName = "SO/Modifiers/AddModifier")]
    public class AddValueModifier : ValueModifier<float>
    {
        public override float ApplyModifier(float valueToModify)
        {
            return valueToModify + currentModifier;
        }
        
        public override void ResetModifier()
        {
            baseModifier = 0f;
            currentModifier = baseModifier;
            modifierSymbol = "+";
        }
    }
}