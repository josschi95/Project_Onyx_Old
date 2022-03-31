using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowAnimHelper : MonoBehaviour
{
    public GameObject arrow;

    public void DrawArrow()
    {
        arrow.SetActive(true);
    }

    public void ReleaseArrow()
    {
        arrow.SetActive(false);
    }
}
