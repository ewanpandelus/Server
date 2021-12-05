using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private RotateTowardsMouse playerLookRotation;
    [SerializeField] private GameObject firePoint;
    private Material playerMat;
    private bool scaled = true;
    private bool canShoot = true;
    private bool stunned = false;
    private Rigidbody rb;
    private Vector3 movement;
    float BONUS_GRAV = 25f;
    bool inAir = true;
    private float elapsedTime = 0;

    private float stunnedTime = 0;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerMat = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material;
    }
    private void Update()
    {
       
        elapsedTime += Time.deltaTime;
        if (stunned)
        {
            AlleviateStun();
            return;
        }
        movement = MovementInput();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetMouseButtonDown(0)&&canShoot&&scaled)
        {
            StartCoroutine(Shoot());
        }
    }

       
    private Vector3 MovementInput()
    {
        return -transform.forward * Input.GetAxisRaw("Horizontal");
    }
    private void FixedUpdate()
    {
        if (inAir)
        {
            IncreaseGravityScale();
        }
        speed = 10f;
        rb.velocity = new Vector2(movement.x * speed, rb.velocity.y);
        ClientSend.PlayerPosition(elapsedTime);
        ClientSend.PlayerRotation(playerLookRotation.GetRotation().x);
    }
    private void IncreaseGravityScale()
    {
        Vector3 vel = rb.velocity;
        vel.y -= BONUS_GRAV * Time.deltaTime * 3.5f;
        rb.velocity = vel;
    }


    private void Jump()
    {
        if (!inAir)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
    }

   
    private IEnumerator Shoot()
    {
        canShoot = false;
        StartCoroutine(ScaleAnimation(0.5f, transform.localScale, new Vector3(3.5f, 3.5f, 3.5f),true));
        StartCoroutine(ChangeColour(Color.red, 0.5f));
        yield return new WaitUntil(() => scaled == true);
        InstantiateBullet();
        StartCoroutine(ScaleAnimation(0.25f, transform.localScale, new Vector3(2, 2, 2),true));
        StartCoroutine(ChangeColour(Color.white, 0.25f));
        yield return new WaitUntil(() => scaled == true);
        canShoot = true;
    }
    private void InstantiateBullet()
    {
        Vector3 direction = playerLookRotation.GetShotDirection();
        direction.z = 0;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.transform);
        bullet.transform.position = new Vector3(bullet.transform.position.x, bullet.transform.position.y, 0);
        Rigidbody bulletBody = bullet.GetComponent<Rigidbody>();
        bullet.transform.parent = null;
        bulletBody.velocity = direction.normalized * 35;
        Destroy(bullet, 10f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "floor")
        {
            inAir = false;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        inAir = true;
    }
    IEnumerator ScaleAnimation(float time, Vector3 fromScale, Vector3 toScale, bool isShooting)
    {
        float i = 0;
        float rate = 1 / time;
        scaled = false;

        while (i < 1)
        {
            i += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(fromScale, toScale, i);
            yield return 0;
        }
        if (isShooting)
        {
            scaled = true;
        }
  
    }
    private IEnumerator ChangeColour(Color endColour, float time)
    {
        float i = 0;
        float rate = 1 / time;
        scaled = false;
        Color startColor = playerMat.color;
        while (i < 1)
        {
            i += Time.deltaTime * rate;
            playerMat.color = Color.Lerp(startColor, endColour, i);
            yield return 0;
        }
    }
    public void CollisionScale()
    {
        Vector3 currentScale = transform.localScale;
        ScaleAnimation(1f, currentScale, currentScale * 1.5f, true);
    }

    public void SetCanMove(bool _canMove)
    {
        stunned = _canMove;
    }
    public float GetCurrentScale()
    {
        return transform.localScale.x;
    }

    private void AlleviateStun()
    {
        stunnedTime += Time.deltaTime;
        if (stunnedTime > 2)
        {
            stunned = false;
        }
    }
   
}
