using UnityEngine;

public class GameAudio : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySFX("humSFX");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
