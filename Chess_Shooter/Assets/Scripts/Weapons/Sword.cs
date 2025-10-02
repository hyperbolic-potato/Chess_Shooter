using UnityEngine;

public class Sword : Weapon
{
    public override void fire()
    {
        if (canFire && !reloading && (clip > 0 || clipSize == -1) && weaponID > -1)
        {
            weaponSpeaker.Play();
            GameObject p = Instantiate(projectile, firePoint.position, firePoint.rotation);
            p.transform.parent = transform;

            

            p.transform.localRotation *= Quaternion.AngleAxis(Random.Range(45f, -45f), Vector3.forward);
            p.tag = "PlayerDamage";
            Destroy(p, projLifespan);
            if (clipSize != -1) clip--;
            canFire = false;
            StartCoroutine("cooldownFire", rof);
        }
    }
}
