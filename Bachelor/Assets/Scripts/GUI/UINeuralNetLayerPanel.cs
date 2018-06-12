using System.Collections.Generic;
using UnityEngine;

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

    public void Display(int layer)
    {
        Display((uint)net.GetNeuronsInLayer(layer));
    }

    public void Display(uint neuronCount)
    {
        UINeuralNetworkWeightPanel dummyNode = Nodes[0];

        for (int i = Nodes.Count; i < neuronCount; i++)
        {
            UINeuralNetworkWeightPanel newNode = Instantiate(dummyNode);
            newNode.transform.SetParent(LayerContents.transform, false);
            Nodes.Add(newNode);
        }

        for (int i = this.Nodes.Count - 1; i >= neuronCount; i++)
        {
            UINeuralNetworkWeightPanel toBeDestroyed = Nodes[i];
            Nodes.RemoveAt(i);
            Destroy(toBeDestroyed);
        }
    }

    public void DisplayConnections(int currentLayer, UINeuralNetLayerPanel nextLayer)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].SetNeuralNet(net);
            Nodes[i].DisplayConnections(i, currentLayer, nextLayer);
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
