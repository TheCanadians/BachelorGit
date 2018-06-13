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
    private Sprite normalCar;

    private GameObject[] cars;
    private NeuralNetwork[] carsNetwork;
    private EvolutionManager evoManager;

    private void Start()
    {
        cars = GameObject.FindGameObjectsWithTag("Player");
        carsNetwork = new NeuralNetwork[cars.Length];
        evoManager = GameObject.Find("EvolutionManager").GetComponent<EvolutionManager>();
    }

    private void Update()
    {
        GetReferences();
        for (int i = 0; i < cars.Length; i++)
        {
            carsNetwork[i] = cars[i].GetComponent<CarMovement>().GetNeuralNetwork();
        }
        NeuralNetwork[] carsNetworkCopy = carsNetwork;

        Array.Sort(carsNetwork, CompareFitness);

        ChangeCarColor();

        NeuralNetPanel.Display(carsNetwork[carsNetwork.Length - 1]);

        SetTexts();       
    }

    void SetTexts()
    {
        Fitness.text = carsNetwork[carsNetwork.Length - 1].GetFitness().ToString();
        GenerationCount.text = evoManager.GetGenerationCount().ToString();

        CarMovement carObject = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<CarMovement>();
        float[] carOutputValues = carObject.GetOutputValues();

        InputTexts[0].text = carOutputValues[0].ToString();
        InputTexts[1].text = carOutputValues[1].ToString();
    }

    void GetReferences()
    {
        cars = GameObject.FindGameObjectsWithTag("Player");
        carsNetwork = new NeuralNetwork[cars.Length];
        evoManager = GameObject.Find("EvolutionManager").GetComponent<EvolutionManager>();
    }

    void ChangeCarColor()
    {
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].GetComponent<CarMovement>().GetNeuralNetwork().GetFitness() == carsNetwork[carsNetwork.Length - 1].GetFitness())
            {
                cars[i].GetComponent<SpriteRenderer>().sprite = firstCar;
                camera.GetComponent<CameraMovement>().SetTarget(cars[i]);
            }
            else
            {
                cars[i].GetComponent<SpriteRenderer>().sprite = normalCar;
            }
        }
    }

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
