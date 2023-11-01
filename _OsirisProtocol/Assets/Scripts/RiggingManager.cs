using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
public class RiggingManager : MonoBehaviour
{
    public MultiRotationConstraint MultiRotationConstraint;
    public void SetConstraintWeight(float newWeight)
    {
        if (MultiRotationConstraint != null)
        {
            MultiRotationConstraint.weight = Mathf.Clamp01(newWeight);
        }
        else
        {
            Debug.LogError("Missing MultiRotationConstraint.");
        }
    }
}
