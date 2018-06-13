using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINeuralNetworkPanel : MonoBehaviour
{

    [SerializeField]
    public List<UINeuralNetLayerPanel> Layers;

    private NeuralNetwork net;

    public void Display(NeuralNetwork net)
    {
        UINeuralNetLayerPanel dummyLayer = Layers[0];

        for (int i = Layers.Count; i < net.GetLayers().Length; i++)
        {
            UINeuralNetLayerPanel newPanel = Instantiate(dummyLayer);
            newPanel.transform.SetParent(this.transform, false);
            Layers.Add(newPanel);
        }

        for (int i = this.Layers.Count - 1; i >= net.GetLayers().Length; i++)
        {
            UINeuralNetLayerPanel toBeDestroyed = Layers[i];
            Layers.RemoveAt(i);
            Destroy(toBeDestroyed);
        }

        for (int i = 0; i < this.Layers.Count - 1; i++)
        {
            this.Layers[i].SetNeuralNet(net);
            int[] layers = net.GetLayers();
            this.Layers[i].Display(i);
        }

        this.Layers[Layers.Count - 1].SetNeuralNet(net);
        this.Layers[Layers.Count - 1].Display(net.GetLayers().Length - 1);

        StartCoroutine(DrawConnections(net));
    }

    private IEnumerator DrawConnections(NeuralNetwork net)
    {
        yield return new WaitForEndOfFrame();

        int[] layers = net.GetLayers();
        for (int i = 0; i < this.Layers.Count - 1; i++)
        {
            this.Layers[i].DisplayConnections(i, this.Layers[i + 1]);
        }

        this.Layers[this.Layers.Count - 1].HideAllConnections();
    }
}
