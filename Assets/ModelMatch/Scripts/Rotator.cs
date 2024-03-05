using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
	[SerializeField]
	private Vector3 axis;
	[SerializeField]
	private float speed;
	[SerializeField]
	private Space space;
	
	
    // Update is called once per frame
    void Update()
	{
		transform.Rotate(axis, speed * Time.deltaTime, space);
    }
}
