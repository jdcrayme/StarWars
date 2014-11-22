using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour {

    #region Public Fields
    public static Globals Instance;  //The singleton instance of the grid

    //public GameObject Label3D;      //The label prefab used for contacts

    //public Player Player = new Player();
    #endregion

	// Use this for initialization
	void Start () {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
