using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
using TMPro;

public class BlackoutManager : MonoBehaviour
{
    [Header("Lighting References")]
    public Light[] spotlights; 
    public Renderer[] lightBulbs; 

    [Header("Blackout Settings")]
    [Range(0f, 1f)]
    public float blackoutChancePerSecond = 0.05f; 
    public float repairTimeLimit = 30f; 

    [Header("UI References")]
    public GameObject timerUIContainer; 
    public TextMeshProUGUI timerText; 

    [Header("Game Over References")]
    public GameObject gameOverPanel; 
    public PlayerInteract playerInteract; 
    public PlayerCam playerCam; 

    public bool isLightOut { get; private set; } = false;
    private float timer = 0f;
    private bool isGameOver = false;

    void Start()
    {
       
        if (timerUIContainer != null) 
        {
            timerUIContainer.SetActive(false);
        }
    }

    void Update()
    {
        if (isGameOver) return; 

        if (!isLightOut)
        {
            if (Random.value < blackoutChancePerSecond * Time.deltaTime)
            {
                TurnOffLights();
            }
        }
        else
        {
            
            timer -= Time.deltaTime;
            
           
            if (timerText != null)
            {
               
                timerText.text = Mathf.CeilToInt(timer).ToString() + "s"; 
            }
            
            if (timer <= 0)
            {
                TriggerGameOver();
            }
        }
    }

    public void TurnOffLights()
    {
        isLightOut = true;
        timer = repairTimeLimit; 
        
       
        if (timerUIContainer != null) timerUIContainer.SetActive(true);

        ToggleLightingSetup(false);
    }

    public void TurnOnLights()
    {
        isLightOut = false;
        
        
        if (timerUIContainer != null) timerUIContainer.SetActive(false);

        ToggleLightingSetup(true);
    }

    private void ToggleLightingSetup(bool isOn)
    {
        foreach (Light spot in spotlights)
        {
            if (spot != null) spot.enabled = isOn;
        }

        foreach (Renderer bulb in lightBulbs)
        {
            if (bulb != null)
            {
                if (isOn) bulb.material.EnableKeyword("_EMISSION");
                else bulb.material.DisableKeyword("_EMISSION");
            }
        }
    }

    private void TriggerGameOver()
    {
        isGameOver = true;
        
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (playerInteract != null)
        {
            playerInteract.playerMovementScript.enabled = false;
            playerInteract.playerRb.isKinematic = true;
            playerInteract.enabled = false; 
        }

        if (playerCam != null) playerCam.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}