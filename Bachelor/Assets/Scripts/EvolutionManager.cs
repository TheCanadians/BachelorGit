using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour {

    public GameObject carPrefab;

    public int populationSize = 20;
    public int[] layers = new int[] {3, 5, 5, 2}; // Topology

    private bool isTraining = false;
    private int generationNumber = 0;
    private List<NeuralNetwork> nets;
    private List<CarMovement> carList = null;

    void StartTraining()
    {
        isTraining = false;
    }

    void Update()
    {
        if (isTraining == false)
        {
            if (generationNumber == 0)
            {
                InitNeuralNetworks();
            }
            else
            {
                nets.Sort();
                for (int i = 0; i < populationSize / 2; i++)
                {
                    nets[i] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                    nets[i].Mutate();

                    nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }

            generationNumber++;

            isTraining = true;
            Invoke("StartTraining", 15f);
            CreateCars();
        }
    }
    
    private void CreateCars()
    {
        if (carList != null)
        {
            for (int i = 0; i < carList.Count; i++)
            {
                GameObject.Destroy(carList[i].gameObject);
            }
        }

        carList = new List<CarMovement>();

        for (int i = 0; i < populationSize; i++)
        {
            CarMovement car = ((GameObject)Instantiate(carPrefab, carPrefab.transform.position, carPrefab.transform.rotation)).GetComponent<CarMovement>();
            car.Init(nets[i]);
            carList.Add(car);
        }
    }
    

    void InitNeuralNetworks()
    {
        if (populationSize % 2 != 0)
        {
            populationSize = 20;
        }

        nets = new List<NeuralNetwork>();

        for (int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            net.Mutate();
            nets.Add(net);
        }
    }
}
