using UnityEngine;
using System.Collections;

public class CockpitThrottleAnimator : MonoBehaviour {

    const float MIN = -4.5f;
    const float MAX = -11.5f;

    const float DELTA = MAX - MIN;

	// Use this for initialization
	public void Start () {
	
	}
	
	// Update is called once per frame
	public void Update ()
	{
        var pos = gameObject.transform.localPosition;

        pos.y = (Input.GetAxis("Throttle") / 2 + 0.5f)*DELTA + MIN;

	    gameObject.transform.localPosition = pos;
	}
}
