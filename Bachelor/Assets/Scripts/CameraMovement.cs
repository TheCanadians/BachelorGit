using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [SerializeField]
    private float CamSpeed = 5f;

    private GameObject target;

	// Update is called once per frame
	void Update () {
		if (target != null)
        {
            Vector3 targetPos = target.transform.position;
            float x = targetPos.x;
            float y = targetPos.y;

            Vector3 homePos = this.transform.position;
            float homeZ = homePos.z;

            Vector3 newPos = new Vector3(x, y, homeZ);

            this.transform.position = Vector3.Lerp(this.transform.position, newPos, CamSpeed * Time.deltaTime);
        }
	}

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}
