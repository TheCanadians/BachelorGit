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

    private float velocity;
    private Quaternion rotation;

    private bool init = false;

    private NeuralNetwork net;
    private Rigidbody2D rBody;
    private float[] sensorVals;
    private Sensor[] sensors;

    private int currentCheckpoint = 1;
    private Transform lastCheckpoint = null;
    private Transform[] checkpoints;

    public float showFitnessLevel;
    public float showDistance;
    private float fitness = 0f;

    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        Physics.IgnoreLayerCollision(8, 8);
        sensors = GetComponentsInChildren<Sensor>();

        GameObject parent = GameObject.Find("Checkpoints");
        checkpoints = parent.GetComponentsInChildren<Transform>();
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
            float[] output = net.FeedForward(sensorVals);

            velocity += (float)System.Math.Abs(output[0]) * acc * Time.deltaTime;

            if (velocity > maxSpeed)
            {
                velocity = maxSpeed;
            }

            rotation = transform.rotation;
            rotation *= Quaternion.AngleAxis((float)-output[1] * turnSpeed * Time.deltaTime, new Vector3(0, 0, 1));

            Vector3 direction = new Vector3(0, 1, 0);
            transform.rotation = rotation;
            direction = rotation * direction;

            this.transform.position += direction * velocity * Time.deltaTime;

            CheckDistance(false);
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

    public void CheckDistance(bool end)
    {
        
        float distance = Vector2.Distance(transform.position, checkpoints[currentCheckpoint].transform.position);
        showDistance = distance;
        if (distance < 50f)
        {
            currentCheckpoint++;
            net.AddFitness(1f);
        }
        if (end)
        {
            fitness = 1f / distance;
            net.AddFitness(fitness);
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
    }
}
