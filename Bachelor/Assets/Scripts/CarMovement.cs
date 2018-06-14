using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {

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

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        Physics.IgnoreLayerCollision(8, 8);
        sensors = GetComponentsInChildren<Sensor>();

        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
        Array.Sort(checkpoints, CompareObNames);
        for (int i = 0; i < checkpoints.Length; i++)
        {
            Debug.Log(checkpoints[i].name);
        }
    }

    int CompareObNames (GameObject x, GameObject y)
    {
        return x.name.CompareTo(y.name);
    }

    private void FixedUpdate()
    {
        if (init == true)
        {
            sensorVals = new float[sensors.Length];
            for (int i = 0; i < sensors.Length; i++)
            {
                sensorVals[i] = sensors[i].GetDistance();
            }
            float[] outputs = net.FeedForward(sensorVals);
            sensorOutputs = outputs;

            velocity += (float)System.Math.Abs(outputs[0]) * acc * Time.deltaTime;

            if (velocity > maxSpeed)
            {
                velocity = maxSpeed;
            }

            rotation = transform.rotation;
            rotation *= Quaternion.AngleAxis((float)-outputs[1] * turnSpeed * Time.deltaTime, new Vector3(0, 0, 1));

            Vector3 direction = new Vector3(0, 1, 0);
            transform.rotation = rotation;
            direction = rotation * direction;

            this.transform.position += direction * velocity * Time.deltaTime;

            CheckDistance();
            CheckTime();
            showFitnessLevel = net.GetFitness();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            Die();
        }
    }

    public void CheckDistance()
    {
        
        // Get distance between last and next checkpoint
        float distanceCheckpoints = Vector2.Distance(checkpoints[currentCheckpoint].transform.position, checkpoints[currentCheckpoint - 1].transform.position);
        // Get distance between next checkpoint and car
        float distanceCar = Vector2.Distance(transform.position, checkpoints[currentCheckpoint].transform.position);
        // Get way completion percentage
        float percentageWay = distanceCar / distanceCheckpoints;

        // Set fitness
        net.SetFitness(checkpoints[currentCheckpoint].GetComponent<Checkpoint>().fitnessValue - 1 + (1 / percentageWay));

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
                net.SetFitness(checkpoints[currentCheckpoint].GetComponent<Checkpoint>().fitnessValue);
            }
        }
        else
        {
            timeSinceCheckpoint += Time.deltaTime;
        }
    }

    private void CheckTime()
    {
        if (timeSinceCheckpoint >= maxTime) {
            Die();
        }
    }

    public void Init(NeuralNetwork net)
    {
        this.net = net;
        init = true;
    }

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

    public NeuralNetwork GetNeuralNetwork()
    {
        return net;
    }

    public float[] GetOutputValues()
    {
        return sensorOutputs;
    }
}
