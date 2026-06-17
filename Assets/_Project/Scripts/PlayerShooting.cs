using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject arrowPrefab;

    public float arrowBaseSpeed = 25f;
    
    public float aimDelayAfterStop = 0.15f; 

    private Animator anim;
    private PlayerAiming playerAiming;
    private Vector2 queuedLaunchVelocity;

    private bool isTriggeringShot = false;
    private float stopTimer = 0f; 

    public bool IsFiringNow
    {
        get
        {
            if (anim == null) return false;
            return anim.GetCurrentAnimatorStateInfo(0).IsName("Archer_Fire") ||
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Archer_QuickDraw") ||
                   isTriggeringShot;
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        playerAiming = GetComponent<PlayerAiming>();
    }

    void Update()
    {
        if (playerAiming == null) return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Archer_Fire") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Archer_QuickDraw"))
        {
            isTriggeringShot = false;
        }

        float horizontalMove = Input.GetAxisRaw("Horizontal");
        bool isMoving = Mathf.Abs(horizontalMove) > 0.1f;

        if (isMoving)
        {
            stopTimer = 0f; 
        }
        else
        {
            stopTimer += Time.deltaTime; 
        }

        if (stopTimer < aimDelayAfterStop || IsFiringNow)
        {
            anim.SetBool("IsAiming", false);
        }
        else
        {
            anim.SetBool("IsAiming", playerAiming.IsAiming());
        }

        if (playerAiming.IsAiming() && Input.GetButtonDown("Fire1"))
        {
            if (IsFiringNow) return;

            isTriggeringShot = true;

            RotateTowardsMouse();

            queuedLaunchVelocity = CalculateBallisticVelocity(firePoint.position, playerAiming.GetAimPosition());

            anim.SetTrigger("Shoot");
        }
    }

    void RotateTowardsMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float directionToMouse = mouseWorldPos.x - transform.position.x;
        Vector3 currentScale = transform.localScale;

        if (directionToMouse > 0.1f)
        {
            currentScale.x = Mathf.Abs(currentScale.x);
        }
        else if (directionToMouse < -0.1f)
        {
            currentScale.x = -Mathf.Abs(currentScale.x);
        }
        transform.localScale = currentScale;
    }

    public void SpawnArrowEvent()
    {
        GameObject arrowGO = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Arrow arrow = arrowGO.GetComponent<Arrow>();

        Collider2D playerCollider = GetComponent<Collider2D>();
        Collider2D arrowCollider = arrowGO.GetComponent<Collider2D>();
        if (playerCollider != null && arrowCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, arrowCollider);
        }

        if (arrow != null)
        {
            arrow.LaunchBallistic(queuedLaunchVelocity);
        }
    }

    Vector2 CalculateBallisticVelocity(Vector3 start, Vector3 target)
    {
        Vector2 displacement = new Vector2(target.x - start.x, target.y - start.y);
        float distance = displacement.magnitude;

        float time = distance / arrowBaseSpeed;
        if (time < 0.05f) time = 0.05f;

        float gravity = Physics2D.gravity.y * 1f;

        float vX = displacement.x / time;
        float vY = (displacement.y - 0.5f * gravity * time * time) / time;

        return new Vector2(vX, vY);
    }
}