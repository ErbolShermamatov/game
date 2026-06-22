using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject arrowPrefab;

    public float fireRate = 0.6f;
    private float nextFireTime = 0f;

    public float arrowBaseSpeed = 25f;
    public float aimDelayAfterStop = 0.15f;

    public int currentArrows = 10;
    public TextMeshProUGUI ammoText;

    private Animator anim;
    private PlayerAiming playerAiming;
    private Vector2 queuedLaunchVelocity;

    private float stopTimer = 0f;

    public bool IsFiringNow
    {
        get
        {
            if (anim == null) return false;
            return anim.GetCurrentAnimatorStateInfo(0).IsName("Archer_Fire") ||
                   anim.GetCurrentAnimatorStateInfo(0).IsName("Archer_QuickDraw");
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        playerAiming = GetComponent<PlayerAiming>();
        UpdateAmmoUI();
    }

    void Update()
    {
        if (playerAiming == null) return;

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

        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime)
        {
            if (currentArrows > 0)
            {
                nextFireTime = Time.time + fireRate;

                RotateTowardsMouse();
                queuedLaunchVelocity = CalculateBallisticVelocity(firePoint.position, playerAiming.GetAimPosition());

                anim.SetTrigger("Shoot");
            }
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
        currentArrows--;
        UpdateAmmoUI();

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CollectibleArrow"))
        {
            currentArrows++;
            UpdateAmmoUI();

            if (collision.name == "PickupZone")
            {
                Destroy(collision.transform.parent.gameObject);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = "x" + currentArrows;
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