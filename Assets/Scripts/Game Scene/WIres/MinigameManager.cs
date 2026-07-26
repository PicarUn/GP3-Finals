using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    [Header("3D Player References")]
    public GameObject mainCamera;    
    public PlayerCam playerCamScript; 

    [Header("2D Minigame References")]
    public GameObject minigameCamera;
    public GameObject minigameContainer; 

    public void OpenWireTask()
    {
        
        playerCamScript.enabled = false;

        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        
        mainCamera.SetActive(false);
        minigameCamera.SetActive(true);
        minigameContainer.SetActive(true);
    }

    public void CloseWireTask()
    {
        
        playerCamScript.enabled = true;

        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

       
        minigameCamera.SetActive(false);
        minigameContainer.SetActive(false);
        mainCamera.SetActive(true);
    }
}