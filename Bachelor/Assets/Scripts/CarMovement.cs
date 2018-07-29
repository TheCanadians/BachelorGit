using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {

    // variables for the car movement and time till death of no checkpoint has been collected
    [SerializeField]
    private float maxSpeed = 5f;
    [SerializeField]
    private float acc = 8f;
    [SerializeField]
    private float turnSpeed = 100f;
    [SerializeField]
    private float maxTime = 7f;

    private float velocity;
    private Quaternion rotation;

    private bool init = false;

    private NeuralNetwork net;
    private Rigidbody2D rBody;
    private float[] sensorVals;
    private Sensor[] sensors;

    private int currentCheckpoint = 1;
    private GameObject[] checkpoints;

    public float showFitnessLevel;
    public float showDistance;
    private float fitness = 0f;

    private float[] sensorOutputs;

    public float timeSinceCheckpoint = 0f;

    public bool isAlive = true;

    private PublicManager publicManager;

    private void Start()
    {
        // Get the user set inputs
        publicManager = GameObject.Find("PublicManager").GetComponent<PublicManager>();
        this.maxSpeed = publicManager.maxSpeed;
        this.acc = publicManager.acceleration;
        this.turnSpeed = publicManager.turnSpeed;
        this.maxTime = publicManager.timeToDeath;
        // Ignore Collision with other cars
        rBody = GetComponent<Rigidbody2D>();
        Physics.IgnoreLayerCollision(8, 8);
        sensors = GetComponentsInChildren<Sensor>();
        // Sort Checkpoints
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        Array.Sort(checkpoints, CompareObNames);
    }
    // function to compare and sort the checkpoints by name
    int CompareObNames (GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

    private void FixedUpdate()
    {
        if (init == true)
        {
            // get sensor values
            sensorVals = new float[sensors.Length];
            for (int i = 0; i < sensors.Length; i++)
            {
                sensorVals[i] = sensors[i].GetDistance();
            }
            // get neural network outputs
            float[] outputs = net.FeedForward(sensorVals);
            sensorOutputs = outputs;
            // calculate velocity based on neural network outputs
            velocity += (float)System.Math.Abs(outputs[0]) * acc * Time.deltaTime;
            // cap velocity at maxSpeed
            if (velocity > maxSpeed)
            {
                velocity = maxSpeed;
            }
            // calculate rotation based on neural network outputs
            rotation = transform.rotation;
            rotation *= Quaternion.AngleAxis((float)-outputs[1] * turnSpeed * Time.deltaTime, new Vector3(0, 0, 1));

            Vector3 direction = new Vector3(0, 1, 0);
            transform.rotation = rotation;
            direction = rotation * direction;

            this.transform.position += direction * velocity * Time.deltaTime;
            // Check Distance to next Checkpoint, if car dies because no checkpoint has been collected in the last x seconds and update fitness score
            CheckDistance();
            CheckTime();
            showFitnessLevel = net.GetFitness();
        }
    }
    // On Collision with the wall object car stops all movement
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            Die();
        }
    }
    // Check the distance between the car and the next checkpoint
    public void CheckDistance()
    {
        
        // Get distance between last and next checkpoint
        float distanceCheckpoints = Vector2.Distance(checkpoints[currentCheckpoint].transform.position, checkpoints[currentCheckpoint - 1].transform.position);
        // Get distance between next checkpoint and car
        float distanceCar = Vector2.Distance(transform.position, checkpoints[currentCheckpoint].transform.position);
        // Get way completion percentage
        float percentageWay = distanceCar / distanceCheckpoints;

        if (percentageWay < 0)
        {
            percentageWay = 0f;
        }

        // Set fitness
        net.SetFitness(checkpoints[currentCheckpoint].GetComponent<Checkpoint>().fitnessValue + (1 / percentageWay));

        float distance = Vector2.Distance(transform.position, checkpoints[currentCheckpoint].transform.position);
        showDistance = distance;
        if (distance < checkpoints[currentCheckpoint].GetComponent<Checkpoint>().radius)
        {
            timeSinceCheckpoint = 0f;
            if (currentCheckpoint < checkpoints.Length - 1)
            {
                currentCheckpoint++;
            }
            else
            {
                Die();
            }
        }
        else
        {
            timeSinceCheckpoint += Time.deltaTime;
        }
    }
    // Checks if the car hasn't collected a checkpoint in the last maxTime seconds
    private void CheckTime()
    {
        if (timeSinceCheckpoint >= maxTime) {
            Die();
        }
    }
    // Initiate car, adds neural network to car object
    public void Init(NeuralNetwork net)
    {
        this.net = net;
        init = true;
    }
    // Sets all movement to null
    public void Die()
    {
        velocity = 0;
        rotation = Quaternion.AngleAxis(0, new Vector3(0, 0, 1));
        for (int i = 0; i < sensors.Length; i++)
        {
            sensors[i].gameObject.SetActive(false);
        }
        this.GetComponent<CarMovement>().enabled = false;
        isAlive = false;
    }
    // return NeuralNetwork Object of chosen car
    public NeuralNetwork GetNeuralNetwork()
    {
        return net;
    }
    // return sensor values
    public float[] GetOutputValues()
    {
        return sensorOutputs;
    }
}
