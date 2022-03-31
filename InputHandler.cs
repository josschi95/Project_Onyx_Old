using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


public class InputHandler : MonoBehaviour
{
    private PlayerControls playerControls;
    AimCameraController aimCameraController;
    PlayerController playerController;
    PlayerCombat playerCombat;
    UiManager UIManager;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        #region - Player Movement -
        playerControls.PlayerMovement.Movement.performed += inputActions => playerController.movementInput = inputActions.ReadValue<Vector2>();
        playerControls.PlayerMovement.Camera.performed += i => playerController.cameraInput = i.ReadValue<Vector2>();
        playerControls.PlayerMovement.Jump.performed += _ => playerController.Jump();
        playerControls.PlayerMovement.Crouch.started += _ => playerController.Crouch();
        playerControls.PlayerMovement.Dodge.performed += _ => playerController.Dodge();
        playerControls.PlayerMovement.Sprint.performed += _ => playerController.Sprint();
        playerControls.PlayerMovement.ToggleWalkRun.started += context => playerController.toggleWalk = !playerController.toggleWalk;
        #endregion

        #region - Player Combat -
        playerControls.PlayerMovement.RightHand.performed += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Primary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Primary();
            }
        };
        playerControls.PlayerMovement.RightHand.canceled += context => playerCombat.ReleaseAttack_Primary();

        playerControls.PlayerMovement.LeftHand.performed += context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Secondary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Secondary();
            }
        };
        playerControls.PlayerMovement.LeftHand.canceled += context => playerCombat.ReleaseAttack_Secondary();
        #endregion

        //I don't know if i'm going to keep this
        playerControls.PlayerMovement.LeftHand.performed += context => aimCameraController.moveRight = !aimCameraController.moveRight;

        #region - UI Controls -
        playerControls.Menu.Menu.started += _ => UIManager.ToggleMenu();
        playerControls.PlayerMovement.Interact.started += context => UiManager.instance.Interact();
        #endregion
    }

    private void OnDisable()
    {
        playerControls.Disable();
        #region - Player Controls -
        playerControls.PlayerMovement.Movement.performed -= inputActions => playerController.movementInput = inputActions.ReadValue<Vector2>();
        playerControls.PlayerMovement.Camera.performed -= i => playerController.cameraInput = i.ReadValue<Vector2>();
        playerControls.PlayerMovement.Jump.performed -= _ => playerController.Jump();
        playerControls.PlayerMovement.Crouch.started -= _ => playerController.Crouch();
        playerControls.PlayerMovement.Dodge.performed -= _ => playerController.Dodge();
        playerControls.PlayerMovement.Sprint.performed -= _ => playerController.Sprint();
        playerControls.PlayerMovement.ToggleWalkRun.started -= context => playerController.toggleWalk = !playerController.toggleWalk;
        #endregion

        #region - Player Combat -
        playerControls.PlayerMovement.RightHand.performed -= context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Primary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Primary();
            }
        };
        playerControls.PlayerMovement.RightHand.canceled -= context => playerCombat.ReleaseAttack_Primary();

        playerControls.PlayerMovement.LeftHand.performed -= context =>
        {
            if (context.interaction is HoldInteraction)
            {
                playerCombat.Hold_Secondary();
            }
            else if (context.interaction is PressInteraction)
            {
                playerCombat.Press_Secondary();
            }
        };
        playerControls.PlayerMovement.LeftHand.canceled -= context => playerCombat.ReleaseAttack_Secondary();
        #endregion

        playerControls.PlayerMovement.LeftHand.performed -= context => aimCameraController.moveRight = !aimCameraController.moveRight;

        #region - UI Controls -
        playerControls.Menu.Menu.started -= _ => UIManager.ToggleMenu();
        playerControls.PlayerMovement.Interact.started -= context => UiManager.instance.Interact();
        #endregion
    }

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        aimCameraController = GetComponent<AimCameraController>();
        playerCombat = GetComponent<PlayerCombat>();
        UIManager = UiManager.instance;
    }
}
