using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UISimulationController : MonoBehaviour {

    [SerializeField]
    private Camera camera;
    [SerializeField]
    private Text[] InputTexts;
    [SerializeField]
    private Text Fitness;
    [SerializeField]
    private Text GenerationCount;
    [SerializeField]
    private UINeuralNetworkPanel NeuralNetPanel;
    [SerializeField]
    private Sprite firstCar;
    [SerializeField]
    private Sprite secondCar;
    [SerializeField]
    private Sprite normalCar;

    private GameObject[] cars;
    private NeuralNetwork[] carsNetwork;
    private EvolutionManager evoManager;

    public bool start = false;

    private void Start()
    {
        cars = GameObject.FindGameObjectsWithTag("Player");
        carsNetwork = new NeuralNetwork[cars.Length];
        evoManager = GameObject.Find("EvolutionManager").GetComponent<EvolutionManager>();
    }

    private void Update()
    {
        if (start)
        {
            GetReferences();
            // Get Neural Network of each car
            for (int i = 0; i < cars.Length; i++)
            {
                carsNetwork[i] = cars[i].GetComponent<CarMovement>().GetNeuralNetwork();
            }
            NeuralNetwork[] carsNetworkCopy = carsNetwork;
            // Sort Array based on fitness
            Array.Sort(carsNetwork, CompareFitness);
            // Change the color of the best and second best car (Fitness)
            ChangeCarColor();
            // Display neural network
            NeuralNetPanel.Display(carsNetwork[carsNetwork.Length - 1]);
            // Set text information (generation number, network outputs, best & average fitness)
            SetTexts();
        }       
    }
    // Set neural network and genetic algorithm text information in GUI
    void SetTexts()
    {
        // Set "Fit" text
        Fitness.text = carsNetwork[carsNetwork.Length - 1].GetFitness().ToString("#.0");
        // Set "Generation" text
        GenerationCount.text = evoManager.GetGenerationCount().ToString();

        float[] carOutputValues = new float[2];

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].GetComponent<CarMovement>().GetNeuralNetwork().GetFitness() == carsNetwork[carsNetwork.Length - 1].GetFitness())
            {
                carOutputValues = cars[i].GetComponent<CarMovement>().GetOutputValues();
            }
        }
        // Set "Turn" text
        InputTexts[0].text = carOutputValues[0].ToString("0.0");
        // Set "Power" text
        InputTexts[1].text = carOutputValues[1].ToString("0.0");
        // Set "Average Fitness" text
        InputTexts[2].text = evoManager.averageFitness.ToString() + "%";
        // Set "Best Fitness" text
        InputTexts[3].text = evoManager.bestFitness.ToString();
    }
    // Get Cars Array, neural Networks of said cars and EvolutionManager.cs
    void GetReferences()
    {
        cars = GameObject.FindGameObjectsWithTag("Player");
        carsNetwork = new NeuralNetwork[cars.Length];
        evoManager = GameObject.Find("EvolutionManager").GetComponent<EvolutionManager>();
    }
    // Change cor color based on position: leading car = red, second leading car = yellow, other = blue
    void ChangeCarColor()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].GetComponent<CarMovement>().GetNeuralNetwork().GetFitness() == carsNetwork[carsNetwork.Length - 1].GetFitness())
            {
                cars[i].GetComponent<SpriteRenderer>().sprite = firstCar;
                camera.GetComponent<CameraMovement>().SetTarget(cars[i]);
            }
            else if (cars[i].GetComponent<CarMovement>().GetNeuralNetwork().GetFitness() == carsNetwork[carsNetwork.Length - 2].GetFitness())
            {
                cars[i].GetComponent<SpriteRenderer>().sprite = secondCar;
            }
            else
            {
                cars[i].GetComponent<SpriteRenderer>().sprite = normalCar;
            }
        }
    }
    // Compare Fitness of two neural networks
    int CompareFitness (NeuralNetwork x, NeuralNetwork y)
    {
        return x.GetFitness().CompareTo(y.GetFitness());
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
