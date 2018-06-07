using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    public float radius = 3f;
    public float fitnessValue = 1f;

    private SpriteRenderer spriteRenderer;

    [Range(0, 50)]
    public int segments = 50;
    [Range(0, 3)]
    public float xradius = 3;
    [Range(0, 3)]
    public float yradius = 3;

    LineRenderer line;

    private void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = (segments + 1);
        line.useWorldSpace = false;
        CreatePoints();
    }

    void CreatePoints()
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, z, 0));

            angle += (360f / segments);
        }
    }
}
