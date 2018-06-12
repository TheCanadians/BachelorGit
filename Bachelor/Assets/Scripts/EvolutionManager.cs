using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour {

    public GameObject carPrefab;

    public float timer = 20f;
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
                if (carList != null)
                {
                    for (int i = 0; i < populationSize; i++)
                    {
                        carList[i].CheckDistance(true);
                    }
                }
                nets.Sort();
                NeuralNetwork best = new NeuralNetwork(nets[nets.Count - 1]);
                NeuralNetwork secBest = new NeuralNetwork(nets[nets.Count - 2]);
                nets[nets.Count - 1] = new NeuralNetwork(best);
                nets[nets.Count - 2] = new NeuralNetwork(secBest);
                for (int i = 0; i < (populationSize/2)-1; i++)
                {
                    nets[i] = new NeuralNetwork(best);
                    nets[i + (populationSize / 2)-1] = new NeuralNetwork(secBest);
                    nets[i].Mutate();
                    nets[i + (populationSize / 2)-1].Mutate();

                    //nets[i + (populationSize / 2)] = new NeuralNetwork(nets[i + (populationSize / 2)]);
                }

                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }

            generationNumber++;

            isTraining = true;
            Invoke("StartTraining", timer);
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

    public int GetGenerationCount()
    {
        return generationNumber;
    }
}
