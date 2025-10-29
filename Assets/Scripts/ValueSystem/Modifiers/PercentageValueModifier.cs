using UnityEngine;

namespace ValueSystem.Modifiers
{
    [CreateAssetMenu(fileName = "newPercentageModifier", menuName = "SO/Modifiers/PercentageModifier")]
    public class PercentageValueModifier : ValueModifier<float>
    {
        public override float ApplyModifier(float valueToModify)
        {
            return valueToModify + (currentModifier * valueToModify);
        }

        public override string ToString()
        {
            return negativeModifier ? "-" : "+" + currentModifier + "%";
        }

        public override void ResetModifier()
        {
            baseModifier = 0f;
            currentModifier = baseModifier;
            modifierSymbol = "%";
        }
    }
}