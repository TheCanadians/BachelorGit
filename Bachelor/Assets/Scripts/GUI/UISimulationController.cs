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

    private bool activate = false;

    private void Start()
    {
        car = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<CarMovement>().GetNeuralNetwork();
        evoManager = GameObject.Find("EvolutionManager").GetComponent<EvolutionManager>();
    }

    private void Update()
    {
        car = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<CarMovement>().GetNeuralNetwork();
        evoManager = GameObject.Find("EvolutionManager").GetComponent<EvolutionManager>();
        if(!activate)
        {
            NeuralNetPanel.Display(car);
            activate = true;
        }
            

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
