using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public static class MyEditorUtils
{
	public static void UpdateProperties<T>(T o, params ValueTuple<string, object>[] properties) where T : UnityEngine.Object {
		SerializedObject so = new SerializedObject(o);
		properties.Foreach(p => {
			var sp = so.FindProperty(p.Item1);
			if (p.Item2 is int) {
				sp.intValue = (int)p.Item2;
			}
			else if (p.Item2 is float) {
				sp.floatValue = (float)p.Item2;
			}
		});
		so.ApplyModifiedProperties();
	}
	
	public static void UpdateIntProperty<T>(T o, string propertyName, int value)
	where T : UnityEngine.Object {
		UpdateProperties(o, ValueTuple.Create(propertyName, (object)value));
	}
	
	public static void UpdateFloatProperty<T>(T o, string propertyName, float value)
	where T : UnityEngine.Object {
		UpdateProperties(o, ValueTuple.Create(propertyName, (object)value));
	}
}
