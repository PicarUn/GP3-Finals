using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;
    public KeyCode interactKey = KeyCode.E;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("References")]
    public GameObject interactUI; // the UI text that says "Press E to Sit"
    public Transform playerBody;  // player capsuel
    public PlayerMovement playerMovementScript;
    public Rigidbody playerRb;

    private bool isSitting = false;
    private Chair currentChair = null;

    void Update()
    {
        if (isSitting)
        {
            HandleSitting();
        }
        else
        {
            HandleLookingForChair();
        }
    }

    void HandleLookingForChair()
    {
        
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            
            Chair chair = hit.collider.GetComponent<Chair>();
            if (chair != null)
            {
                interactUI.SetActive(true); 

                if (Input.GetKeyDown(interactKey))
                {
                    SitDown(chair);
                }
                return;
            }
        }
        
        // if player not looking at a chair, hide ui
        interactUI.SetActive(false);
    }

    void HandleSitting()
    {
        
        playerBody.position = currentChair.sitPoint.position;
        
        // hide ui while sitting
        interactUI.SetActive(false);

        // get up if E or Space is pressed
        if (Input.GetKeyDown(interactKey) || Input.GetKeyDown(jumpKey))
        {
            StandUp();
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