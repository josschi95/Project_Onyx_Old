using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AimCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    PlayerCombat playerCombat;
    Cinemachine3rdPersonFollow vcam;
    [HideInInspector] public bool moveRight = true;
    [HideInInspector] public bool aimCamActive = false;

    IEnumerator zoomInCoroutine;
    IEnumerator zoomOutCoroutine;
     
    private void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
        vcam = aimVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        zoomInCoroutine = AimZoomIn();
        zoomOutCoroutine = AimzoomOut();
    }

    public IEnumerator AimZoomIn()
    {
        yield return new WaitForSeconds(1.5f);
        StopCoroutine(zoomOutCoroutine);
        if (playerCombat.bowDrawn)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            aimCamActive = true;
        }
    }

    public IEnumerator AimzoomOut()
    {
        yield return new WaitForSeconds(0.5f);
        StopCoroutine(zoomInCoroutine);
        aimVirtualCamera.gameObject.SetActive(false);
        aimCamActive = false;
    }

    void SwitchAimShoulder()
    {
        /*
        if (aimVirtualCamera.gameObject.activeSelf)
        {
            if (moveRight && vcam.CameraSide < 1)
            {
                vcam.CameraSide = Mathf.Lerp(vcam.CameraSide, 1, Time.deltaTime * 10f);
            }
            else if (!moveRight && vcam.CameraSide > 0)
            {
                vcam.CameraSide = Mathf.Lerp(vcam.CameraSide, 0, Time.deltaTime * 10f);
            }
        }
        */
    }
}
