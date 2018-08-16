using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class EvolutionManager : MonoBehaviour {

    // normal car prefab
    public GameObject carPrefab;

    private int populationSize = 20;
    private int[] layers; // Topology

    public bool training = false;
    public bool test = false;
    public bool isTraining = false;
    private bool isSpawning = true;
    private int generationNumber = 0;
    private List<NeuralNetwork> nets;
    private List<CarMovement> carList = null;

    private static System.Random randomizer = new System.Random();
    public string selectionType = "RouletteWheel";
    private int tournamentWinners;
    private bool logProgress;
    private int stopNumber;
    private bool lastLog = false;
    private bool readSave = false;
    public bool safety = false;

    public int averageFitness = 0;
    public float bestFitness = 0f;
    public float[][][] saveWeights;
    public float[][][] loadWeights;

    private List<string> compareList = new List<string>();
    private string textPath = "Assets/compare.txt";
    public string readPath;
    private StreamWriter writer;
    private StreamReader reader;

    public TextAsset file;

    PublicManager publicManager;

    private void Start()
    {
        // get user inputs
        publicManager = GameObject.Find("PublicManager").GetComponent<PublicManager>();
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
         * 2.2.2.3. Back to 2.1.
         */
        if (training)
        {
            if (!isTraining)
            {
                GetUserInput();
                // Initiate Neural Networks once after the start of the simulation
                InitNeuralNetworks();
                // Evaluate fitness of generation
                Eval();
                // Select parents based on selection method
                Select();

                generationNumber++;

                isTraining = true;
                isSpawning = true;
                // Create new generation of cars
                CreateCars();
            }
            if (isSpawning)
            {
                CheckAlive();
            }
        }
        else if (test)
        {
            if (!isTraining)
            {
                populationSize = 1;
                if (!readSave)
                {
                    readSave = true;
                    ReadNeuralNetworkFile(readPath);
                    InitNeuralNetworksTest();
                }
                generationNumber++;
                isTraining = true;
                isSpawning = true;
                CreateCars();
            }
            if (isSpawning)
            {
                CheckAlive();
            }
        }
    }
    private void GetUserInput()
    {
        this.populationSize = publicManager.population;
        this.layers = publicManager.layers;
        this.selectionType = publicManager.selectionTypeName;
        this.tournamentWinners = publicManager.numberOfTournamentWinners;
        this.logProgress = publicManager.logProgress;
        this.stopNumber = publicManager.stopGenerationNumber;
    }
    // Initiate neural networks for the population once
    private void InitNeuralNetworks()
    {
        // initialize only once
        if (generationNumber == 0)
        {
            nets = new List<NeuralNetwork>();
            // generates new random neural network and sets mutation settings, than mutates
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
    private void InitNeuralNetworksTest()
    {
        nets = new List<NeuralNetwork>();
        List<int> numberOfNeuronsPerLayer = new List<int>();
        numberOfNeuronsPerLayer.Add(5);
        for (int i = 0; i < loadWeights.Length; i++)
        {
            numberOfNeuronsPerLayer.Add(loadWeights[i].Length);
        }
        int[] loadedTopology = numberOfNeuronsPerLayer.ToArray();
        NeuralNetwork testNet = new NeuralNetwork(loadedTopology);
        testNet.SetWeightsMatrix(loadWeights);
        nets.Add(testNet);
    }
    // Evaluate the generation based on fitness scores
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
            // sorts Array based on fitness, worst fitness score first, best fitness score last
            nets.Sort();
        }

    }
    // Select parents based on chosen selection method
    private void Select()
    {
        List<NeuralNetwork> newNets = new List<NeuralNetwork>();
        if (selectionType == "Elitist" || selectionType == "Elitist Selection")
        {
            newNets = ElitistSelection();
        }
        else if (selectionType == "RouletteWheel" || selectionType == "RouletteWheel Selection")
        {
            newNets = RouletteWheelSelection();
        }
        else if (selectionType == "Rank" || selectionType == "Rank Selection")
        {
            newNets = RankSelection();
        }
        else if (selectionType == "Tournament" || selectionType == "Tournament Selection")
        {
            newNets = TournamentSelection();
        }
        else if (selectionType == "Random" || selectionType == "Random Selection")
        {
            newNets = RandomSelection();
        }

        nets = newNets;

        for (int i = 0; i < populationSize; i++)
        {
            nets[i].SetFitness(0f);
        }
    }
    // Selection Type Roulette Wheel Selection.
    // Chooses randomly two parents. Cars with higher fitness scores have a higher probability to be chosen
    private List<NeuralNetwork> RouletteWheelSelection()
    {
        // generate new List for the new generation
        List<NeuralNetwork> newNets = new List<NeuralNetwork>();
        SetFitnessPercent(nets);

        for (int i = 0; i < populationSize / 2; i++)
        {
            NeuralNetwork parentA = PickParent(nets);
            NeuralNetwork parentB = PickParent(nets);

            NeuralNetwork[] newNet = Crossover(parentA, parentB);

            newNet[0].Mutate();
            newNet[1].Mutate();
            newNets.Add(new NeuralNetwork(newNet[0]));
            newNets.Add(new NeuralNetwork(newNet[1]));
        }
        return newNets;
    }
    // Selection Type Rank Selection
    // Chosses randomly two parents. Cars with a higher rank (based on fitness scores) have a higher probability to be chosen. Works similar to roulette wheel selection.
    private List<NeuralNetwork> RankSelection()
    {
        // generate new List for the new generation
        List<NeuralNetwork> newNets = new List<NeuralNetwork>();
        SetRankPercent(nets);

        for (int i = 0; i < populationSize / 2; i++)
        {
            NeuralNetwork parentA = PickParent(nets);
            NeuralNetwork parentB = PickParent(nets);

            NeuralNetwork[] newNet = Crossover(parentA, parentB);

            newNet[0].Mutate();
            newNet[1].Mutate();
            newNets.Add(new NeuralNetwork(newNet[0]));
            newNets.Add(new NeuralNetwork(newNet[1]));
        }
        return newNets;
    }
    // Selection Type Tournament Selection
    // selects x cars randomly and choses the car with the highest fitness score as one parent. Do twice.
    private List<NeuralNetwork> TournamentSelection()
    {
        // generate new List for the new generation
        List<NeuralNetwork> newNets = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize / 2; i++)
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

            NeuralNetwork[] newNet = Crossover(parentA, parentB);

            newNet[0].Mutate();
            newNet[1].Mutate();
            newNets.Add(new NeuralNetwork(newNet[0]));
            newNets.Add(new NeuralNetwork(newNet[1]));
        }
        return newNets;
    }
    // Selection Type Elitist Selection.
    // Chooses the two cars with the highest fitness score and generates a new generation from their Genes
    private List<NeuralNetwork> ElitistSelection()
    {
        // generate new List for the new generation
        List<NeuralNetwork> newNets = new List<NeuralNetwork>();
        NeuralNetwork parentA = new NeuralNetwork(nets[nets.Count - 1]);
        NeuralNetwork parentB = new NeuralNetwork(nets[nets.Count - 2]);

        for (int i = 0; i < populationSize / 2; i++)
        {
            NeuralNetwork[] newNet = Crossover(parentA, parentB);

            newNet[0].Mutate();
            newNet[1].Mutate();
            newNets.Add(new NeuralNetwork(newNet[0]));
            newNets.Add(new NeuralNetwork(newNet[1]));
        }
        return newNets;
    }
    // Selection Type Random Selection
    // Selects two parents completely randomly
    private List<NeuralNetwork> RandomSelection()
    {
        // generate new List for the new generation
        List<NeuralNetwork> newNets = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize / 2; i++)
        {
            int a = Random.Range(0, nets.Count - 1);
            int b = Random.Range(0, nets.Count - 1);

            NeuralNetwork parentA = new NeuralNetwork(nets[a]);
            NeuralNetwork parentB = new NeuralNetwork(nets[b]);

            NeuralNetwork[] newNet = Crossover(parentA, parentB);

            newNet[0].Mutate();
            newNet[1].Mutate();
            newNets.Add(new NeuralNetwork(newNet[0]));
            newNets.Add(new NeuralNetwork(newNet[1]));
        }
        return newNets;
    }
    // Adds the fitness values of all cars together and calculates each cars score as follows:
    // score = fitnessValue / SUM(fitnessValuesOfAllCars)
    private void SetFitnessPercent(List<NeuralNetwork> nets)
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
    // Adds the ranks of all cars together and calculates each cars score as follows:
    // score = rank / SUM(RanksOfAllCars)
    private void SetRankPercent(List<NeuralNetwork> nets)
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
    // randomly picks a parent for Roulette Wheel and Rank Selection
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
    // Performs One-Point Crossover over the two parent cars. Generates two new gene sets for the next generation
    private NeuralNetwork[] Crossover(NeuralNetwork a, NeuralNetwork b)
    {
        float[][][] aWeights = a.GetWeightsMatrix();
        float[][][] bWeights = b.GetWeightsMatrix();
        NeuralNetwork[] newNeuralNetwork = new NeuralNetwork[2];
        newNeuralNetwork[0] = new NeuralNetwork(a);
        newNeuralNetwork[1] = new NeuralNetwork(b);
        float[][][] newWeightsA = aWeights;
        float[][][] newWeightsB = aWeights;

        int splitPoint1A = Random.Range(0, aWeights.Length);
        int splitPoint1B = Random.Range(0, aWeights[splitPoint1A].Length);
        int splitPoint1C = Random.Range(0, aWeights[splitPoint1A][splitPoint1B].Length);

        for (int i = 0; i < aWeights.Length; i++)
        {
            for (int j = 0; j < aWeights[i].Length; j++)
            {
                for (int k = 0; k < aWeights[i][j].Length; k++)
                {
                    if (i >= splitPoint1A && j >= splitPoint1B && k >= splitPoint1C)
                    {
                        newWeightsA[i][j][k] = bWeights[i][j][k];
                        newWeightsB[i][j][k] = aWeights[i][j][k];
                    }
                    else
                    {
                        newWeightsA[i][j][k] = bWeights[i][j][k];
                        newWeightsB[i][j][k] = aWeights[i][j][k];
                    }
                }
            }
        }
        newNeuralNetwork[0].SetWeightsMatrix(newWeightsA);
        newNeuralNetwork[1].SetWeightsMatrix(newWeightsB);

        return newNeuralNetwork;
    }
    // Check if all Cars have died. If all cars have dies restarts with new generation
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
                float bestGenFitness;
                bestGenFitness = GetBestCar();
                // logs the average fitness for every generation in a text file
                if (logProgress)
                {
                    string log = generationNumber + "   Best Fitness: " + bestGenFitness +  "   Average Fitness: " + averageFitness + "   Selection Type: " + selectionType;
                    compareList.Add(log);

                    GameObject checkpointsParent = GameObject.Find("Checkpoints");
                    Checkpoint lastCheckpoint = checkpointsParent.transform.GetChild(checkpointsParent.transform.childCount - 1).GetComponent<Checkpoint>();

                    if (generationNumber <= stopNumber && bestGenFitness <= lastCheckpoint.fitnessValue && !lastLog)
                    {
                        writer.WriteLine(log);
                    }
                    else
                    {
                        if (!lastLog)
                        {
                            writer.WriteLine(log);
                            lastLog = true;
                        }
                        writer.Close();
                    }
                }

               

                Invoke("StartTraining", 1f);
            }
        }
    }
    // calculates the average fitness in percent for the last generation
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
    // selects overall (over all generations) best fitness score
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
            bestFitness = bestCar;
            if (bestFitness >= 33 && !test && safety)
            {
                saveWeights = carsNetworks[carsNetworks.Length - 1].GetWeightsMatrix();
                SaveBestNeuralNetwork(saveWeights);
                safety = false;
            }
        }
        return bestCar;
    }

    public void SaveBestNeuralNetwork(float[][][] array)
    {
        string savePath = Random.Range(0, 100000).ToString();
        savePath = "Assets/" + generationNumber + "-" + savePath + ".txt";
        StreamWriter writer2 = new StreamWriter(savePath, true);

        for (int i = 0; i < array.Length; i++)
        {
            writer2.WriteLine("Layer");
            for (int j = 0; j < array[i].Length; j++)
            {
                string neuronWeights = "";
                for (int k = 0; k < array[i][j].Length; k++)
                {
                    if (k < array[i][j].Length - 1)
                    {
                        neuronWeights += array[i][j][k].ToString() + " ";
                    }
                    else
                    {
                        neuronWeights += array[i][j][k].ToString();
                    }
                    
                }
                writer2.WriteLine(neuronWeights);
            }
        }

        writer2.Close();
        AssetDatabase.ImportAsset(savePath);
        TextAsset asset = (TextAsset)Resources.Load(savePath);
    }

    public void ReadNeuralNetworkFile(string neuralNetworkFilePath)
    {
        bool sameLayer = true;
        reader = new StreamReader(neuralNetworkFilePath);

        float[][][] loadedWeights;
        List<float[][]> loadedWeightsList = new List<float[][]>();

        while (reader.Peek() >= 0)
        {
            string layerLine = reader.ReadLine();

            if (layerLine == "Layer")
            {
                List<float[]> loadedWeightsLayer = new List<float[]>();
                List<string> layerStringList = new List<string>();

                while (sameLayer)
                {
                    int nextLine = reader.Peek();
                    if (nextLine != 76 && reader.Peek() >= 0)
                    {
                        string readNextLine = reader.ReadLine();
                        layerStringList.Add(readNextLine);
                    }
                    else
                    {
                        sameLayer = false;
                    }
                }
                foreach(string line in layerStringList)
                {
                    List<float> loadedWeightsNeuron = new List<float>();
                    string[] splitLine = line.Split(' ');
                    foreach(string weight in splitLine)
                    {
                        loadedWeightsNeuron.Add((float)double.Parse(weight));
                    }
                    loadedWeightsLayer.Add(loadedWeightsNeuron.ToArray());
                }
                loadedWeightsList.Add(loadedWeightsLayer.ToArray());
                sameLayer = true;
            }
        }
        loadedWeights = loadedWeightsList.ToArray();
        loadWeights = loadedWeights;
        reader.Close();
    }
    // Create new Car Objects
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
    // Compare function to compare two neural Networks by fitness
    int CompareFitness(NeuralNetwork x, NeuralNetwork y)
    {
        return x.GetFitness().CompareTo(y.GetFitness());
    }
    // return generation Number
    public int GetGenerationCount()
    {
        return generationNumber;
    }
    // Set generation Number
    public void SetGenerationNumber(int newGenerationNumber)
    {
        generationNumber = newGenerationNumber;
    }
}
