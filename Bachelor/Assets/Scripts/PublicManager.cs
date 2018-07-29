using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PublicManager : MonoBehaviour {

    // scripts to start up when start button is pressed
    public EvolutionManager evoMan;
    public UIController uiCon;
    public UISimulationController uiSimCon;
    // inactive Gameobjects and layer Prefab
    public InputField layer;
    public GameObject tournamentPanel;
    public GameObject stopAtGenerationPanel;
    // Settings for the cars, how fast they turn, accelerate, their max Speed and how much time without checkpoint needs to pass before they die
    [Header("Car Settings")]
    [SerializeField]
    public float turnSpeed = 0f;
    [SerializeField]
    public float acceleration = 0f;
    [SerializeField]
    public float maxSpeed = 0f;
    [SerializeField]
    public float timeToDeath = 0f;
    // Adds noise to the sensors
    [Header("Sensor Settings")]
    [Range(0.0f, 5.0f)]
    public float maxNoise = 0f;
    [Range(-5.0f, 0.0f)]
    public float minNoise = 0f;
    // Settings for the neural network, like activation function and number of layers and neurons per layer
    [Header("Neural Network Settings")]
    public ActivationFunction activationFnc;
    [SerializeField]
    public int[] layers;
    // Mutation Settings for the probability that mutation occurs and for which amount it occurs
    [Header("Mutation Settings")]
    [SerializeField]
    public float mutationProbability = 0f;
    [SerializeField]
    public float mutationAmount = 0f;
    // Settings for the genetic algorithm, regulates the population size, selection method, if the progress per generation should be logged in a text file and when to stop logging
    [Header("Genetic Algorithm Settings")]
    [SerializeField]
    public int population = 0;
    [SerializeField]
    public SelectionType selectionType;
    [SerializeField]
    public string selectionTypeName;
    [SerializeField]
    public int numberOfTournamentWinners = 4;
    [SerializeField]
    public bool logProgress = false;
    [SerializeField]
    public int stopGenerationNumber = 50;
    // enum to choose the Activation Functions from. Isn't used in the GUI
    [SerializeField]
    public enum ActivationFunction
    {
        Sigmoid,TanH,SoftSign
    }
    // enum to choose the Selection Method. Isn't used in the GUI.
    [SerializeField]
    public enum SelectionType
    {
        Elitist,RouletteWheel,Tournament,Rank,Random
    }
    
    // Method that starts the simulation when the GUI Button "Start Simulation" is pressed. Gets all relevant GUI values and sets them.
    public void StartSimulation()
    {
        Slider minNoiseSlider = GameObject.Find("MinNoiseSlider").GetComponent<Slider>();
        minNoise = minNoiseSlider.value;
        Slider maxNoiseSlider = GameObject.Find("MaxNoiseSlider").GetComponent<Slider>();
        maxNoise = maxNoiseSlider.value;
        InputField numberOfLayers = GameObject.Find("NumberOfLayersInput").GetComponent<InputField>();
        GameObject layersPanel = GameObject.Find("LayersPanel");
        layers = new int[int.Parse(numberOfLayers.text)];
        int index = -1;
        foreach(Transform child in layersPanel.transform)
        {
            if (child.GetSiblingIndex() != 0)
            {
                InputField childInput = child.GetComponent<InputField>();
                int numberOfNeurons = int.Parse(childInput.text);
                layers[index] = numberOfNeurons;
            }
            index++;
        }
        InputField populationSize = GameObject.Find("PopulationInput").GetComponent<InputField>();
        population = int.Parse(populationSize.text);
        Dropdown selectionTypeDropdown = GameObject.Find("SelectionTypeDropdown").GetComponent<Dropdown>();
        int dropdownValue = selectionTypeDropdown.value;
        selectionTypeName = selectionTypeDropdown.options[dropdownValue].text;
        InputField mutationProb = GameObject.Find("MutationProbabilityInput").GetComponent<InputField>();
        mutationProbability = float.Parse(mutationProb.text);
        InputField mutationAmountInput = GameObject.Find("MutationAmountInput").GetComponent<InputField>();
        mutationAmount = float.Parse(mutationAmountInput.text);
        Toggle compareToggle = GameObject.Find("LogProgressToggle").GetComponent<Toggle>();
        logProgress = compareToggle.isOn;
        if (selectionTypeDropdown.value == 2)
        {
            InputField tournamentWinners = GameObject.Find("TournamentWinnersInput").GetComponent<InputField>();
            numberOfTournamentWinners = int.Parse(tournamentWinners.text);
        }
        if (compareToggle.isOn)
        {
            InputField stopAtGeneration = GameObject.Find("StopAtGenerationInput").GetComponent<InputField>();
            stopGenerationNumber = int.Parse(stopAtGeneration.text);
        }
        evoMan.selectionType = selectionTypeName;
        // Starts the simulation by toggling 3 bools in 3 different scripts
        evoMan.start = true;
        uiCon.start = true;
        uiSimCon.start = true;
        // Deactivates GUI after start
        GameObject.Find("SimulationPanel").SetActive(false);
    }
    // Refreshes min Slider text value
    public void ChangeMinSliderText(Slider minSlider)
    {
        GameObject.Find("MinSliderText").GetComponent<Text>().text = minSlider.value.ToString();
    }
    // Refreshes max Slider text value
    public void ChangeMaxSliderText(Slider maxSlider)
    {
        GameObject.Find("MaxSliderText").GetComponent<Text>().text = maxSlider.value.ToString();
    }
    // Changes the number of input fields below. Can't go higher than 8 and has to be at least 2 (for input and output layer)
    public void ChangeNeuralLayerCount(InputField layers)
    {
        int numberOfLayers = int.Parse(layers.text);
        
        if (numberOfLayers < 2)
        {
            layers.text = "2";
            numberOfLayers = 2;
        }
        if (numberOfLayers > 8)
        {
            layers.text = "8";
            numberOfLayers = 8;
        }

        GameObject layersObject = GameObject.Find("LayersPanel");

        // Destroy old clones first
        foreach (Transform child in layersObject.transform)
        {
            if (child.name == "InputLayer(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
            
        }
        // Instantiate new input fields and set them in the hierarchy between input and output layer
        for (int i = 2; i < numberOfLayers; i++)
        {
            InputField newLayer = Instantiate(layer, layer.transform.position, Quaternion.identity);
            newLayer.transform.SetParent(layersObject.transform, false);
            newLayer.transform.SetSiblingIndex(2);
        }
    }
    // Toggles the Tournament Selection GUI to be visible
    public void OnTournamentSelectionSelected(Dropdown dropdown)
    {
        int selectionType = dropdown.value;
        if (selectionType == 2)
        {
            tournamentPanel.SetActive(true);
        }
        else
        {
            tournamentPanel.SetActive(false);
        }
        
    }
    // Toggles the stop at generation GUI
    public void OnSelectionToggled(Toggle toggle)
    {
        if(toggle.isOn)
        {
            stopAtGenerationPanel.SetActive(true);
        }
        else
        {
            stopAtGenerationPanel.SetActive(false);
        }
    }
}
