using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Touch;
using DG.Tweening;

public class TestInput : MonoBehaviour
{
	public float duration = 0.3f;
	private bool bigSize = false;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }
    
	public void OnClick(Vector2 position) {
		float size = bigSize ? 1 : 2;
		transform.DOScale(size, duration).SetEase(Ease.OutBounce).OnComplete(() => {bigSize = !bigSize;});
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
