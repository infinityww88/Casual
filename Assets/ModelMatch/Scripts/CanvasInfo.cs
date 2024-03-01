using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CanvasInfo : MonoBehaviour
{
	public RectTransform rt;
    
	[Button]
	void Info() {
		
	}
	
	// Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn.
	protected void OnDrawGizmos()
	{
		if (rt == null) {
			return;
		}
		Vector3[] corners = new Vector3[4];
		rt.GetWorldCorners(corners);
		corners.Foreach(p => {
			Gizmos.DrawSphere(p, 0.2f);
		});
	}

    // Update is called once per frame
    void Update()
    {
    }
}
