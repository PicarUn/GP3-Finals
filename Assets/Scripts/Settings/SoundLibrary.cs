using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Library", menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Header("Background Music")]
    public List<Sound> musicTracks = new List<Sound>();

    [Header("Sound Effects")]
    public List<Sound> sfxClips = new List<Sound>();
}
