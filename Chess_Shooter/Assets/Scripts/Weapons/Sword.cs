using UnityEngine;
using System.Collections;
public class Sword : Weapon
{
    MeshRenderer mesh;
    protected override void Start()
    {
        base.Start();
        mesh = transform.GetChild(1).GetComponent<MeshRenderer>();
    }
    public override void fire()
    {
        if (canFire && !reloading && (clip > 0 || clipSize == -1) && weaponID > -1)
        {
            StartCoroutine(Swing());
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

    public IEnumerator Swing()
    {
        mesh.enabled = false;
        yield return new WaitForSeconds(projLifespan);
        mesh.enabled = true;
    }
}
