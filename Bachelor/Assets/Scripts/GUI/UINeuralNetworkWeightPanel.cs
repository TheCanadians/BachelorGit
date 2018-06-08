using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINeuralNetworkWeightPanel : MonoBehaviour {

    [SerializeField]
    private List<Image> Weights;
    [SerializeField]
    private Color PositiveColor;
    [SerializeField]
    private Color NegativeColor;

    private NeuralNetwork net;

    public void SetNeuralNet(NeuralNetwork net)
    {
        this.net = net;
    }

    public void DisplayConnections(int neuronIndex, int currentLayer, UINeuralNetLayerPanel nextLayer)
    {
        Image dummyWeight = Weights[0];
        dummyWeight.gameObject.SetActive(true);

        for (int i = Weights.Count; i < net.GetNeuronsInLayer(currentLayer + 1); i++)
        {
            Image newWeight = Instantiate(dummyWeight);
            newWeight.transform.SetParent(this.transform, false);
            Weights.Add(newWeight);
        }

        for (int i = this.Weights.Count - 1; i >= net.GetNeuronsInLayer(currentLayer + 1); i++)
        {
            Image toBeDestroyed = Weights[i];
            Weights.RemoveAt(i);
            Destroy(toBeDestroyed);
        }

        for (int i = 0; i < Weights.Count; i++)
        {
            float[][][] weights = net.GetWeightsMatrix();
            PositionConnection(Weights[i], nextLayer.Nodes[i], neuronIndex, i, weights[currentLayer]);
        }
    }

    public void HideConnections()
    {
        for (int i = this.Weights.Count - 1; i >= 1; i++)
        {
            Image toBeDestroyed = Weights[i];
            Weights.RemoveAt(i);
            Destroy(toBeDestroyed);
        }

        Weights[0].gameObject.SetActive(false);
    }

    private void PositionConnection(Image connection, UINeuralNetworkWeightPanel otherNode, int nodeIndex, int connectedNodeIndex, float[][] weights)
    {
        connection.transform.localPosition = Vector3.zero;

        Vector2 sizeDelta = connection.rectTransform.sizeDelta;
        float weight = weights[nodeIndex][connectedNodeIndex];
        sizeDelta.x = (float)System.Math.Abs(weight);
        if (sizeDelta.x < 1)
        {
            sizeDelta.x = 1;
        }

        if (weight >= 0)
        {
            connection.color = PositiveColor;
        }
        else
        {
            connection.color = NegativeColor;
        }

        Vector2 connectionVector = this.transform.position - otherNode.transform.position;
        sizeDelta.y = connectionVector.magnitude / GameObject.Find("UI").GetComponent<Canvas>().scaleFactor;

        connection.rectTransform.sizeDelta = sizeDelta;

        float angle = Vector2.Angle(Vector2.up, connectionVector);
        connection.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
    }
}
