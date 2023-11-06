using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followBone : MonoBehaviour
{
    public Transform bone;
    public Transform torsoBone;

    // Update is called once per frame
    void Update()
    {
        transform.position = bone.position;
        transform.rotation = torsoBone.rotation;
    }
}
