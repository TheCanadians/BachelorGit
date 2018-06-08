using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private Canvas Canvas;
    private UISimulationController simulationUI;

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
        simulationUI = GetComponentInChildren<UISimulationController>(true);

        simulationUI.Show();
    }
}
