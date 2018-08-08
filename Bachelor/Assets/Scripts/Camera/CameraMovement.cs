using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    // Speed the camera moves from one car to another
    [SerializeField]
    private float CamSpeed = 5f;
    // target the camera follows
    private GameObject target;

	// Update is called once per frame
	void Update () {
		if (target != null)
        {
            // get target position and current position
            Vector3 targetPos = target.transform.position;
            float x = targetPos.x;
            float y = targetPos.y;

            Vector3 homePos = this.transform.position;
            float homeZ = homePos.z;

            Vector3 newPos = new Vector3(x, y, homeZ);
            // Lerp camera position to new target position
            this.transform.position = Vector3.Lerp(this.transform.position, newPos, CamSpeed * Time.deltaTime);
        }
	}
    // Set new camera target
    public void SetTarget(GameObject target)
    {
        this.target = target;
    }
}
