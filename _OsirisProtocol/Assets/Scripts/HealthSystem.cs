using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("Health Stats")]
    public float maxHealth = 100;
    public float currentHealth;
    public bool isDeath;

    [Header("UI Elments")]
    public GameObject healthbar;
    public Image barFill;

    //Player Components
    Animator animator;

    //References
    private Camera mainCamera;

    //Ragdoll components array
    Rigidbody[] ragdollRigidbodies;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        ActivateRagdoll(false);
    }
    void Start()
    {
        currentHealth = maxHealth;

        mainCamera = Camera.main;
    }
    void Update()
    {
        HealthbarUI();

        if (currentHealth <= 0)
        {
            ActivateRagdoll(true);
            healthbar.SetActive(false);
            isDeath = true;
        }
    }
    public void ApplyDamage(float damageApplied, string impactZone)
    {
        Debug.Log(impactZone);
        currentHealth -= damageApplied;
    }
    void ActivateRagdoll(bool isActive)
    {
        animator.enabled = !isActive;

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = !isActive;
        }
    }
    void HealthbarUI()
    {
        float fillValue = currentHealth / maxHealth;
        barFill.fillAmount = fillValue;

        if (CompareTag("iEnemy"))
        {
            healthbar.transform.rotation = Quaternion.LookRotation(healthbar.transform.position - mainCamera.transform.position);
        }
    }
}
