using UnityEngine;
using System.Collections;

public class CockpitStickAnimator : MonoBehaviour {
    
    private const float X_ANG = 10;
    private const float Y_ANG = 10;

	// Use this for initialization
	public void Start () {
	
	}
	
	// Update is called once per frame
    public void Update()
    {
        var eAng = gameObject.transform.localEulerAngles;

        eAng.x = -Input.GetAxis("Pitch")*X_ANG;
        eAng.y = -Input.GetAxis("Roll")*Y_ANG;

        gameObject.transform.localEulerAngles = eAng;
    }
}
