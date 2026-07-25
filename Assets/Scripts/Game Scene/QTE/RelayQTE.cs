using UnityEngine;
using UnityEngine.UI;

public class RelayQTE : MonoBehaviour
{
    [Header("UI References")]
    public Transform pointA;
    public Transform pointB;
    public RectTransform pointer;
    public RectTransform safeZone;

    [Header("Settings")]
    public float speed = 500f;
    private Vector3 targetPos;

    
    public PlayerInteract playerInteract;

    void OnEnable()
    {
        
        pointer.position = pointA.position;
        targetPos = pointB.position;
    }

    void Update()
    {
        
        pointer.position = Vector3.MoveTowards(pointer.position, targetPos, speed * Time.deltaTime);

        if (Vector3.Distance(pointer.position, pointA.position) < 0.1f)
            targetPos = pointB.position;
        else if (Vector3.Distance(pointer.position, pointB.position) < 0.1f)
            targetPos = pointA.position;

        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckSuccess();
        }
    }

    void CheckSuccess()
    {
       
        if (RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointer.position, null))
        {
            Debug.Log("Success! Signal Relayed.");
            CloseMinigame();
        }
        else
        {
            Debug.Log("Failed! Missed the frequency.");
            CloseMinigame();
        }
    }

    void CloseMinigame()
    {
       
        gameObject.SetActive(false);
        playerInteract.EndMinigame();
    }
}