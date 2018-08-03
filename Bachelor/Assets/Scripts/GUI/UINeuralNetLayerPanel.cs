using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINeuralNetLayerPanel : MonoBehaviour
{

    [SerializeField]
    private RectTransform LayerContents;
    [SerializeField]
    public List<UINeuralNetworkWeightPanel> Nodes;

    private NeuralNetwork net;

    public void SetNeuralNet(NeuralNetwork net)
    {
        this.net = net;
    }

    public void Display(int layer, bool bias)
    {
        Display((uint)net.GetNeuronsInLayer(layer), bias);
    }

    public void Display(uint neuronCount, bool bias)
    {
        UINeuralNetworkWeightPanel dummyNode = Nodes[0];

        for (int i = Nodes.Count; i < neuronCount; i++)
        {
            UINeuralNetworkWeightPanel newNode = Instantiate(dummyNode);
            newNode.transform.SetParent(LayerContents.transform, false);
            Nodes.Add(newNode);
            if (i == neuronCount - 1 && bias)
            {
                Color oldColor = newNode.GetComponent<Image>().color;
                Color newColor = new Color(237, 0, 255, 1);
                newNode.GetComponent<Image>().color = newColor;
            }
        }

        for (int i = this.Nodes.Count - 1; i >= neuronCount; i++)
        {
            UINeuralNetworkWeightPanel toBeDestroyed = Nodes[i];
            Nodes.RemoveAt(i);
            Destroy(toBeDestroyed);
        }
    }

    public void DisplayConnections(int currentLayer, UINeuralNetLayerPanel nextLayer, bool biasLayer)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].SetNeuralNet(net);
            Nodes[i].DisplayConnections(i, currentLayer, nextLayer, biasLayer);
        }
    }

    public void HideAllConnections()
    {
        foreach (UINeuralNetworkWeightPanel node in Nodes)
        {
            node.HideConnections();
        }
    }
}
