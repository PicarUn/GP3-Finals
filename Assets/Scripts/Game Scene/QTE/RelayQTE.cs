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

    [HideInInspector] 
    public int targetSuccesses = 1; 
    private int currentSuccesses = 0;

    void OnEnable()
    {
        pointer.position = pointA.position;
        targetPos = pointB.position;
        currentSuccesses = 0; 
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
            currentSuccesses++;
            Debug.Log($"QTE Hit! ({currentSuccesses}/{targetSuccesses})");

            if (currentSuccesses >= targetSuccesses)
            {
                Debug.Log("Task Complete!");
                CloseMinigame(true);
            }
        }
        else
        {
            Debug.Log("Failed! Resetting progress.");
           
            currentSuccesses = 0; 
        }
    }

    void CloseMinigame(bool success)
    {
        gameObject.SetActive(false);
        playerInteract.EndMinigame(success);
    }
}