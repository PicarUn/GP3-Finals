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
    public MinigameManager minigameManager; 

    public Transform playerBody;  
    public PlayerMovement playerMovementScript;
    public Rigidbody playerRb;
    public GameObject qtePanel; 

    private bool isSitting = false;
    private bool inMinigame = false;
    private Chair currentChair = null;

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
                lookingAtMic = true;
            }
            else if (hit.collider.CompareTag("FuseBox"))
            {
                lookingAtFuseBox = true;
            }
        }

        sitUI.SetActive(lookingAtChair);
        micUI.SetActive(lookingAtMic);
        
        
        if (fuseBoxUI != null) 
        {
            fuseBoxUI.SetActive(lookingAtFuseBox);
        }

        if (isSitting)
        {
           
            playerBody.position = currentChair.sitPoint.position;

            
            if (lookingAtMic && Input.GetKeyDown(interactKey))
            {
                StartMinigame();
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
                StartMinigame();
            }
            
            else if (lookingAtFuseBox && Input.GetKeyDown(interactKey))
            {
                StartWireMinigame();
            }
        }
    }

    void StartMinigame() 
    {
        inMinigame = true;
        
        sitUI.SetActive(false);
        micUI.SetActive(false);
        if (fuseBoxUI != null) fuseBoxUI.SetActive(false);
        
        playerMovementScript.enabled = false;
        playerRb.isKinematic = true; 
        playerRb.linearVelocity = Vector3.zero;

        qtePanel.SetActive(true);
    }

    
    void StartWireMinigame()
    {
        inMinigame = true;
        
        sitUI.SetActive(false);
        micUI.SetActive(false);
        if (fuseBoxUI != null) fuseBoxUI.SetActive(false);
        
        
        playerMovementScript.enabled = false;
        playerRb.isKinematic = true; 
        playerRb.linearVelocity = Vector3.zero;

        
        minigameManager.OpenWireTask();
    }

    public void EndMinigame() 
    {
        inMinigame = false;
        
        if (!isSitting) 
        {
            playerMovementScript.enabled = true;
            playerRb.isKinematic = false;
        }
    }

    
    public void EndWireMinigame()
    {
        inMinigame = false;
        
       
        playerMovementScript.enabled = true;
        playerRb.isKinematic = false;

        
        minigameManager.CloseWireTask();
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