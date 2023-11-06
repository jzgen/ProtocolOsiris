using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RiggingManager))]
public class CoverSystem : MonoBehaviour
{
    [Header("Ray Adjustment")]
    public float rayDistance = 0.4f;
    public float distanceToCover = 0.5f;
    private LayerMask coverLayerMask;

    [Header("Cover Interpolation")]
    public float interpolationDuration;
    private float timeElapsed;
    public AnimationCurve interpolationCurve;

    //Position and rotation variables
    [HideInInspector] public Vector3 coverPosition;
    [HideInInspector] public Vector3 lastPosition;
    [HideInInspector] public Quaternion coverRotation;
    [HideInInspector] public Quaternion lastRotation;

    [Header("References")]
    public Transform borderRayOrigin;
    private GameObject hitReferenceObject;

    //Raycast support objects
    private float coverDirection;
    private bool coverClose;
    private bool isCover = false;

    //Debug elements
    Vector3 hitPos; //Temporal, for DrawGizmos

    //Player Components
    Animator animator;
    GamepadHandler gamePad;
    PlayerCharacterController player;
    RiggingManager riggingManager;
    public float rigWeight = 1;

    void Start()
    {
        animator = GetComponent<Animator>();
        gamePad = GetComponent<GamepadHandler>();
        player = GetComponent<PlayerCharacterController>();
        riggingManager = GetComponent<RiggingManager>();

        hitReferenceObject = new GameObject("Hit Object");

        coverLayerMask = LayerMask.GetMask("Covers");
    }

    void Update()
    {
        riggingManager.SetConstraintWeight(rigWeight);

        if (!isCover)
        {
            CoverRayDetector();
        }

        if (coverClose && gamePad.input.characterControls.Cover.triggered && player.currentStates != player.coverState)
        {
            StartCoroutine(MoveToCover());
        }
        else if (gamePad.input.characterControls.Cover.triggered && player.currentStates == player.coverState)
        {
            StartCoroutine(QuitCover());
        }
    }

    IEnumerator MoveToCover()
    {
        animator.applyRootMotion = false;
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        isCover = true;

        rigWeight = 0;

        timeElapsed = 0;

        //Interpolation Move
        while (timeElapsed < interpolationDuration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / interpolationDuration);
            float curveValue = interpolationCurve.Evaluate(t);

            animator.SetBool("IsCover", true);
            animator.SetFloat("playerDirection", coverDirection);
            animator.SetLayerWeight(1, 0);

            Vector3 positionInterpolated = Vector3.Lerp(lastPosition, coverPosition, curveValue);
            Quaternion rotationInterpolated = Quaternion.Slerp(lastRotation, coverRotation, curveValue);

            transform.SetPositionAndRotation(positionInterpolated, rotationInterpolated);

            yield return null;
        }

        //Change To cover
        transform.SetPositionAndRotation(coverPosition, coverRotation);

        player.SetState(player.coverState);
    }

    IEnumerator QuitCover()
    {
        player.animator.SetBool("IsCover", false);
        player.animator.SetLayerWeight(1, 1);
        player.SetState(player.standstate);

        yield return new WaitForSeconds(0.6f);

        rigWeight = 1;
        isCover = false;
    }

    void CoverRayDetector()
    {
        Vector3 rayOrigin = transform.position + new Vector3(0, 0.5f, 0);
        Vector3 rayDirection = transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, rayDistance, coverLayerMask))
        {
            //Temporal, for DrawGizmos
            hitPos = hit.point;
            coverClose = true;

            //Position a empty object in the hit point
            hitReferenceObject.SetActive(true);

            //Align empty object position and rotation to cover
            hitReferenceObject.transform.position = hit.point;
            hitReferenceObject.transform.rotation = Quaternion.LookRotation(hit.normal);

            //Get the player position relative to the empty object local space
            Vector3 playerPosition = transform.position;
            Vector3 positionRelativeToOrigin = hitReferenceObject.transform.InverseTransformPoint(playerPosition);

            //Store the player relative position along the Axis X and set it to aniamtor
            coverDirection = positionRelativeToOrigin.x;
            animator.SetFloat("playerDirection", coverDirection);

            //Get the position to move player behind the cover
            coverPosition = (hit.point + hit.normal * distanceToCover) + (Vector3.up * -0.5f);

            //Get the rotation to look forward the cover
            coverRotation = Quaternion.LookRotation(-hit.normal);

            Debug.DrawLine(rayOrigin, hit.point, Color.blue);
        }
        else
        {
            coverClose = false;
            Debug.DrawLine(rayOrigin, rayOrigin + transform.forward * rayDistance, Color.red);
        }
    }

    void OnDrawGizmos()
    {
        if (coverClose)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(coverPosition, 0.1f);
        }
    }

}
