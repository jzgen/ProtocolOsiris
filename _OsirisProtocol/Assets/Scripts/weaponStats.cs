using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponStats : MonoBehaviour
{
    public enum WeaponCategory
    {
        Rifle,
        Subfusil,
        Pistola
    }

    public string Name;
    public  WeaponCategory Category;
    public float Damge;
    public float FireRate;
    public int magazineCapacity;
    public float RecoilAmount;
    public float reloadSpeed;
}
