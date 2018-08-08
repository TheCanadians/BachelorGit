using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // radius in which the checkpoint can be collected
    public float radius = 3f;
    // fitness value of this checkpoint. Later checkpoints have higher values
    public float fitnessValue = 1f;
}
