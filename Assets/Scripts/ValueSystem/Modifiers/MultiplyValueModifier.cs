using System;
using UnityEngine;

namespace ValueSystem.Modifiers
{
    [CreateAssetMenu(fileName = "newMultModifier", menuName = "SO/Modifiers/MultiplyModifier")]
    public class MultiplyValueModifier : ValueModifier<float>
    {
        public override float ApplyModifier(float valueToModify)
        {
            return valueToModify * currentModifier;
        }

        public override void ResetModifier()
        {
            baseModifier = 1f;
            currentModifier = baseModifier;
            modifierSymbol = "x";
        }
    }
}