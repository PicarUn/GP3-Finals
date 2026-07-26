using UnityEngine;

public class WireTask : MonoBehaviour
{
    [Header("Cameras")]
    public Camera minigameCamera; 

    [Header("Wire Components")]
    public SpriteRenderer wireStretchyPart; 
    public Transform wireStartPoint; 
    
    [Header("Connection Target")]
    public Transform correctConnectionPoint; 
    public GameObject lightIndicator; 

    private bool isConnected = false;
    private Vector3 initialPosition;
    private float defaultWireLength;

    void Start()
    {
        initialPosition = transform.position;
        defaultWireLength = wireStretchyPart.size.x;
        
        if (lightIndicator != null) 
        {
            lightIndicator.SetActive(false);
        }
    }

    void OnMouseDrag()
    {
        if (isConnected) return; 

       
        Vector3 mousePos = minigameCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f; 

        transform.position = mousePos;

        
        float distance = Vector3.Distance(wireStartPoint.position, mousePos);
        wireStretchyPart.size = new Vector2(distance, wireStretchyPart.size.y);

       
        Vector3 direction = mousePos - wireStartPoint.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        wireStartPoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnMouseUp()
    {
        if (isConnected) return;

        float snapDistance = 0.5f; 
        
        if (Vector3.Distance(transform.position, correctConnectionPoint.position) < snapDistance)
        {
            transform.position = correctConnectionPoint.position;
            
            float finalDistance = Vector3.Distance(wireStartPoint.position, correctConnectionPoint.position);
            wireStretchyPart.size = new Vector2(finalDistance, wireStretchyPart.size.y);
            
            isConnected = true;
            
            if (lightIndicator != null) 
            {
                lightIndicator.SetActive(true);
            }
        }
        else
        {
            transform.position = initialPosition;
            wireStretchyPart.size = new Vector2(defaultWireLength, wireStretchyPart.size.y); 
            wireStartPoint.rotation = Quaternion.identity;
        }
    }
}