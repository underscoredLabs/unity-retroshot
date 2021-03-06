﻿using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InGameMenuManager : MonoBehaviour
{
    [Tooltip("Root GameObject of the menu used to toggle its activation")]
    public GameObject menuRoot;
    [Tooltip("Master volume when menu is open")]
    [Range(0.001f, 1f)]
    public float volumeWhenMenuOpen = 0.5f;
    [Tooltip("Slider component for look sensitivity")]
    public Slider lookSensitivitySlider;
    [Tooltip("Slider component for run speed")]
    public Slider runSpeedSlider;
    [Tooltip("Slider component for air speed")]
    public Slider jumpForceSlider;
    [Tooltip("Slider component for gravity")]
    public Slider gravitySlider;
    [Tooltip("Toggle component for shadows")]
    public Toggle shadowsToggle;
    [Tooltip("Toggle component for invincibility")]
    public Toggle invincibilityToggle;
    [Tooltip("Toggle component for framerate display")]
    public Toggle framerateToggle;
    [Tooltip("GameObject for the controls")]
    public GameObject controlImage;

    PlayerInputHandler m_PlayerInputsHandler;
    PlayerCharacterController m_PlayerCharacterController;
    Health m_PlayerHealth;
    FramerateCounter m_FramerateCounter;

    void Start()
    {
        m_PlayerInputsHandler = FindObjectOfType<PlayerInputHandler>();
        DebugUtility.HandleErrorIfNullFindObject<PlayerInputHandler, InGameMenuManager>(m_PlayerInputsHandler, this);

        m_PlayerCharacterController = FindObjectOfType<PlayerCharacterController>();

        m_PlayerHealth = m_PlayerInputsHandler.GetComponent<Health>();
        DebugUtility.HandleErrorIfNullGetComponent<Health, InGameMenuManager>(m_PlayerHealth, this, gameObject);

        m_FramerateCounter = FindObjectOfType<FramerateCounter>();
        DebugUtility.HandleErrorIfNullFindObject<FramerateCounter, InGameMenuManager>(m_FramerateCounter, this);

        menuRoot.SetActive(false);

        // if the user saved a value
        if (GlobalPlayerSettings.lookSensitivity > 0) {
          // change slider to saved value
         lookSensitivitySlider.value = GlobalPlayerSettings.lookSensitivity;
         // change player sensivity to saved value
          m_PlayerInputsHandler.lookSensitivity = GlobalPlayerSettings.lookSensitivity;
        } else {
          // default is 1
          lookSensitivitySlider.value = m_PlayerInputsHandler.lookSensitivity;
        }

        lookSensitivitySlider.value = m_PlayerInputsHandler.lookSensitivity;
        lookSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);

        runSpeedSlider.value = m_PlayerCharacterController.maxSpeedOnGround;
        runSpeedSlider.onValueChanged.AddListener(OnRunSpeedChanged);

        jumpForceSlider.value = m_PlayerCharacterController.jumpForce;
        jumpForceSlider.onValueChanged.AddListener(OnJumpForceChanged);

        gravitySlider.value = m_PlayerCharacterController.gravityDownForce;
        gravitySlider.onValueChanged.AddListener(OnGravityChanged);

        shadowsToggle.isOn = QualitySettings.shadows != ShadowQuality.Disable;
        shadowsToggle.onValueChanged.AddListener(OnShadowsChanged);

        invincibilityToggle.isOn = m_PlayerHealth.invincible;
        invincibilityToggle.onValueChanged.AddListener(OnInvincibilityChanged);

        framerateToggle.isOn = m_FramerateCounter.uiText.gameObject.activeSelf;
        framerateToggle.onValueChanged.AddListener(OnFramerateCounterChanged);
    }

    private void Update()
    {
        // Lock cursor when clicking outside of menu
        if (!menuRoot.activeSelf && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetButtonDown(GameConstants.k_ButtonNamePauseMenu)
            || (menuRoot.activeSelf && Input.GetButtonDown(GameConstants.k_ButtonNameCancel)))
        {
            if (controlImage.activeSelf)
            {
                controlImage.SetActive(false);
                return;
            }

            SetPauseMenuActivation(!menuRoot.activeSelf);

        }

        if (Input.GetAxisRaw(GameConstants.k_AxisNameVertical) != 0)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                lookSensitivitySlider.Select();
            }
        }
    }

    public void ClosePauseMenu()
    {
        SetPauseMenuActivation(false);
    }

    void SetPauseMenuActivation(bool active)
    {
        menuRoot.SetActive(active);

        if (menuRoot.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0f;
            AudioUtility.SetMasterVolume(volumeWhenMenuOpen);

            EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f;
            AudioUtility.SetMasterVolume(1);
        }

    }

    void OnMouseSensitivityChanged(float newValue)
    {
        m_PlayerInputsHandler.lookSensitivity = newValue;
        // update saved value
        GlobalPlayerSettings.lookSensitivity = newValue;
    }

    void OnRunSpeedChanged(float newValue)
    {
        m_PlayerCharacterController.maxSpeedOnGround = newValue;
        GlobalPlayerSettings.runSpeed = newValue;
    }

    void OnJumpForceChanged(float newValue)
    {
        m_PlayerCharacterController.jumpForce = newValue;
        GlobalPlayerSettings.jumpForce = newValue;
    }

    void OnGravityChanged(float newValue)
    {
        m_PlayerCharacterController.gravityDownForce = newValue;
        GlobalPlayerSettings.gravity = newValue;
    }

    void OnShadowsChanged(bool newValue)
    {
        QualitySettings.shadows = newValue ? ShadowQuality.All : ShadowQuality.Disable;
    }

    void OnInvincibilityChanged(bool newValue)
    {
        m_PlayerHealth.invincible = newValue;
    }

    void OnFramerateCounterChanged(bool newValue)
    {
        m_FramerateCounter.uiText.gameObject.SetActive(newValue);
    }

    public void OnShowControlButtonClicked(bool show)
    {
        controlImage.SetActive(show);
    }
}
