using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Sirenix.OdinInspector;
using System;
using UnityEngine.UIElements;
using QFSW.QC;

namespace ModelMatch {
	
	public class WallController : MonoBehaviour
	{
		[BoxGroup("Wall")]
		public Transform leftWall;
		[BoxGroup("Wall")]
		public Transform topWall;
		[BoxGroup("Wall")]
		public Transform rightWall;
		[BoxGroup("Wall")]
		public Transform bottomWall;
		
		[BoxGroup("Anchor")]
		public Transform leftAnchor;
		[BoxGroup("Anchor")]
		public Transform topAnchor;
		[BoxGroup("Anchor")]
		public Transform rightAnchor;
		[BoxGroup("Anchor")]
		public Transform bottomAnchor;
		
		private void AlignWall(Transform wall, Vector3 pos) {
			pos.y = wall.position.y;
			wall.position = pos;
		}
		
		[Button]
		public void AlignWalls() {
			var area = GetPlayAreaAnchor();
			AlignWall(leftWall, area.Item1);
			AlignWall(rightWall, area.Item2);
			AlignWall(topWall, area.Item3);
			AlignWall(bottomWall, area.Item4);
		}
		
		
		ValueTuple<Vector3, Vector3, Vector3, Vector3> GetPlayAreaAnchor() {
			Plane plane = new Plane(Vector3.up, Vector3.zero);
			
			var left = Utils.GetCameraRayPos(Camera.main, leftAnchor.position, plane);
			var right = Utils.GetCameraRayPos(Camera.main, rightAnchor.position, plane);
			var top = Utils.GetCameraRayPos(Camera.main, topAnchor.position, plane);
			var bottom = Utils.GetCameraRayPos(Camera.main, bottomAnchor.position, plane);
			
			return ValueTuple.Create(left, right, top, bottom);
		}
		
		public Vector4 GetPlayArea() {
			var area = GetPlayAreaAnchor();
			return new Vector4(area.Item1.x,
				area.Item2.x,
				area.Item3.z,
				area.Item4.z);
		}
	}

}
