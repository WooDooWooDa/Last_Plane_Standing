using System;
using UnityEngine;

namespace ValueSystem.Modifiers
{
    [CreateAssetMenu(fileName = "newMultModifier", menuName = "SO/Modifiers/MultiplyModifier")]
    public class MultiplyValueModifier : ValueModifier<float>
    {
        public override float ApplyModifier(float valueToModify)
        {
            return modifier * valueToModify;
        }

        protected override void Reset()
        {
            modifier = 1f;
            modifierSymbol = "x";
        }
    }
}