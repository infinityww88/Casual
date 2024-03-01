using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.Assertions;
using QFSW.QC;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using System.Threading;
using Cysharp.Threading.Tasks;

using Random = UnityEngine.Random;

public static class Utils
{
	[Command("SetIp")]
	static string ip = "192.168.0.102";
	
	public static void Foreach<T>(this IEnumerable<T> array, Action<T> action) {
		foreach (var e in array) {
			action(e);
		}
	}
	
	public static void Foreach<T>(this IEnumerable<T> array, Action<int, T> action)
	{
		int i = 0;
		foreach (var e in array) {
			action(i, e);
			i++;
		}
	}
	
	public static bool GetWorldAreaByUI(Camera camera, VisualElement uiElement, Vector3 worldPos,
		out Vector3 lb, out Vector3 lt, out Vector3 rt, out Vector3 rb) {
		var rect = GetViewRectByUI(uiElement);
		return Utils.GetCameraZPlane(camera,
			rect,
			new Plane(-camera.transform.forward, worldPos),
			out lb,
			out lt,
			out rt,
			out rb);
	}
	
	public static Rect GetViewRectByUI(VisualElement uiElement) {
		var lb = uiElement.LocalToWorld(new Vector2(0, 0));
		var rt = uiElement.LocalToWorld(new Vector2(uiElement.resolvedStyle.width, uiElement.resolvedStyle.height));
		var l = lb.x; //uiElement.resolvedStyle.left;
		var t = Camera.main.pixelHeight - lb.y; //uiElement.resolvedStyle.top;
		var r = rt.x;//l + uiElement.resolvedStyle.width;
		var b = Camera.main.pixelHeight - rt.y;
		var vlb = Camera.main.ScreenToViewportPoint(new Vector3(l, b));
		var vrt = Camera.main.ScreenToViewportPoint(new Vector3(r, t));
		//Debug.Log($"=> {l} {t} {r} {b} {vlb} {vrt}");
		var rect = new Rect(vlb.x, vlb.y, vrt.x - vlb.x, vrt.y - vlb.y);
		return rect;
	}
	
	// return world left, right, top bottom
	public static Vector4 GetWorldAreaYByUI(Camera camera, VisualElement uiElement, float worldY) {
		var rect = GetViewRectByUI(uiElement);
		var intersect = Utils.GetCameraZPlane(camera,
			rect,
			new Plane(Vector3.up, Vector3.up * worldY),
			out Vector3 lb,
			out Vector3 lt,
			out Vector3 rt,
			out Vector3 rb);
		Assert.IsTrue(intersect);
		return new Vector4(lb.x, rt.x, rt.z, lb.z);
	}
	
	public static Vector3 GetCameraRayPos(Camera camera, Vector3 worldPos, Plane plane) {
		Ray ray = new Ray(worldPos, camera.transform.forward);
		float distance = 0;
		bool hit = plane.Raycast(ray, out distance);
		Assert.IsTrue(hit);
		return ray.GetPoint(distance);
	}
	
	public static bool GetCameraZPlane(Camera camera, Rect viewPort, Plane plane,
		out Vector3 leftBottom,
		out Vector3 leftTop,
		out Vector3 rightTop,
		out Vector3 rightBottom)
	{
		leftBottom = leftTop = rightTop = rightBottom = Vector3.zero;
		Ray ray = camera.ViewportPointToRay(new Vector2(viewPort.left, viewPort.top));
		float distance = 0;
		if (!plane.Raycast(ray, out distance)) {
			return false;
		}
		leftBottom = ray.GetPoint(distance);
		
		ray = camera.ViewportPointToRay(new Vector2(viewPort.left, viewPort.bottom));
		plane.Raycast(ray, out distance);
		leftTop = ray.GetPoint(distance);
		
		ray = camera.ViewportPointToRay(new Vector2(viewPort.right, viewPort.bottom));
		plane.Raycast(ray, out distance);
		rightTop = ray.GetPoint(distance);
		
		ray = camera.ViewportPointToRay(new Vector2(viewPort.right, viewPort.top));
		plane.Raycast(ray, out distance);
		rightBottom = ray.GetPoint(distance);
		
		return true;
	}
	
	public static T GetOrCreateComponent<T>(this GameObject o) where T : Component {
		T comp = null;
		if (!o.TryGetComponent(out comp)) {
			comp = o.AddComponent<T>();
		}
		return comp;
	}
	
	public static bool isNullOrEmpty<T>(IEnumerable<T> collection) {
		return collection == null || collection.Count() == 0;
	}
	
	public static T RandomElement<T>(List<T> array) {
		if (isNullOrEmpty(array)) return default(T);
		int i = Random.Range(0, array.Count);
		return array[i];
	}
	
	public static T RandomElement<T>(T[] array) {
		if (isNullOrEmpty(array)) return default(T);
		int i = Random.Range(0, array.Length);
		return array[i];
	}
	
	public static void For(int num, Action<int> action) {
		for (int i = 0; i < num; i++) {
			action(i);
		}
	}
	
	public static void ForeachChild(Transform parent, Action<Transform> action) {
		for (int i = 0; i < parent.childCount; i++) {
			action(parent.GetChild(i));
		}
	}
	
	public static YieldInstruction TweenTransform(Transform src, Transform dest, float duration, float lerpFactor) {
		return DOTween.Sequence().AppendCallback(() => {
			src.position = Vector3.Lerp(src.position, dest.position, lerpFactor);
			src.rotation = Quaternion.Lerp(src.rotation, dest.rotation, lerpFactor);
			src.localScale = Vector3.Lerp(src.localScale, dest.lossyScale, lerpFactor);
		})
			.AppendInterval(0.01f)
			.SetLoops(50)
			.WaitForCompletion();
	}
	
	public static void CaptureCamera(Camera camera, RenderTexture rt, string path) {
		var t = RenderTexture.active;
		RenderTexture.active = rt;
		camera.targetTexture = rt;
		camera.Render();
		Texture2D tex = new Texture2D(rt.width, rt.height);
		tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
		tex.Apply();
		byte[] data = ImageConversion.EncodeToPNG(tex);
		File.WriteAllBytes(path, data);
		camera.targetTexture = null;
		RenderTexture.active = t;
	}
	
	public static async UniTask<string> DownloadText(string fileName) {
		UnityWebRequest req = UnityWebRequest.Get($"http://{ip}:7777/{fileName}");
		await req.SendWebRequest();
		if (req.result != UnityWebRequest.Result.Success) {
			throw new Exception($"download failed {req.error} {req.result}");
		}
		return req.downloadHandler.text;
		//Debug.Log($"Download {req.downloadHandler.text}");
	}
}
