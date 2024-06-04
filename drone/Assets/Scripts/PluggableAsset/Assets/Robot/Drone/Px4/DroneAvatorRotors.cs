using UnityEngine;
using System.Collections;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;

public class DroneAvatorRotors : MonoBehaviour
{
    private DroneRotor[] rotors;
    public float initial_pos = -0.61f;
    // Use this for initialization
    void Start()
    {
        rotors = this.GetComponentsInChildren<DroneRotor>();
        foreach (var rotor in rotors)
        {
            rotor.Initialize(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y > initial_pos)
        {
            foreach (var rotor in rotors)
            {
                rotor.AddForce(0.5f);
            }
        }
        else
        {
            foreach (var rotor in rotors)
            {
                rotor.AddForce(0.0f);
            }
        }
    }
}
