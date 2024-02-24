using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour, ITool
{
    private bool readyToUse = false;
    private GunData gunData;
    public ToolData data
    {
        get
        {
            return gunData;
        }
        set
        {
            if (value.GetType() != typeof(GunData))
            {
                Debug.Log("Incorrect data type!");
                return;
            }
            gunData = (GunData)value;
            GetComponent<SpriteRenderer>().sprite = gunData.sprite;
            readyToUse = true;
        }
    }
    Transform bulletOrigin;

    private void Start()
    {
        bulletOrigin = transform.Find("BulletOrigin");
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

    public void Shoot()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.Euler(0, 0, angle);

        Bullet bullet = Instantiate(gunData.bulletPrefab, bulletOrigin.position, Quaternion.Euler(0, 0, angle - 90)).GetComponent<Bullet>();
        bullet.speed = gunData.bulletSpeed;
        bullet.lifeTime = gunData.bulletLifetime;
        bullet.damage = gunData.bulletDamage;
    }
}
