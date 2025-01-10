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

    [SerializeField] float lookSpeed = 100f;
    [SerializeField] float pivotSpeed = 50;
    [SerializeField] float groundedFollowSpeed = 0.15f;
    [SerializeField] float aerialFollowSpeed = 0.25f;


    private float targetPosition;

    private float defaultPosition;
    private float lookAngle;
    private float pivotAngle;
    public float minimumPivot = -15;
    public float maximumPivot = 25;

    private Vector2 currentMouseDelta;
    private Vector2 mouseDeltaVelocity;

    [SerializeField] float cameraSphereRadius = 0.1f;
    [SerializeField] float cameraCollisionOffSet = 0.2f;
    [SerializeField] float minimumCollisionOffset = 0.2f;

    float cameraSideOffset = 0.3f;

    private void Awake()
    {
        myTransform = transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    private void LateUpdate()
    {
        float delta = Time.deltaTime;

        Vector2 mouseInput = new Vector2(InputController.instance.CameraHorizontalInput, InputController.instance.CameraVerticalInput);
        mouseInput = SmoothMouseInput(mouseInput, 0.05f);

        FollowTarget(delta);
        HandleCameraRotation(delta, mouseInput.x, mouseInput.y);
    }

    private Vector2 SmoothMouseInput(Vector2 rawMouseInput, float smoothTime)
    {
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, rawMouseInput, ref mouseDeltaVelocity, smoothTime);
        return currentMouseDelta;
    }


    public void FollowTarget(float delta)
    {
        float followSpeed = playerMovement.CharacterController.isGrounded ? groundedFollowSpeed : aerialFollowSpeed;

        //Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, followSpeed * delta);
        myTransform.position = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameraFollowVelocity, followSpeed * delta);
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
        myTransform.rotation = Quaternion.Slerp(myTransform.rotation, targetRotation, delta * lookSpeed);

        rotation = Vector3.zero;
        rotation.x = pivotAngle;

        targetRotation = Quaternion.Euler(rotation);
        cameraPivotTransform.localRotation = Quaternion.Slerp(cameraPivotTransform.localRotation, targetRotation, delta * pivotSpeed);
    }

    private void HandleCameraCollision(float delta)
    {
        targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = (cameraTransform.position - cameraPivotTransform.position).normalized;

        if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit, Mathf.Abs(defaultPosition), ignoreLayers))
        {
            float distanceToHit = Vector3.Distance(cameraPivotTransform.position, hit.point);
            targetPosition = -(distanceToHit - cameraCollisionOffSet);
        }

        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = -minimumCollisionOffset;
        }

        cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta * 10f);
        cameraTransformPosition.x = Mathf.Lerp(cameraTransform.localPosition.x, cameraSideOffset, delta * 10f);

        cameraTransform.localPosition = cameraTransformPosition;
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(cameraPivotTransform.position, cameraSphereRadius);
    }

}
