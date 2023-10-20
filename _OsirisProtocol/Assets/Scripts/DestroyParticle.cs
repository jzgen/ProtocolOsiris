using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{
    public float WaitTime;
    void Update()
    {
        StartCoroutine(DestroyAfterWait(WaitTime));
    }

    IEnumerator DestroyAfterWait(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
