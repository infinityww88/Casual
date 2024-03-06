using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PreviewTexture : MonoBehaviour
{
	[SerializeField]
	private Transform pivot;
	[SerializeField]
	private Light light;
	[SerializeField]
	private Transform attachPoint;
	
	[Button]
	public void Front() {
		pivot.localScale = new Vector3(1, 1, 1);
	}
	
	[Button]
	public void Back() {
		pivot.localScale = new Vector3(-1, 1, 1);
	}
}
