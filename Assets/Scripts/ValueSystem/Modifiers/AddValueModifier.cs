using UnityEngine;

namespace ValueSystem.Modifiers
{
    [CreateAssetMenu(fileName = "newAddModifier", menuName = "SO/Modifiers/AddModifier")]
    public class AddValueModifier : ValueModifier<float>
    {
        public override float ApplyModifier(float valueToModify)
        {
            return valueToModify + modifier;
        }
        
        protected override void Reset()
        {
            modifier = 0f;
            modifierSymbol = "+";
        }
    }
}