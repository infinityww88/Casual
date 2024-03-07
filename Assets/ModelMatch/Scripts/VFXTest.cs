using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class VFXTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
	    
    }
    
	[Button]
	void Play() {
		GetComponent<ParticleSystem>().Play();
		transform.DOLocalMoveX(10, 1f).SetEase(Ease.Linear)
			.OnComplete(() => {
				GetComponent<ParticleSystem>().Stop();
			});
	}

	[Button]
	void Stop() {
		//GetComponent<ParticleSystem>().Stop();
	}
}
