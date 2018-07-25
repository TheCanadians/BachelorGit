﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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
    private float crossoverProbability = 0.1f;
    private string crossoverType = "RouletteWheel";
    private int tournamentWinners;
    private bool compareSelection;
    private int stopNumber;

    public int averageFitness = 0;
    public float bestFitness = 0f;

    private List<string> compareList = new List<string>();
    private string textPath = "Assets/compare.txt";
    private StreamWriter writer;

    UINeuralNetworkGraph neuralGraph;
    PublicManager publicManager;

    private void Start()
    {
        //neuralGraph = GameObject.Find("NeuralNetworkDiagramm").GetComponent<UINeuralNetworkGraph>();
        publicManager = GameObject.Find("PublicManager").GetComponent<PublicManager>();
        this.populationSize = publicManager.population;
        this.layers = publicManager.layers;
        this.crossoverType = publicManager.crossType.ToString();
        this.tournamentWinners = publicManager.numberOfTournamentWinners;
        this.compareSelection = publicManager.compareSelection;
        this.stopNumber = publicManager.stopGenerationNumber;
        writer = new StreamWriter(textPath, true);
    }

    void StartTraining()
    {
        isTraining = false;
    }

    void Update()
    {
        /*
         * 1. Create the first generation                        -> Initialize
         * 2.1. Calculate fitness                                -> Evaluation
         * 2.2. Reproduction / Selection                         
         * 2.2.1. Pick 2 parents                                 -> Selection
         * 2.2.2. Make a new child                               -> Reproduction
         * 2.2.2.1. Crossover parents DNA to make new child
         * 2.2.2.2. Mutate new child
         * 2.2.2.3. Back to 1.
         */



        if (isTraining == false)
        {
            InitNeuralNetworks();
            Eval();
            Select();

            generationNumber++;

            isTraining = true;
            isSpawning = true;
            //Invoke("StartTraining", timer);
            CreateCars();
        }
        if (isSpawning)
        {
            CheckAlive();
        }
    }

    private void InitNeuralNetworks()
    {
        if (generationNumber == 0)
        {
            if (populationSize % 2 != 0)
            {
                populationSize = 20;
            }

            nets = new List<NeuralNetwork>();

            for (int i = 0; i < populationSize; i++)
            {
                NeuralNetwork net = new NeuralNetwork(layers);
                net.mutationAmount = publicManager.mutationAmount;
                net.mutationProbability = publicManager.mutationProbability;
                net.Mutate();
                nets.Add(net);
            }
        }
    }

    private void Eval()
    {
        if (generationNumber != 0)
        {
            if (carList != null)
            {
                for (int i = 0; i < populationSize; i++)
                {
                    carList[i].CheckDistance();
                }
            }
            nets.Sort();
        }

    }

    private void Select()
    {
        List<NeuralNetwork> newNets = new List<NeuralNetwork>();

        if (crossoverType == "Elitist")
        {
            NeuralNetwork parentA = new NeuralNetwork(nets[nets.Count - 1]);
            NeuralNetwork parentB = new NeuralNetwork(nets[nets.Count - 2]);

            for (int i = 0; i < populationSize; i++)
            {
                NeuralNetwork newNet = Crossover(parentA, parentB);

                newNet.Mutate();
                newNets.Add(new NeuralNetwork(newNet));
            }
        }
        else if (crossoverType == "RouletteWheel")
        {
            RouletteWheelSelection(nets);

            for (int i = 0; i < populationSize; i++)
            {
                NeuralNetwork parentA = PickParent(nets);
                NeuralNetwork parentB = PickParent(nets);

                NeuralNetwork newNet = Crossover(parentA, parentB);

                newNet.Mutate();
                newNets.Add(new NeuralNetwork(newNet));
            }
        }
        else if (crossoverType == "Rank")
        {
            RankSelection(nets);

            for (int i = 0; i < populationSize; i++)
            {
                NeuralNetwork parentA = PickParent(nets);
                NeuralNetwork parentB = PickParent(nets);

                NeuralNetwork newNet = Crossover(parentA, parentB);

                newNet.Mutate();
                newNets.Add(new NeuralNetwork(newNet));
            }
        }
        else if (crossoverType == "Tournament")
        {
            for (int i = 0; i < populationSize; i++)
            {
                NeuralNetwork parentA;
                NeuralNetwork parentB;

                List<int> numbersA = new List<int>(nets.Count);
                List<int> numbersB = new List<int>(nets.Count);

                for (int number = 0; number < populationSize; number++)
                {
                    numbersA.Add(number);
                    numbersB.Add(number);
                }

                List<int> winnersA = new List<int>(tournamentWinners);
                List<int> winnersB = new List<int>(tournamentWinners);

                for (int j = 0; j < tournamentWinners; j++)
                {
                    int randomNumA = Random.Range(0, numbersA.Count);
                    
                    winnersA.Add(numbersA[randomNumA]);
                    numbersA.RemoveAt(randomNumA);
                    int randomNumB = Random.Range(0, numbersB.Count);
                    winnersB.Add(numbersB[randomNumB]);
                    numbersB.RemoveAt(randomNumB);
                }

                winnersA.Sort();
                winnersB.Sort();

                parentA = nets[winnersA[winnersA.Count - 1]];
                parentB = nets[winnersB[winnersB.Count - 1]];

                NeuralNetwork newNet = Crossover(parentA, parentB);

                newNet.Mutate();
                newNets.Add(new NeuralNetwork(newNet));
            }
        }
        else if (crossoverType == "Random")
        {
            for (int i = 0; i < populationSize; i++)
            {
                int a = Random.Range(0, nets.Count - 1);
                int b = Random.Range(0, nets.Count - 1);

                NeuralNetwork parentA = new NeuralNetwork(nets[a]);
                NeuralNetwork parentB = new NeuralNetwork(nets[b]);

                NeuralNetwork newNet = Crossover(parentA, parentB);

                newNet.Mutate();
                newNets.Add(new NeuralNetwork(newNet));
            }
         }

        nets = newNets;

        for (int i = 0; i < populationSize; i++)
        {
            nets[i].SetFitness(0f);
        }
    }

    private void RouletteWheelSelection(List<NeuralNetwork> nets)
    {
        // sum up fitness
        float sum = 0f;
        for (int i = 0; i < nets.Count; i++)
        {
            sum += Mathf.Pow(nets[i].GetFitness(), 4);
        }
        // normalize fitness to value between 0 and 1
        for (int j = 0; j < nets.Count; j++)
        {
            nets[j].score = Mathf.Pow(nets[j].GetFitness(), 4) / sum;
        }
    }

    private void RankSelection(List<NeuralNetwork> nets)
    {
        float sum = 0f;
        for (int i = 0; i < nets.Count; i++)
        {
            sum += i;
        }
        // normalize fitness to value between 0 and 1
        for (int j = 0; j < nets.Count; j++)
        {
            nets[j].score = j / sum;
        }
    }

    private NeuralNetwork PickParent(List<NeuralNetwork> nets)
    {
        int index = 0;
        float r = Random.Range(0f, 1f);
        float firstRandom = r;
        while (r > 0)
        {
            r = r - nets[index].score;
            index++;
        }
        index--;
        return nets[index];
    }

    private NeuralNetwork Crossover(NeuralNetwork a, NeuralNetwork b)
    {
        float[][][] aWeights = a.GetWeightsMatrix();
        float[][][] bWeights = b.GetWeightsMatrix();
        NeuralNetwork newNeuralNetwork = new NeuralNetwork(a);
        float[][][] newWeights = aWeights;

        int splitPointA = Random.Range(0, aWeights.Length);
        int splitPointB = Random.Range(0, aWeights[splitPointA].Length);
        int splitPointC = Random.Range(0, aWeights[splitPointA][splitPointB].Length);

        for (int i = 0; i < aWeights.Length; i++)
        {
            for (int j = 0; j < aWeights[i].Length; j++)
            {
                for (int k = 0; k < aWeights[i][j].Length; k++)
                {
                    if (i >= splitPointA && j >= splitPointB && k >= splitPointC)
                    {
                        newWeights[i][j][k] = bWeights[i][j][k];
                    }
                    else
                    {
                        newWeights[i][j][k] = aWeights[i][j][k];
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

                averageFitness = (int)GetAverageFitness();
                bestFitness = GetBestCar();

                if (compareSelection)
                {
                    string compareString = generationNumber + ":   Best Car: " + bestFitness + "   Average Fitness: " + averageFitness + "   Selection Type: " + crossoverType;
                    compareList.Add(compareString);

                    if (generationNumber <= stopNumber)
                    {
                        writer.WriteLine(compareString);
                    }
                    else
                    {
                        writer.Close();
                    }
                }

               

                Invoke("StartTraining", 1f);
            }
        }
    }

    private float GetAverageFitness()
    {
        float averageFitness = 0;
        for (int i = 0; i < carList.Count; i++)
        {
            NeuralNetwork carNetwork = carList[i].GetNeuralNetwork();
            averageFitness += carNetwork.GetFitness();
        }
        int numberOfCheckpoints = GameObject.FindGameObjectsWithTag("Checkpoint").Length;
        return (float)((averageFitness / carList.Count) / numberOfCheckpoints) * 100;
    }

    private float GetBestCar()
    {
        float bestCar = 0f;
        NeuralNetwork[] carsNetworks = new NeuralNetwork[carList.Count];
        for (int i = 0; i < carList.Count; i++)
        {
            carsNetworks[i] = carList[i].GetNeuralNetwork();
        }
        System.Array.Sort(carsNetworks, CompareFitness);
        bestCar = carsNetworks[carsNetworks.Length - 1].GetFitness();
        if (bestCar > bestFitness)
        {
            return bestCar;
        }
        else
        {
            return bestFitness;
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

    int CompareFitness(NeuralNetwork x, NeuralNetwork y)
    {
        return x.GetFitness().CompareTo(y.GetFitness());
    }

    public int GetGenerationCount()
    {
        return generationNumber;
    }
}
