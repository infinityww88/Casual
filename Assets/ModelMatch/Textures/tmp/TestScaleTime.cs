using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScaleTime : MonoBehaviour
{
	[SerializeField]
	private float _speed = 60f;
	
	private ScaleTime _scaleTime;
	
    // Start is called before the first frame update
    void Start()
    {
    	_scaleTime = GetComponent<ScaleTime>();   
    }

    // Update is called once per frame
    void Update()
    {
	    transform.Rotate(_scaleTime.DeltaTime * _speed,
		    _scaleTime.DeltaTime * _speed, 0);
    }
}
