﻿using System.Collections.Generic;
using System;

public class NeuralNetwork : IComparable<NeuralNetwork> {

    private int[] layers; // number of layers
    private float[][] neurons; // number of neurons per layer
    private float[][][] weights; // number of weights per neuron
    private float fitness; // fitness value of the network

    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        // Generate Neurons and Weights
        InitNeurons();
        InitWeights();
    }

    public NeuralNetwork(NeuralNetwork copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();
        CopyWeights(copyNetwork.weights);
    }

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
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
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
                // Hyerbolic tangent activation
                neurons[i][j] = (float)Math.Tanh(value);
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

                    // random number to check if to mutate weight
                    float randomNum = UnityEngine.Random.Range(0f, 100f);

                    if (randomNum <= 2f)
                    {
                        // flip sign
                        weight *= -1;
                    }
                    else if (randomNum <= 4f)
                    {
                        // new weight
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if (randomNum <= 6f)
                    {
                        // increase weight
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                        weight *= factor;
                    }
                    else if (randomNum <= 8f)
                    {
                        // decrease weight
                        float factor = UnityEngine.Random.Range(0f, 1f);
                        weight *= factor;
                    }
                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float fitnessValue)
    {
        fitness += fitnessValue;
    }

    public void SetFitness(float fitnessValue)
    {
        fitness = fitnessValue;
    }

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
}