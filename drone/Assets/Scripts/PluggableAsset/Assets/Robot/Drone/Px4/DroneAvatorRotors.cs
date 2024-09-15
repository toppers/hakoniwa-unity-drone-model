using UnityEngine;
using System.Collections;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using Hakoniwa.AR.Core;

public class DroneAvatorRotors : MonoBehaviour, IHakoAvatorState
{
    private AudioSource audioSource;
    public Camera target_camera;
    public string audio_path;
    private bool enableAudio = false;
    private DroneRotor[] rotors;
    public float initial_pos = -0.61f;
    private float my_controls = 0;
    // Use this for initialization
    void Start()
    {
        rotors = this.GetComponentsInChildren<DroneRotor>();
        foreach (var rotor in rotors)
        {
            rotor.Initialize(this);
        }
        audioSource = GetComponent<AudioSource>();
        LoadAudio();
    }
    void LoadAudio()
    {
        AudioClip clip = Resources.Load<AudioClip>(this.audio_path);
        if (clip != null)
        {
            Debug.Log("audio found: " + audio_path);
            audioSource.clip = clip;
            audioSource.Stop();
            enableAudio = true;
        }
        else
        {
            Debug.LogWarning("audio not found: " + audio_path);
        }
    }
    public float maxDistance = 100.0f;
    public float minDistance = 0.0f;

    // Update is called once per frame
    void Update()
    {
        foreach (var rotor in rotors)
        {
            rotor.AddForce(my_controls);
        }
        if (enableAudio == false)
        {
            return;
        }
        // Calculate distance to the target camera
        float distance = Vector3.Distance(target_camera.transform.position, transform.position);

        // Map the distance to volume level
        float volume = 1.0f - Mathf.Clamp01((distance - minDistance) / (maxDistance - minDistance));

        if (audioSource.isPlaying == false && my_controls > 0)
        {
            audioSource.Play();
        }
        else if (audioSource.isPlaying == true && my_controls == 0)
        {
            audioSource.Stop();
        }

        if (audioSource.isPlaying)
        {
            audioSource.volume = volume;
        }
    }

    public void SetState(int state)
    {
        my_controls = ( ((float)(state)) / 100.0f );
    }
}
