using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PreviewTexture : MonoBehaviour
{
	[SerializeField]
	private Transform pivot;
	
	[Button]
	public void Front() {
		pivot.localScale = new Vector3(1, 1, 1);
	}
	
	[Button]
	public void Back() {
		pivot.localScale = new Vector3(-1, 1, 1);
	}
}
