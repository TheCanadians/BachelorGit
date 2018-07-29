using System.Collections.Generic;
using System;
using UnityEngine;

public class NeuralNetwork : IComparable<NeuralNetwork> {

    private int[] layers; // number of layers
    private float[][] neurons; // number of neurons per layer
    private float[][][] weights; // number of weights per neuron
    private float fitness; // fitness value of the network
    public float score = 0f;

    private PublicManager pM = GameObject.Find("PublicManager").GetComponent<PublicManager>();
    public String activationFnc;

    public float mutationProbability = 0.01f;
    public float mutationAmount = 2f;
    private static System.Random randomizer = new System.Random();

    // Initiate neural network (occurs only on simulation start
    public NeuralNetwork(int[] layers)
    {
        activationFnc = pM.activationFnc.ToString();
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        // Generate Neurons and Weights
        InitNeurons();
        InitWeights();
    }
    // generate network based on given weights matrix
    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        activationFnc = pM.activationFnc.ToString();
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();
        CopyWeights(copyNetwork.weights);
    }
    // copy all weights
    private void CopyWeights(float[][][] copyWeights)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    // Create Neurons
    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++) // every Neural Network layer
        {
            neuronsList.Add(new float[layers[i]]); // add layer to List
        }

        neurons = neuronsList.ToArray(); // convert List to Array
    }

    // Create Weights
    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();

        // loop through all neurons that have a weight connected to them
        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> weightsPerLayerList = new List<float[]>();

            int neuronsInPrevLayer = layers[i - 1];
            //loop through all neurons in this layer
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPrevLayer];

                // loop over all neurons in the previous layer
                for (int k = 0; k < neuronsInPrevLayer; k++)
                {
                    // set weights to random value between -0.5f and 0.5f
                    neuronWeights[k] = UnityEngine.Random.Range(-1f, 1f);
                }

                // add to List weightsPerLayerList
                weightsPerLayerList.Add(neuronWeights);
            }

            // add weightsPerLayerList to weightsList and convert to Array
            weightsList.Add(weightsPerLayerList.ToArray());
        }

        // convert to array
        weights = weightsList.ToArray();
    }

    // Feed Forward method to get outputs
    public float[] FeedForward(float[] inputs)
    {
        // Add inputsto input neurons
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // loop over all neurons and compute neural net values
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;

                for (int k = 0; k < neurons[i-1].Length; k++)
                {
                    // sum all weights connected from previous layers neurons to this neuron
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                if (activationFnc == "Sigmoid") neurons[i][j] = (float)SigmoidFunction(value);
                else if (activationFnc == "TanH") neurons[i][j] = (float)TanHFunction(value);
                else if (activationFnc == "SoftSign") neurons[i][j] = (float)SoftSignFunction(value);
            }
        }
        // return output values
        return neurons[neurons.Length - 1];
    }

    // Mutate Neural Network Weights
    public void Mutate()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    if (randomizer.NextDouble() <= mutationProbability)
                    {
                        // Mutate by random amount in range [-mutationAmount, mutationAmount]
                        weight += (float)(randomizer.NextDouble() * (mutationAmount * 2) - mutationAmount);
                    }
                    weights[i][j][k] = weight;
                }
            }
        }
    }
    // Sigmoid activation function
    private static double SigmoidFunction(double value)
    {
        if (value > 10) return 1.0;
        else if (value < -10) return 0.0;
        else return 1.0 / (1.0 + Math.Exp(-value));
        //return 1.0 / (1.0 + Math.Exp(-value));
    }
    // TanH activation function
    private static double TanHFunction(double value)
    {
        if (value > 10) return 1.0;
        if (value < -10) return -1.0;
        else return Math.Tanh(value);
        //return Math.Tanh(value);
    }
    // Soft Sign activation function
    private static double SoftSignFunction(double value)
    {
        return value / (1 + Math.Abs(value));
    }
    // Add x to fitness score
    public void AddFitness(float fitnessValue)
    {
        fitness += fitnessValue;
    }
    // Set fitness score
    public void SetFitness(float fitnessValue)
    {
        fitness = fitnessValue;
    }
    // return fitness score
    public float GetFitness()
    {
        return fitness;
    }

    // Compare two neural and sort based on fitness
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null) return 1;

        if (fitness > other.fitness)
        {
            return 1;
        }
        else if (fitness < other.fitness)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    } 
    // return layers array
    public int[] GetLayers()
    {
        return layers;
    }
    // return number of neurons in layer x
    public int GetNeuronsInLayer(int layer)
    {
        int count = neurons[layer].Length;
        return count;
    }
    // return weights matrix
    public float[][][] GetWeightsMatrix()
    {
        return weights;
    }
    // set weights matrix
    public void SetWeightsMatrix(float[][][] weights)
    {
        this.weights = weights;
    }
}
