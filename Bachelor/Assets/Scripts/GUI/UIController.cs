using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private Canvas Canvas;
    private UISimulationController simulationUI;

    public bool start = false;

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
        simulationUI = GetComponentInChildren<UISimulationController>(true);
    }

    private void Update()
    {
        // Wait for StartButton Press
        if (start)
        {
            simulationUI.Show();
            start = false;
        }
    }
}
