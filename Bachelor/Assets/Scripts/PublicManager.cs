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
    public GameObject simulationPanel;
    public GameObject neuralNetworkPanel;
    public GameObject evolutionPanel;
    public GameObject loadNeuralNetPanel;
    public GameObject saveNeuralNetPanel;
    public GameObject saveBestCarToggle;
    public InputField path;
    public InputField layer;
    public GameObject tournamentPanel;
    public GameObject stopAtGenerationPanel;
    public GameObject saveLogPanel;
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
    [Range(0.0f, 1.0f)]
    public float stdDev = 0f;
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
    public TextAsset file;
    
    // Method that starts the simulation when the GUI Button "Start Simulation" is pressed. Gets all relevant GUI values and sets them.
    public void StartSimulation()
    {
        // if Test is choosen, set loadPath and toggle test bool in EvolutionManager.cs
        if (GameObject.Find("TestToggle").GetComponent<Toggle>().isOn)
        {
            string loadPath = this.path.text;
            evoMan.readPath = loadPath;
            evoMan.test = true;
        }
        // if Training is choosen, set topology, population size, selectionType, mutation probability and amount and toggles training bool in EvolutionManager.cs
        else
        {
            // Save Path
            InputField savePathInput = GameObject.Find("SaveNeuralNetInput").GetComponent<InputField>();
            evoMan.savePath = savePathInput.text;
            // Topology
            InputField numberOfLayers = GameObject.Find("NumberOfLayersInput").GetComponent<InputField>();
            GameObject layersPanel = GameObject.Find("LayersPanel");
            layers = new int[int.Parse(numberOfLayers.text) + 2];
            int index = -1;
            foreach (Transform child in layersPanel.transform)
            {
                if (child.GetSiblingIndex() != 0)
                {
                    InputField childInput = child.GetComponent<InputField>();
                    int numberOfNeurons = int.Parse(childInput.text);
                    layers[index] = numberOfNeurons;
                }
                index++;
            }
            // Population Size
            InputField populationSize = GameObject.Find("PopulationInput").GetComponent<InputField>();
            population = int.Parse(populationSize.text);
            // Selection type
            Dropdown selectionTypeDropdown = GameObject.Find("SelectionTypeDropdown").GetComponent<Dropdown>();
            int dropdownValue = selectionTypeDropdown.value;
            selectionTypeName = selectionTypeDropdown.options[dropdownValue].text;
            evoMan.selectionType = selectionTypeName;
            // Mutation Probability
            InputField mutationProb = GameObject.Find("MutationProbabilityInput").GetComponent<InputField>();
            mutationProbability = float.Parse(mutationProb.text);
            // Mutation Amount
            InputField mutationAmountInput = GameObject.Find("MutationAmountInput").GetComponent<InputField>();
            mutationAmount = float.Parse(mutationAmountInput.text);
            // Set number of tournament Winners
            if (selectionTypeDropdown.value == 2)
            {
                InputField tournamentWinners = GameObject.Find("TournamentWinnersInput").GetComponent<InputField>();
                numberOfTournamentWinners = int.Parse(tournamentWinners.text);
            }
            // Set bools in EvolutionManager.cs
            evoMan.safety = GameObject.Find("TrainingToggle").GetComponent<Toggle>().isOn;
            evoMan.training = true;
        }
        // Set variables irrelevant of Test or Training chosen
        // Standard Deviation
        Slider stdDevSlider = GameObject.Find("StdDevSlider").GetComponent<Slider>();
        stdDev = stdDevSlider.value;
        // Log Progress Toggle   
        Toggle compareToggle = GameObject.Find("LogProgressToggle").GetComponent<Toggle>();
        logProgress = compareToggle.isOn;
        evoMan.logProgress = this.logProgress;  
        // If Log Progress is checked activate InputField for number of generations logged and for path to save textfile to
        if (compareToggle.isOn)
        {
            InputField stopAtGeneration = GameObject.Find("StopAtGenerationInput").GetComponent<InputField>();
            stopGenerationNumber = int.Parse(stopAtGeneration.text);
            evoMan.stopNumber = stopGenerationNumber;
            InputField saveNeuralNet = GameObject.Find("SaveLogInput").GetComponent<InputField>();
            string savePath = saveNeuralNet.text;
            evoMan.textPath = savePath;
        }   
        // Starts the simulation by toggling bools in UIController.cs and UISimulationController.cs
        uiCon.start = true;
        uiSimCon.start = true;
        // Deactivates GUI after start
        simulationPanel.SetActive(false);
    }
    // Refreshes Standard Deviation Slider text value
    public void ChangeStdDevSliderText(Slider stdDevSlider)
    {
        GameObject.Find("StdDevInputField").GetComponent<InputField>().text = stdDevSlider.value.ToString();
    }
    // Updates Slider if InputField value is changed
    public void OnStdDevInputChanged(InputField stdDevInput)
    {
        GameObject.Find("StdDevSlider").GetComponent<Slider>().value = float.Parse(stdDevInput.text);
    }
    // Changes the number of input fields below. Can't go higher than 8 and has to be at least 2 (for input and output layer)
    public void ChangeNeuralLayerCount(InputField layers)
    {
        int numberOfLayers = int.Parse(layers.text);
        
        if (numberOfLayers < 1)
        {
            layers.text = "1";
            numberOfLayers = 1;
        }
        if (numberOfLayers > 6)
        {
            layers.text = "6";
            numberOfLayers = 6;
        }

        GameObject layersObject = GameObject.Find("LayersPanel");

        // Destroy old clones first
        foreach (Transform child in layersObject.transform)
        {
            if (child.name == "HiddenLayer(Clone)")
            {
                GameObject.Destroy(child.gameObject);
            }
            
        }
        // Instantiate new input fields and set them in the hierarchy between input and output layer
        for (int i = 1; i < numberOfLayers; i++)
        {
            InputField newLayer = Instantiate(layer, layer.transform.position, Quaternion.identity);
            newLayer.interactable = true;
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
            saveLogPanel.SetActive(true);
        }
        else
        {
            stopAtGenerationPanel.SetActive(false);
            saveLogPanel.SetActive(false);
        }
    }
    // Caps the population at 80 for performance reasons
    public void OnPopulationChanged(InputField popSize)
    {
        int populationSize = int.Parse(popSize.text);
        if (populationSize < 0)
        {
            popSize.text = Mathf.Abs(populationSize).ToString();
        }
        else if (populationSize > 80)
        {
            popSize.text = "80";
        }
    }
    // Activates UI to select a number of tournament winners if Tournament Selection was as selection method picked
    public void OnTournamentWinnerChanged(InputField tournamentWinners)
    {
        int numberWinners = int.Parse(tournamentWinners.text);
        if (numberWinners < 0)
        {
            tournamentWinners.text = Mathf.Abs(numberWinners).ToString();
        }
    }
    // Caps the mutation probability between 0 and 1, for obvious reasons
    public void OnMutationProbChanged(InputField mutationProb)
    {
        float mutation = float.Parse(mutationProb.text);
        if (mutation < 0)
        {
            mutationProb.text = "0";
        }
        else if (mutation > 1)
        {
            mutationProb.text = "1";
        }
    }
    // Activates GUI for Test, deactivates all Settings, except Training/Test Toggles and Log Progress Checkbox, activates InputField to set path from where to load the neuralnetwork weights txt
    public void TestToggle(Toggle testToggle)
    {
        if(testToggle.isOn)
        {
            loadNeuralNetPanel.SetActive(true);
            saveNeuralNetPanel.SetActive(false);
            neuralNetworkPanel.SetActive(false);
            evolutionPanel.SetActive(false);
        }
        else
        {
            loadNeuralNetPanel.SetActive(false);
            saveNeuralNetPanel.SetActive(true);
            neuralNetworkPanel.SetActive(true);
            evolutionPanel.SetActive(true);
        }
    }
    // If Training Toggle is set, activate Auto Save Checkbox
    public void TrainingToggle(Toggle trainingToggle)
    {
        if (trainingToggle.isOn)
        {
            saveBestCarToggle.SetActive(true);
        }
        else
        {
            saveBestCarToggle.SetActive(false);
        }
    }
    // If Auto Save Toggle ist set, activate Save Path Input Field
    public void AutoSaveToggle(Toggle autoSaveToggle)
    {
        if (autoSaveToggle.isOn)
        {
            saveNeuralNetPanel.SetActive(true);
        }
        else
        {
            saveNeuralNetPanel.SetActive(false);
        }
    }
}
