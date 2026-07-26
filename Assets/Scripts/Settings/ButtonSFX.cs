using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClip;

    public void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(buttonClip);
        }
    }
}