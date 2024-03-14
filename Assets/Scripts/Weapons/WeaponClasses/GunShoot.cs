using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour, ITool
{
    private bool readyToUse = false;
    private GunData gunData;
    [SerializeField]
    private Transform bulletOrigin;
    SpriteRenderer spriteRender;
    Transform plr;

    public ToolData data
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
            spriteRender = GetComponent<SpriteRenderer>();
            spriteRender.sprite = gunData.sprite;
            plr = GameObject.Find("Player").transform;
            readyToUse = true;
        }
    }

    IEnumerator useDelay()
    {
        yield return new WaitForSeconds(gunData.fireCoolDown);
        readyToUse = true;
    }

    public void Use()
    {
        if (readyToUse)
        {
            readyToUse = false;
            StartCoroutine(useDelay());
            Shoot();
        }
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
        Vector2 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - plr.position).normalized;
        angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
