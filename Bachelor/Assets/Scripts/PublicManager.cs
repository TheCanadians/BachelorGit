using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicManager : MonoBehaviour {

    [SerializeField]
    public float turnSpeed = 0f;
    [SerializeField]
    public float acceleration = 0f;
    [SerializeField]
    public float maxSpeed = 0f;
    [SerializeField]
    public float timeToDeath = 0f;

    [SerializeField]
    public float mutationProbability = 0f;
    [SerializeField]
    public float mutationAmount = 0f;

    [SerializeField]
    public int population = 0;
    [SerializeField]
    public float crossoverProbability = 0f;
    [SerializeField]
    public int[] layers;
    [SerializeField]
    public CrossoverType crossType;





    [SerializeField]
    public enum CrossoverType
    {
        Elitist,Pooling
    }
    
}
