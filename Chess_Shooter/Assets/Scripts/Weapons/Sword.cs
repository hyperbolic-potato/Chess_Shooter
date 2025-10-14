using UnityEngine;
using System.Collections;
public class Sword : Weapon
{
    MeshRenderer mesh;

    public float angleNoise = 5f;
    public float angleSeverity = 45f;
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

            //me on my way to program a suphisticated, obscure, and unnecessary detail knowing full well noone is going to notice it:

            p.transform.localRotation *= Quaternion.AngleAxis(Random.Range(angleNoise, -angleNoise), Vector3.forward);
            p.transform.localRotation *= Quaternion.AngleAxis(Mathf.Clamp((transform.rotation.eulerAngles.x + 180) % 360, 180f - angleSeverity, 180 + angleSeverity) - 180, Vector3.forward);
            
            //waiter, waiter, more quaternion bullshittery please!

            
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
