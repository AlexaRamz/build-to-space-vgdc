using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : Tool
{
    private GunData gunData;
    [SerializeField]
    private Transform bulletOrigin;
    PlayerMovement plrMove;
    AudioManager audioManager;
    public float rotationSpeed = 10f;
    int lastFacingDir;

    public override ToolData data
    {
        get
        {
            return gunData;
        }
        set
        {
            if (!(value is GunData))
            {
                Debug.Log("Incorrect data type!");
                return;
            }
            gunData = (GunData)value;
        }
    }

    void Start()
    {
        plrMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        audioManager = GameObject.Find("AudioManager")?.GetComponent<AudioManager>();
        lastFacingDir = plrMove.faceDirection;
    }

    public override bool Use()
    {
        if (!base.Use()) return false;

        Shoot();
        if (audioManager != null)
            audioManager.PlaySFX(gunData.shootSounds[Random.Range(0, gunData.shootSounds.Length)]);
        return true;
    }

    public void Shoot()
    {
        Bullet bullet = Instantiate(gunData.bulletPrefab, bulletOrigin.position, Quaternion.Euler(0, 0, transform.eulerAngles.z - 90)).GetComponent<Bullet>();
        bullet.speed = gunData.bulletSpeed;
        bullet.lifeTime = gunData.bulletLifetime;
        bullet.damage = gunData.bulletDamage;
    }

    void LateUpdate()
    {
        plrMove.UpdateToolFlipY();
        float currentAngle = transform.eulerAngles.z;
        if (plrMove.faceDirection != lastFacingDir)
        {
            currentAngle += 180;
        }

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (plrMove.faceDirection < 0)
        {
            targetAngle = Mathf.Sign(targetAngle) * Mathf.Clamp(Mathf.Abs(targetAngle), 90, 180);
        }
        else
        {
            targetAngle = Mathf.Sign(targetAngle) * Mathf.Clamp(Mathf.Abs(targetAngle), 0, 90);
        }
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newAngle);

        lastFacingDir = plrMove.faceDirection;
    }
}
