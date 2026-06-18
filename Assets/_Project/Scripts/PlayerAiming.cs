using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAiming : MonoBehaviour
{
    public GameObject reticlePrefab;

    public float maxAimRadius = 5f;
    public LayerMask obstacleLayer;

    public float verticalDeadzoneHalfWidth = 20f;

    private GameObject currentReticle;
    private bool isAiming = false;
    private Vector3 targetAimPosition;

    void Start()
    {
        if (reticlePrefab != null)
        {
            currentReticle = Instantiate(reticlePrefab);
            currentReticle.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            isAiming = true;
            currentReticle.SetActive(true);
            UpdateReticlePosition();
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isAiming = false;
            currentReticle.SetActive(false);
        }
    }

    void UpdateReticlePosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector3 offset = mouseWorldPos - transform.position;
        float distance = offset.magnitude;

        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;

        float minDeadZone = 90f - verticalDeadzoneHalfWidth;
        float maxDeadZone = 90f + verticalDeadzoneHalfWidth;

        if (angle > minDeadZone && angle < maxDeadZone)
        {
            angle = (angle < 90f) ? minDeadZone : maxDeadZone;
        }

        float minDownDeadZone = -90f - verticalDeadzoneHalfWidth;
        float maxDownDeadZone = -90f + verticalDeadzoneHalfWidth;

        if (angle > minDownDeadZone && angle < maxDownDeadZone)
        {
            angle = (angle < -90f) ? minDownDeadZone : maxDownDeadZone;
        }

        float radians = angle * Mathf.Deg2Rad;
        Vector3 constrainedOffset = new Vector3(Mathf.Cos(radians), Mathf.Sin(radians), 0f) * distance;

        Vector3 desiredPos = transform.position + constrainedOffset;
        if (distance > maxAimRadius)
        {
            desiredPos = transform.position + (constrainedOffset.normalized * maxAimRadius);
        }

        RaycastHit2D hit = Physics2D.Linecast(transform.position, desiredPos, obstacleLayer);

        if (hit.collider != null)
        {
            targetAimPosition = hit.point;
        }
        else
        {
            targetAimPosition = desiredPos;
        }

        currentReticle.transform.position = targetAimPosition;

        currentReticle.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void LateUpdate()
{
    if (isAiming)
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        bool isMoving = Mathf.Abs(horizontalMove) > 0.1f;

        if (!isMoving)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float directionToMouse = mouseWorldPos.x - transform.position.x;
            Vector3 scale = transform.localScale;

            if (directionToMouse > 0.1f)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else if (directionToMouse < -0.1f)
            {
                scale.x = -Mathf.Abs(scale.x);
            }

            transform.localScale = scale;
        }
    }
}

    public Vector3 GetAimPosition()
    {
        return targetAimPosition;
    }

    public bool IsAiming()
    {
        return isAiming;
    }
}
