using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour {

    [SerializeField]
    private static float maxSpeed = 5f;

    private bool init = false;

    private NeuralNetwork net;
    private Rigidbody2D rBody;
    private float[] sensorVals;
    private Sensor[] sensors;


    private void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
        Physics.IgnoreLayerCollision(8, 8);
        sensors = GetComponentsInChildren<Sensor>();
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
            rBody.velocity = maxSpeed * transform.up.normalized * output[0];
            rBody.angularVelocity = 100f * output[1];
        }
    }

    public void Init(NeuralNetwork net)
    {
        this.net = net;
        init = true;
    }
}
