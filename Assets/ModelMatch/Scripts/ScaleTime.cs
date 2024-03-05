using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleTime : MonoBehaviour
{
	[SerializeField]
	private float _scale;
	
	private float _startTime = 0;
	private float _time = 0;
	
    // Start is called before the first frame update
    void Start()
    {
	    _time = _startTime = Time.time;
    }
    
	public float StartTime => _startTime;
	public float CurrTime => _time;
	public float DeltaTime => Time.deltaTime * _scale;

    // Update is called once per frame
    void Update()
    {
	    _time += Time.deltaTime * _scale;
    }
}
