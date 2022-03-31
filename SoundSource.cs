using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SoundSource : MonoBehaviour
{
    public SphereCollider soundCollider;

    public float soundMultiplier = 1f;

    // Start is called before the first frame update
    public virtual void Start()
    {
        soundCollider = GetComponent<SphereCollider>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.soundDetected = true;
            enemy.soundSourcePosition = transform.position;
        }
    }
}