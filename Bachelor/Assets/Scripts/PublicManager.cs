using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicManager : MonoBehaviour {

    [Header("Car Settings")]
    [SerializeField]
    public float turnSpeed = 0f;
    [SerializeField]
    public float acceleration = 0f;
    [SerializeField]
    public float maxSpeed = 0f;
    [SerializeField]
    public float timeToDeath = 0f;
    [Header("Sensor Settings")]
    [Range(0.0f, 5.0f)]
    public float maxNoise = 0f;
    [Range(-5.0f, 0.0f)]
    public float minNoise = 0f;
    [Header("Neural Network Settings")]
    public ActivationFunction activationFnc;
    [SerializeField]
    public int[] layers;
    [Header("Mutation Settings")]
    [SerializeField]
    public float mutationProbability = 0f;
    [SerializeField]
    public float mutationAmount = 0f;
    [Header("Genetic Algorithm Settings")]
    [SerializeField]
    public int population = 0;
    [SerializeField]
    public CrossoverType crossType;
    [SerializeField]
    public int numberOfTournamentWinners = 4;
    [SerializeField]
    public float crossoverProbability = 0f;
    [SerializeField]
    public bool compareSelection = false;
    [SerializeField]
    public int stopGenerationNumber = 50;

    




    [SerializeField]
    public enum ActivationFunction
    {
        Sigmoid,TanH,SoftSign
    }
    [SerializeField]
    public enum CrossoverType
    {
        Elitist,RouletteWheel,Tournament,Rank,Random
    }
    
}
