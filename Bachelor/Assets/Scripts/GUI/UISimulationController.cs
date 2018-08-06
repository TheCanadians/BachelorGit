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
            for (int i = 0; i < cars.Length; i++)
            {
                carsNetwork[i] = cars[i].GetComponent<CarMovement>().GetNeuralNetwork();
            }
            NeuralNetwork[] carsNetworkCopy = carsNetwork;

            Array.Sort(carsNetwork, CompareFitness);

            ChangeCarColor();

            NeuralNetPanel.Display(carsNetwork[carsNetwork.Length - 1]);
            //NeuralNetGraph.Position();

            SetTexts();
        }       
    }

    void SetTexts()
    {
        Fitness.text = carsNetwork[carsNetwork.Length - 1].GetFitness().ToString("#.0");
        GenerationCount.text = evoManager.GetGenerationCount().ToString();

        float[] carOutputValues = new float[2];

        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].GetComponent<CarMovement>().GetNeuralNetwork().GetFitness() == carsNetwork[carsNetwork.Length - 1].GetFitness())
            {
                carOutputValues = cars[i].GetComponent<CarMovement>().GetOutputValues();
            }
        }

        InputTexts[0].text = carOutputValues[0].ToString("0.0");
        InputTexts[1].text = carOutputValues[1].ToString("0.0");
        InputTexts[2].text = evoManager.averageFitness.ToString() + "%";
        InputTexts[3].text = evoManager.bestFitness.ToString();
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

    public void WipeOldNetwork()
    {
        // layer
        foreach(Transform layer in NeuralNetPanel.transform)
        {
            // content
            foreach(Transform content in layer)
            {
                // neurons
                foreach(Transform neuron in content)
                {
                    // weight
                    foreach (Transform weight in neuron)
                    {
                        if (layer.name == "Layer(Clone)")
                        {
                            Destroy(weight.gameObject);
                        }
                        else if (weight.name == "Weight(Clone)")
                        {
                            Destroy(weight.gameObject);
                        }
                    }
                    if (layer.name == "Layer(Clone)")
                    {
                        Destroy(neuron.gameObject);
                    }
                    else if (neuron.name == "Neuron(Clone)")
                    {
                        Destroy(neuron.gameObject);
                    }
                }
                if (layer.name == "Layer(Clone)")
                {
                    Destroy(content.gameObject);
                }
            }
            if (layer.name == "Layer(Clone)")
            {
                Destroy(layer.gameObject);
            }
        }
    }
}
