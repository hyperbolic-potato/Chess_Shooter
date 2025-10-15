using UnityEngine;
using TMPro;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class KillDoor : MonoBehaviour
{
    public int enemies;
    public int threshold = 1;
    TextMeshPro tmp;
    NavMeshSurface nms;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp = transform.GetChild(0).GetComponent<TextMeshPro>();
        nms = GameObject.FindGameObjectWithTag("Navmesh").GetComponent<NavMeshSurface>();
    }

    // Update is called once per frame
    void Update()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        tmp.text = (enemies - threshold).ToString();
        
        if(enemies <= threshold)
        {
            nms.BuildNavMesh();
            Destroy(gameObject);
        }
    }
}
