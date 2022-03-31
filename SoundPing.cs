using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPing : SoundSource
{
    public override void Start()
    {
        base.Start();
        StartCoroutine(DestroySelf());
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
