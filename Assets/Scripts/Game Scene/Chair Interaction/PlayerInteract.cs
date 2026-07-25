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
        }

       
        sitUI.SetActive(lookingAtChair);
        micUI.SetActive(lookingAtMic);

       
        if (isSitting)
        {
            // Keep the player locked to the chair
            playerBody.position = currentChair.sitPoint.position;

            // If sitting and looking at mic, E starts the minigame
            if (lookingAtMic && Input.GetKeyDown(interactKey))
            {
                StartMinigame();
            }
            // If they press Space, OR if they press E while NOT looking at the mic, they stand up
            else if (Input.GetKeyDown(standKey) || (Input.GetKeyDown(interactKey) && !lookingAtMic))
            {
                StandUp();
            }
        }
        else // If standing up
        {
            if (lookingAtChair && Input.GetKeyDown(interactKey))
            {
                SitDown(seenChair);
            }
            else if (lookingAtMic && Input.GetKeyDown(interactKey))
            {
                StartMinigame();
            }
        }
    }

    void StartMinigame()
    {
        inMinigame = true;
        
        
        sitUI.SetActive(false);
        micUI.SetActive(false);
        
       
        playerMovementScript.enabled = false;
        playerRb.isKinematic = true; 
        playerRb.linearVelocity = Vector3.zero;

        
        qtePanel.SetActive(true);
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