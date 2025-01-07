using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]  private Transform targetTransform;
    [SerializeField]  private Transform cameraTransform;
    [SerializeField]  private Transform cameraPivotTransform;
    private Transform myTransform;
    private Vector3 cameraTransformPosition;
    [SerializeField] private LayerMask ignoreLayers;
    Vector3 cameraFollowVelocity = Vector3.zero;

    [SerializeField] PlayerMovement playerMovement;

    [SerializeField] float lookSpeed = 200f;
    [SerializeField] float groundedFollowSpeed = 20f;
    [SerializeField] float aerialFollowSpeed = 10f;
    [SerializeField] float pivotSpeed = 100;

    private float targetPosition;

    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -15;
    public float maximumPivot = 25;

    [SerializeField] float cameraSphereRadius = 0.1f;
    [SerializeField] float cameraCollisionOffSet = 0.2f;
    [SerializeField] float minimumCollisionOffset = 0.2f;

    float cameraSideOffset = 0.3f;

    private void Awake()
    {
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
    }


    public void FollowTarget(float delta)
    {
        if (playerMovement.CharacterController.isGrounded)
        {
            Vector3 targetPosition = Vector3.SmoothDamp
                (myTransform.position, targetTransform.position, ref cameraFollowVelocity, groundedFollowSpeed * delta);
            myTransform.position = targetPosition;
        }
        else
        {
            Vector3 targetPosition = Vector3.SmoothDamp
               (myTransform.position, targetTransform.position, ref cameraFollowVelocity, aerialFollowSpeed * delta);
            myTransform.position = targetPosition;
        }
        HandleCameraCollision(delta);
    }

    public void HandleCameraRotation(float delta, float horizontalCameraInput, float verticalCameraInput)
    {
        //lookAngle += (mouseXInput * lookSpeed) / delta;
        //pivotAngle -= (mouseYInput * pivotSpeed) / delta;
        lookAngle += horizontalCameraInput * lookSpeed * delta;
        pivotAngle -= verticalCameraInput * pivotSpeed * delta;

        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        myTransform.rotation = targetRotation;

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = targetRotation;
    }

    private void HandleCameraCollision(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
        direction.Normalize();

        if (Physics.SphereCast
            (cameraPivotTransform.position, cameraSphereRadius, direction, out hit,
            Mathf.Abs(targetPosition), ignoreLayers))
        {
            float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(dis - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.05f);
        //cameraTransformPosition.z = Mathf.SmoothDamp(cameraTransform.localPosition.z,targetPosition,ref followSpeed, delta / 0.05f);
        cameraTransformPosition.x = cameraSideOffset;
        cameraTransform.localPosition = cameraTransformPosition;


    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(cameraPivotTransform.position, cameraSphereRadius);
    }

}
