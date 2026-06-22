using UnityEngine;
using TMPro;

public class PlayerShooting : MonoBehaviour
{
    public Transform firePoint;
    public GameObject arrowPrefab;
    public float fireRate = 0.6f;
    public float arrowBaseSpeed = 25f;
    
    public int currentArrows = 10;
    public TextMeshProUGUI ammoText;

    public float aimDelayAfterStop = 0.15f;

    private Animator anim;
    private PlayerAiming playerAiming;
    private Vector2 queuedLaunchVelocity;
    private Camera mainCam;
    
    private float nextFireTime = 0f;
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
        mainCam = Camera.main;
        UpdateAmmoUI();
    }

    void Update()
    {
        if (playerAiming == null) return;

        UpdateAimingAnimationState();
        HandleShootingInput();
    }

    private void UpdateAimingAnimationState()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        bool isMoving = Mathf.Abs(horizontalMove) > 0.1f;

        if (isMoving) stopTimer = 0f;
        else stopTimer += Time.deltaTime;

        if (stopTimer < aimDelayAfterStop || IsFiringNow)
        {
            anim.SetBool("IsAiming", false);
        }
        else
        {
            anim.SetBool("IsAiming", playerAiming.IsAiming());
        }
    }

    private void HandleShootingInput()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextFireTime && currentArrows > 0)
        {
            nextFireTime = Time.time + fireRate;

            RotateTowardsMouse();
            queuedLaunchVelocity = CalculateBallisticVelocity(firePoint.position, playerAiming.GetAimPosition());

            anim.SetTrigger("Shoot");
        }
    }

    private void RotateTowardsMouse()
    {
        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        float directionToMouse = mouseWorldPos.x - transform.position.x;
        
        Vector3 currentScale = transform.localScale;
        if (directionToMouse > 0.1f) currentScale.x = Mathf.Abs(currentScale.x);
        else if (directionToMouse < -0.1f) currentScale.x = -Mathf.Abs(currentScale.x);
        
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

        if (arrow != null) arrow.LaunchBallistic(queuedLaunchVelocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("CollectibleArrow"))
        {
            currentArrows++;
            UpdateAmmoUI();

            if (collision.name == "PickupZone" && collision.transform.parent != null)
            {
                Destroy(collision.transform.parent.gameObject);
            }
            else
            {
                Destroy(collision.gameObject);
            }
        }
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null) ammoText.text = "x" + currentArrows;
    }

    private Vector2 CalculateBallisticVelocity(Vector3 start, Vector3 target)
    {
        Vector2 displacement = new Vector2(target.x - start.x, target.y - start.y);
        float time = Mathf.Max(displacement.magnitude / arrowBaseSpeed, 0.05f);

        float vX = displacement.x / time;
        float vY = (displacement.y - 0.5f * Physics2D.gravity.y * time * time) / time;

        return new Vector2(vX, vY);
    }
}