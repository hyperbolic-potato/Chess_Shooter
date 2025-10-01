using UnityEngine;
using System.Collections;
using Unity.AI;
using UnityEngine.SceneManagement;
public class KingBehavior : BasicEnemyController
{

    public float runawayDistance = 5f;
    protected override IEnumerator attack(){
        yield return null;
    }

    protected override void Navigate()
    {
        base.Navigate();

        if (isNavigating)
        {
            Vector3 difference = playerTransform.position - transform.position;

            difference = -difference.normalized;

            agent.destination = transform.position + difference * runawayDistance;
        }
    }

    protected override void Death() //mmm yes my favorite kind of death
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        base.Death();
    }
}
