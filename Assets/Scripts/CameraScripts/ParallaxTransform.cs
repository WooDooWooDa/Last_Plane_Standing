using System;
using UnityEngine;

public class ParallaxTransform : MonoBehaviour
{
    private void OnEnable()
    {
        TryAddToParallax();
    }

    private void OnDisable()
    {
        ParallaxController.Instance.RemoveFromParallax(transform);
    }

    private void OnDestroy()
    {
        ParallaxController.Instance.RemoveFromParallax(transform);
    }

    private void TryAddToParallax()
    {
        if (ParallaxController.Instance is null)
        {
            Invoke(nameof(TryAddToParallax), 0.05f);
        }
        else
        {
            ParallaxController.Instance.AddToParallax(transform);
        }
    }
}