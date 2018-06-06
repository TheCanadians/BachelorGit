using UnityEngine;

public class Sensor : MonoBehaviour {

    // Layer that sensors should sense
    [SerializeField]
    private LayerMask SensorLayer;
    // EndPoint Sprite
    [SerializeField]
    private SpriteRenderer EndPoint;

    // Max sensor distance
    [SerializeField]
    private const float MAX_DISTANCE = 10f;

    private float distance;

    private void Start()
    {
        EndPoint.gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        // Vector between EndPoint and Sensor
        Vector2 sensorDirection = EndPoint.transform.position - this.transform.position;
        sensorDirection.Normalize();

        // Raycast to check for wall hit
        RaycastHit2D wallHit = Physics2D.Raycast(this.transform.position, sensorDirection, MAX_DISTANCE, SensorLayer);

        // Check what was hit
        if (wallHit.collider == null)
        {
            wallHit.distance = MAX_DISTANCE;
        }

        distance = wallHit.distance;
        // Set EndPoint position to raycast hit position
        EndPoint.transform.position = (Vector2)this.transform.position + sensorDirection * wallHit.distance;
    }

    public float GetDistance()
    {
        return distance;
    }

    // Help functions
    // Hide Endpoint (after collision)
    public void Hide()
    {
        EndPoint.gameObject.SetActive(false);
    }
    // Show EndPoint (after initializing)
    public void Show()
    {
        EndPoint.gameObject.SetActive(true);
    }
}
