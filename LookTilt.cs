using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookTilt : MonoBehaviour
{
    /*
    public float minX = 60;
    public float maxX = 100;

    [Header("Player Settings")]
    [SerializeField] private float sensX = 120;

    float mouseX;

    float multiplier = 0.01f;

    float xRotation;

    private void Start()
    {
        PlayerStats.instance.onDeath += TurnOff;
    }
    private void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse Y");

        xRotation -= mouseX * sensX * multiplier; //OG

        xRotation = Mathf.Clamp(xRotation, minX, maxX);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (!GameMaster.instance.gamePaused && !PlayerStats.instance.isDead)
        {
            transform.localRotation = Quaternion.Euler(xRotation, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }
    }

    private void TurnOff()
    {
        this.enabled = false;
    }
    */
}