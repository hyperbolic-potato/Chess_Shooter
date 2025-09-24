using UnityEngine;

public class explosionSelfDestruct : MonoBehaviour
{
    public ParticleSystem p;
    private void Start()
    {
        p = GetComponent<ParticleSystem>();
    }
    void Update()
    {
        if (!p.isPlaying) Destroy(this.gameObject);
    }
}
