using UnityEngine;
using TMPro;
using System.Collections;

public class GunSystem : MonoBehaviour
{

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string reloadTrigger = "Reload";
    [SerializeField] private string upperBodyLayerName = "UpperBody";
    [SerializeField] private string reloadStateName = "Reload";
    //[SerializeField] private string reloadingBool = "Reloading";
    int upperBodyLayer = -1;
    int reloadShortHash, reloadFullHash;
    //Reload/Shoot data
    public int damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    //bools for managing reload/shoot
    bool shooting, readyToShoot, reloading;

    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    //graphics
    public GameObject muzzleFlash, bulletHoleGraphic;

    //aiming data
    public Vector3 normalLocalPosition;
    public Vector3 aimingLocalPosition;

    public float aimSmoothing = 10;

    //weapon settings
    public float mouseSensitivity = 1;
    public float weaponSwayAmount = -1;
    Vector2 currentRotation;

    //recoil data
    public bool randomizeRecoil;
    public Vector2 randomRecoilConstraints;
    public Vector2[] recoilPattern;
    Vector3 recoilOffset;

    public float recoilKickBack = 0.25f;
    public float recoilReturnSpeed = 20f;

    private void Update()
    {
        DetermineRotation();
        DetermineAim();
        HandleInput();

        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
    }
    
    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;

        normalLocalPosition = transform.localPosition;

        if (animator)
        {
            upperBodyLayer = animator.GetLayerIndex(upperBodyLayerName);
            if (upperBodyLayer < 0)
            {
                upperBodyLayer = 0;
            }
            reloadShortHash = Animator.StringToHash(reloadStateName);
            reloadFullHash = Animator.StringToHash($"{upperBodyLayerName}.{reloadStateName}");

            
        }
    }
    private void HandleInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && (bulletsLeft < magazineSize || bulletsLeft == 0) && !reloading) Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void DetermineRotation()
    {
        Vector2 mouseAxis = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        mouseAxis *= mouseSensitivity;
        currentRotation += mouseAxis;

        transform.localPosition += (Vector3)mouseAxis * weaponSwayAmount / 1000;

        //transform.root.localRotation = Quaternion.AngleAxis(currentRotation.x, Vector3.up);
        //transform.parent.localRotation = Quaternion.AngleAxis(currentRotation.y, Vector3.right);
    }

    private void DetermineAim()
    {
        Vector3 target = normalLocalPosition;
        if (Input.GetKey(KeyCode.Mouse1) || Input.GetKey("z")) target = aimingLocalPosition;

        Vector3 desiredPosition = Vector3.Lerp(transform.localPosition, target + recoilOffset, Time.deltaTime * aimSmoothing
        );

        transform.localPosition = desiredPosition;
    }

    private void DetermineRecoil()
    {
        //transform.localPosition -= Vector3.forward * 0.1f;
        recoilOffset += Vector3.back * recoilKickBack;

        if (randomizeRecoil)
        {
            float xRecoil = Random.Range(-randomRecoilConstraints.x, randomRecoilConstraints.x);
            float yRecoil = Random.Range(-randomRecoilConstraints.y, randomRecoilConstraints.y);

            Vector2 recoil = new Vector2(xRecoil, yRecoil);

            currentRotation += recoil;
        }
        else
        {
            int currentStep = magazineSize + 1 - bulletsLeft;
            currentStep = Mathf.Clamp(currentStep, 0, recoilPattern.Length - 1);

            currentRotation += recoilPattern[currentStep];
        }
    }

    private void Shoot()
    {
        DetermineRecoil();
        readyToShoot = false;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        Debug.DrawRay(fpsCam.transform.position, direction * range, Color.yellow, 0.2f);

        bool hit = Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy.value == 0 ? ~0 : whatIsEnemy);

        if (hit)
        {
            var target = rayHit.collider.GetComponentInParent<ZombieController>();
            if (target != null) target.TakeDamage(damage);

            if (bulletHoleGraphic)
            {
                Quaternion rot = Quaternion.LookRotation(rayHit.normal);
                var hole = Instantiate(bulletHoleGraphic, rayHit.point + rayHit.normal * 0.001f, rot);
                Destroy(hole, 5f);
            }
        }

        SpawnMuzzleFlash();

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0) Invoke("Shoot", timeBetweenShots);
    }
     void SpawnMuzzleFlash()
    {
        if (!muzzleFlash) return;

        var rot = attackPoint.rotation;
        var flash = Instantiate(muzzleFlash, attackPoint.position, rot, attackPoint);

        var ps = flash.GetComponent<ParticleSystem>();
        if (ps)
        {
            var main = ps.main;
            main.loop = false;
            ps.Play();
            Destroy(flash, main.startLifetime.constantMax + 0.1f);
        }
        else
        {
            Destroy(flash, 0.05f);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        if (reloading) return;
        // if (!animator)
        // {
        //     ReloadFinished();
        //     return;
        // }

        reloading = true;
        readyToShoot = false;

        if(animator)
        {
            animator.ResetTrigger(reloadTrigger);
            animator.SetTrigger(reloadTrigger);
            //animator.CrossFadeInFixedTime(reloadStateName, 0.05f, upperBodyLayer);
        }

        //StopAllCoroutines();
        //StartCoroutine(WaitForReload());
        CancelInvoke("ReloadFinished");
        Invoke("ReloadFinished", reloadTime);
    }
    
    // private IEnumerator WaitForReload()
    // {
    //     yield return new WaitUntil(() =>
    //     {
    //         var st = animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
    //         return st.shortNameHash == reloadShortHash || st.fullPathHash == reloadFullHash;
    //     });

    //     yield return new WaitUntil(() =>
    //     {
    //         var st = animator.GetCurrentAnimatorStateInfo(upperBodyLayer);
    //         return st.normalizedTime >= 1f && !animator.IsInTransition(upperBodyLayer);
    //     });

    //     ReloadFinished();
    // }

    private void ReloadFinished()
    {
        bulletsLeft = magazineSize;
        reloading = false;
        readyToShoot = true;
    }
}
