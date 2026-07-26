using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode standKey = KeyCode.Space; 

    [Header("References")]
    public GameObject sitUI;
    public GameObject micUI; 
    public GameObject fuseBoxUI; 
    
    
    public RelayQTE qteScript; 
    public BlackoutManager blackoutManager; 

    public Transform playerBody;  
    public PlayerMovement playerMovementScript;
    public Rigidbody playerRb;

    private bool isSitting = false;
    private bool inMinigame = false;
    private Chair currentChair = null;
    
    
    public enum CurrentTask { None, Mic, Fusebox }
    private CurrentTask activeTask = CurrentTask.None;

    void Update()
    {
        if (inMinigame) return; 

        bool lookingAtMic = false;
        bool lookingAtChair = false;
        bool lookingAtFuseBox = false; 
        Chair seenChair = null;

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            seenChair = hit.collider.GetComponent<Chair>();
            
            if (seenChair != null && !isSitting) 
            {
                lookingAtChair = true;
            }
            else if (hit.collider.CompareTag("Mic"))
            {
                
                if (blackoutManager != null && !blackoutManager.isLightOut)
                {
                    lookingAtMic = true;
                }
            }
            else if (hit.collider.CompareTag("FuseBox"))
            {
                
                if (blackoutManager != null && blackoutManager.isLightOut)
                {
                    lookingAtFuseBox = true;
                }
            }
        }

        if (sitUI != null) sitUI.SetActive(lookingAtChair);
        if (micUI != null) micUI.SetActive(lookingAtMic);
        if (fuseBoxUI != null) fuseBoxUI.SetActive(lookingAtFuseBox);

        if (isSitting)
        {
            playerBody.position = currentChair.sitPoint.position;

            if (lookingAtMic && Input.GetKeyDown(interactKey))
            {
                StartQTE(CurrentTask.Mic);
            }
            else if (Input.GetKeyDown(standKey) || (Input.GetKeyDown(interactKey) && !lookingAtMic))
            {
                StandUp();
            }
        }
        else 
        {
            if (lookingAtChair && Input.GetKeyDown(interactKey))
            {
                SitDown(seenChair);
            }
            else if (lookingAtMic && Input.GetKeyDown(interactKey))
            {
                StartQTE(CurrentTask.Mic);
            }
            else if (lookingAtFuseBox && Input.GetKeyDown(interactKey))
            {
                StartQTE(CurrentTask.Fusebox);
            }
        }
    }

    void StartQTE(CurrentTask taskType) 
    {
        inMinigame = true;
        activeTask = taskType;
        
        if (sitUI != null) sitUI.SetActive(false);
        if (micUI != null) micUI.SetActive(false);
        if (fuseBoxUI != null) fuseBoxUI.SetActive(false);
        
        playerMovementScript.enabled = false;
        playerRb.isKinematic = true; 
        playerRb.linearVelocity = Vector3.zero;

       
        if (taskType == CurrentTask.Fusebox)
        {
            qteScript.targetSuccesses = 3;
        }
        else 
        {
            qteScript.targetSuccesses = 1;
        }

        
        qteScript.gameObject.SetActive(true);
    }

    public void EndMinigame(bool success) 
    {
        inMinigame = false;
        
        
        if (success && activeTask == CurrentTask.Fusebox)
        {
            if (blackoutManager != null) blackoutManager.TurnOnLights();
        }

        activeTask = CurrentTask.None;
        
        if (!isSitting) 
        {
            playerMovementScript.enabled = true;
            playerRb.isKinematic = false;
        }
    }

    void SitDown(Chair chair)
    {
        isSitting = true;
        currentChair = chair;
        playerMovementScript.enabled = false;
        playerRb.isKinematic = true; 
        playerRb.linearVelocity = Vector3.zero; 
    }

    void StandUp()
    {
        isSitting = false;
        playerBody.position = currentChair.dismountPoint.position;
        playerMovementScript.enabled = true;
        playerRb.isKinematic = false;
        currentChair = null;
    }
}