using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour {

    public GameObject carPrefab;

    public float timer = 20f;
    public int populationSize = 20;
    public int[] layers = new int[] {3, 5, 5, 2}; // Topology

    private bool isTraining = false;
    private bool isSpawning = true;
    private int generationNumber = 0;
    private List<NeuralNetwork> nets;
    private List<CarMovement> carList = null;

    private static System.Random randomizer = new System.Random();
    private float crossoverProbability = 0.5f;

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
                if (carList != null)
                {
                    for (int i = 0; i < populationSize; i++)
                    {
                        carList[i].CheckDistance();
                    }
                }
                nets.Sort();
                NeuralNetwork best = new NeuralNetwork(nets[nets.Count - 1]);
                NeuralNetwork secBest = new NeuralNetwork(nets[nets.Count - 2]);
                nets[nets.Count - 1] = new NeuralNetwork(best);
                nets[nets.Count - 2] = new NeuralNetwork(secBest);
                for (int i = 0; i < populationSize-2; i++)
                {
                    NeuralNetwork newNet = Crossover(best, secBest);
                    nets[i] = new NeuralNetwork(newNet);
                    nets[i].Mutate();
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }

            generationNumber++;

            isTraining = true;
            isSpawning = true;
            Invoke("StartTraining", timer);
            CreateCars();
        }
        if (isSpawning)
        {
            CheckAlive();
        }
    }

    private NeuralNetwork Crossover(NeuralNetwork a, NeuralNetwork b)
    {
        float[][][] aWeights = a.GetWeightsMatrix();
        float[][][] bWeights = b.GetWeightsMatrix();
        NeuralNetwork newNeuralNetwork = new NeuralNetwork(a);
        float[][][] newWeights = aWeights;

        for (int i = 0; i < aWeights.Length; i++)
        {
            for (int j = 0; j < aWeights[i].Length; j++)
            {
                for (int k = 0; k < aWeights[i][j].Length; k++)
                {
                    if (randomizer.Next() < crossoverProbability)
                    {
                        newWeights[i][j][k] = aWeights[i][j][k];
                    }
                    else
                    {
                        newWeights[i][j][k] = bWeights[i][j][k];
                    }
                }
            }
        }
        newNeuralNetwork.SetWeightsMatrix(newWeights);

        return newNeuralNetwork;
    }

    private void CheckAlive()
    {
        int deathCounter = 0;
        for (int i = 0; i < carList.Count; i++)
        {
            if (carList[i].isAlive == false)
            {
                deathCounter++;
            }
            if (deathCounter == carList.Count)
            {
                isSpawning = false;
                Invoke("StartTraining", 1f);
            }
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
            car.name = i.ToString();
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

    public int GetGenerationCount()
    {
        return generationNumber;
    }
}
