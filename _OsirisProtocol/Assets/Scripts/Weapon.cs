using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string Name;
    public enum WeaponCategory
    {
        Rifle,
        Subfusil,
        Pistola
    }
    public  WeaponCategory Category;
    public float damage;
    public float FireRate;
    public int magazineCapacity;
    public float RecoilAmount;
    public float reloadSpeed;
}
