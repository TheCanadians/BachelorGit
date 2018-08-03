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

    public void DisplayConnections(int neuronIndex, int currentLayer, UINeuralNetLayerPanel nextLayer, bool biasLayer)
    {
        Image dummyWeight = Weights[0];
        dummyWeight.gameObject.SetActive(true);
        for (int i = Weights.Count; i < net.GetNeuronsInLayer(currentLayer + 1); i++)
        {
            if (biasLayer && i == net.GetNeuronsInLayer(currentLayer + 1) - 1)
            {

            }
            else
            {
                Image newWeight = Instantiate(dummyWeight);
                newWeight.transform.SetParent(this.transform, false);
                Weights.Add(newWeight);
            }

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
        try
        {
            connection.transform.localPosition = Vector3.zero;

            Vector2 sizeDelta = connection.rectTransform.sizeDelta;
            float weight = weights[connectedNodeIndex][nodeIndex];
            sizeDelta.x = (float)System.Math.Abs(weight * 2);
            if (sizeDelta.x < 1)
            {
                sizeDelta.x = 1;
            }
            else if (sizeDelta.x > 3f)
            {
                sizeDelta.x = 3;
            }

            if (weight >= 0)
            {
                connection.color = PositiveColor;
            }
            else
            {
                connection.color = NegativeColor;
            }
            Color var = connection.color;
            var.a = 1f;
            connection.color = var;
            Vector2 connectionVector = this.transform.position - otherNode.transform.position;
            sizeDelta.y = connectionVector.magnitude / GameObject.Find("UI").GetComponent<Canvas>().scaleFactor;

            connection.rectTransform.sizeDelta = sizeDelta;

            float angle = Vector2.Angle(Vector2.up, connectionVector);
            connection.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        }
        catch(System.IndexOutOfRangeException ex)
        {
            
        }
    }
}
