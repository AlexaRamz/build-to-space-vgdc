using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : Tool
{
    private GunData gunData;
    [SerializeField]
    private Transform bulletOrigin;
    Transform plr;
    AudioManager audioManager;

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
        plr = GameObject.Find("Player").transform;
        audioManager = GameObject.Find("AudioManager")?.GetComponent<AudioManager>();
    }

    public override bool Use()
    {
        if (!base.Use()) return false;

        Shoot();
        if (audioManager != null)
            audioManager.PlaySFX(gunData.shootSounds[Random.Range(0, gunData.shootSounds.Length)]);
        return true;
    }

    float angle;
    public void Shoot()
    {
        Bullet bullet = Instantiate(gunData.bulletPrefab, bulletOrigin.position, Quaternion.Euler(0, 0, angle - 90)).GetComponent<Bullet>();
        bullet.speed = gunData.bulletSpeed;
        bullet.lifeTime = gunData.bulletLifetime;
        bullet.damage = gunData.bulletDamage;
    }

    void LateUpdate()
    {
        plr.GetComponent<PlayerMovement>().UpdateToolFlipY();
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - plr.position).normalized;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
