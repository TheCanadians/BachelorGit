using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINeuralNetworkGraph : MonoBehaviour {

    [SerializeField]
    private List<Image> AverageConnections;
    [SerializeField]
    private List<Image> BestConnections;
    [SerializeField]
    private Color AverageColor;
    [SerializeField]
    private Color BestColor;

    private List<float> bestCarList = new List<float>();
    private List<float> averageCarList = new List<float>();

	public void AddBestCar(float bestCarFitness)
    {
        bestCarList.Add(bestCarFitness);
        Debug.Log(bestCarFitness);
    }

    public void AddAverageCar(float averageCarFitness)
    {
        averageCarList.Add(averageCarFitness);
        Debug.Log(averageCarFitness);
    }

    public void Position()
    {
        PositionConnectionAverage();
        PositionConnectionBest();
    }

    private void PositionConnectionAverage()
    {
        for (int i = 1; i < averageCarList.Count; i++)
        {
            Image toBeDestroyed = AverageConnections[i];
            AverageConnections.RemoveAt(i);
            Destroy(toBeDestroyed);
        }

        for (int i = 0; i < averageCarList.Count; i++)
        {
            Debug.Log(i);
            Image dummyConnection = AverageConnections[0];
            dummyConnection.gameObject.SetActive(true);

            Image connection = Instantiate(dummyConnection);
            connection.transform.SetParent(this.transform, false);
            AverageConnections.Add(connection);

            connection.transform.localPosition = Vector3.zero;

            Vector2 sizeDelta = connection.rectTransform.sizeDelta;

            sizeDelta.x = 1f;

            connection.color = AverageColor;

            Color var = connection.color;
            var.a = 1f;
            connection.color = var;


            Vector2 lastPos;
            if (i == 0)
            {
                lastPos = new Vector2(0, 0);
            }
            else
            {
                lastPos = new Vector2(averageCarList[i - 1], i - 1);
            }
            
            Vector2 pos = new Vector2(averageCarList[i], i);

            Vector2 connectionVector = lastPos - pos;
            sizeDelta.y = connectionVector.magnitude / GameObject.Find("UI").GetComponent<Canvas>().scaleFactor;

            connection.rectTransform.sizeDelta = sizeDelta;

            float angle = Vector2.Angle(Vector2.up, connectionVector);
            connection.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        }
    }

    private void PositionConnectionBest()
    {
        for (int i = 1; i < averageCarList.Count; i++)
        {
            Image toBeDestroyed = BestConnections[i];
            BestConnections.RemoveAt(i);
            Destroy(toBeDestroyed);
        }

        for (int i = 0; i < averageCarList.Count; i++)
        {
            Image dummyConnection = BestConnections[0];
            dummyConnection.gameObject.SetActive(true);

            Image connection = Instantiate(dummyConnection);
            connection.transform.SetParent(this.transform, false);
            BestConnections.Add(connection);
            connection.transform.localPosition = Vector3.zero;

            Vector2 sizeDelta = connection.rectTransform.sizeDelta;

            connection.color = BestColor;

            Color var = connection.color;
            var.a = 1f;
            connection.color = var;

            Vector2 lastPos;
            if (i == 0)
            {
                lastPos = new Vector2(0, 0);
            }
            else
            {
                lastPos = new Vector2(bestCarList[i - 1], i - 1);
            }

            Vector2 pos = new Vector2(bestCarList[i], i);

            Vector2 connectionVector = lastPos - pos;
            sizeDelta.y = connectionVector.magnitude / GameObject.Find("UI").GetComponent<Canvas>().scaleFactor;

            connection.rectTransform.sizeDelta = sizeDelta;

            float angle = Vector2.Angle(Vector2.up, connectionVector);
            connection.transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, 1));
        }
    }
}
