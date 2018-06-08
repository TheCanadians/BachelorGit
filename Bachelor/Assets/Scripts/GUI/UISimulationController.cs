using UnityEngine;
using UnityEngine.UI;
using System;

public class UISimulationController : MonoBehaviour {

    [SerializeField]
    private Text[] InputTexts;
    [SerializeField]
    private Text Fitness;
    [SerializeField]
    private Text GenerationCount;
    [SerializeField]
    private UINeuralNetworkPanel NeuralNetPanel;

    private NeuralNetwork car;
    private EvolutionManager evoManager;

    private void Start()
    {
        car = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<CarMovement>().GetNeuralNetwork();
        evoManager = GameObject.Find("EvolutionManager").GetComponent<EvolutionManager>();
    }

    private void Update()
    {
        car = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<CarMovement>().GetNeuralNetwork();
        evoManager = GameObject.Find("EvolutionManager").GetComponent<EvolutionManager>();
        NeuralNetPanel.Display(car);

        Fitness.text = car.GetFitness().ToString();
        GenerationCount.text = evoManager.GetGenerationCount().ToString();
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
