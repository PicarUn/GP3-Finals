using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("References")]
    public GameObject interactUI; 
    public Transform playerBody;  
    public PlayerMovement playerMovementScript;
    public Rigidbody playerRb;
    

    public GameObject qtePanel; 

    private bool isSitting = false;
    private bool inMinigame = false; 
    private Chair currentChair = null;

    void Update()
    {
        if (isSitting)
            HandleSitting();
        else if (inMinigame)
            return; 
        else
            HandleLooking();
    }

    void HandleLooking()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            Chair chair = hit.collider.GetComponent<Chair>();
            
           
            if (chair != null)
            {
                interactUI.SetActive(true);
                if (Input.GetKeyDown(interactKey)) SitDown(chair);
                return;
            }
            else if (hit.collider.CompareTag("Mic")) 
            {
                interactUI.SetActive(true);
                if (Input.GetKeyDown(interactKey)) StartMinigame();
                return;
            }
        }
        
        interactUI.SetActive(false);
    }

   
    void StartMinigame()
    {
        inMinigame = true;
        interactUI.SetActive(false);
        
      
        playerMovementScript.enabled = false;
        playerRb.isKinematic = true; 
        playerRb.linearVelocity = Vector3.zero;

       
        qtePanel.SetActive(true);
    }

    public void EndMinigame()
    {
        inMinigame = false;
        
        
        playerMovementScript.enabled = true;
        playerRb.isKinematic = false;
    }
   

    void HandleSitting()
    {
        playerBody.position = currentChair.sitPoint.position;
        interactUI.SetActive(false);

        if (Input.GetKeyDown(interactKey) || Input.GetKeyDown(jumpKey))
            StandUp();
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