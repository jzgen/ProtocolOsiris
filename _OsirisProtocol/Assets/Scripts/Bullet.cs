using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject impactPrefab;
    public Weapon currentWeapon;
    private float damage;
    string impactZone;

    private void Start()
    {
        if (currentWeapon != null)
        {
            damage = currentWeapon.damage;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        Instantiate(impactPrefab, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal));
        var targetHealth = other.gameObject.GetComponentInParent<HealthSystem>();
        if (targetHealth != null)
        {
            impactZone = other.gameObject.name;
            targetHealth.ApplyDamage(damage, impactZone);
        }

        Destroy(gameObject);
    }
}
