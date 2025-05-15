using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InatesiCharacter.Shared.Game
{
    public static class GameObjectExtensions
    {
		private static Dictionary<GameObject, Dictionary<Type, object>> s_GameObjectComponentMap = new Dictionary<GameObject, Dictionary<Type, object>>();

		private static Dictionary<GameObject, Dictionary<Type, object>> s_GameObjectParentComponentMap = new Dictionary<GameObject, Dictionary<Type, object>>();

		private static Dictionary<GameObject, Dictionary<Type, object>> s_GameObjectInactiveParentComponentMap = new Dictionary<GameObject, Dictionary<Type, object>>();

		private static Dictionary<GameObject, Dictionary<Type, object[]>> s_GameObjectComponentsMap = new Dictionary<GameObject, Dictionary<Type, object[]>>();

		private static Dictionary<GameObject, Dictionary<Type, object[]>> s_GameObjectParentComponentsMap = new Dictionary<GameObject, Dictionary<Type, object[]>>();

		public static T GetCachedComponent<T>(this GameObject gameObject)
		{
			if (s_GameObjectComponentMap.TryGetValue(gameObject, out var value))
			{
				if (value.TryGetValue(typeof(T), out var value2))
				{
					return (T)value2;
				}
			}
			else
			{
				value = new Dictionary<Type, object>();
				s_GameObjectComponentMap.Add(gameObject, value);
			}
			T component = gameObject.GetComponent<T>();
			value.Add(typeof(T), component);
			return component;
		}

		public static T GetCachedParentComponent<T>(this GameObject gameObject)
		{
			if (s_GameObjectParentComponentMap.TryGetValue(gameObject, out var value))
			{
				if (value.TryGetValue(typeof(T), out var value2))
				{
					return (T)value2;
				}
			}
			else
			{
				value = new Dictionary<Type, object>();
				s_GameObjectParentComponentMap.Add(gameObject, value);
			}
			T componentInParent = gameObject.GetComponentInParent<T>();
			value.Add(typeof(T), componentInParent);
			return componentInParent;
		}

		public static T[] GetCachedComponents<T>(this GameObject gameObject)
		{
			if (s_GameObjectComponentsMap.TryGetValue(gameObject, out var value))
			{
				if (value.TryGetValue(typeof(T), out var value2))
				{
					return value2 as T[];
				}
			}
			else
			{
				value = new Dictionary<Type, object[]>();
				s_GameObjectComponentsMap.Add(gameObject, value);
			}
			T[] components = gameObject.GetComponents<T>();
			value.Add(typeof(T), components as object[]);
			return components;
		}

		public static T[] GetCachedParentComponents<T>(this GameObject gameObject)
		{
			if (s_GameObjectParentComponentsMap.TryGetValue(gameObject, out var value))
			{
				if (value.TryGetValue(typeof(T), out var value2))
				{
					return value2 as T[];
				}
			}
			else
			{
				value = new Dictionary<Type, object[]>();
				s_GameObjectParentComponentsMap.Add(gameObject, value);
			}
			T[] componentsInParent = gameObject.GetComponentsInParent<T>();
			value.Add(typeof(T), componentsInParent as object[]);
			return componentsInParent;
		}

		public static T GetCachedInactiveComponentInParent<T>(this GameObject gameObject) where T : Component
		{
			if (s_GameObjectInactiveParentComponentMap.TryGetValue(gameObject, out var value))
			{
				if (value.TryGetValue(typeof(T), out var value2))
				{
					return (T)value2;
				}
			}
			else
			{
				value = new Dictionary<Type, object>();
				s_GameObjectInactiveParentComponentMap.Add(gameObject, value);
			}
			T val = (T)(object)null;
			Transform val2 = gameObject.transform;
			while ((object)val2 != null && !((object)(val = val2.GetComponent<T>()) != null))
			{
				val2 = val2.parent;
			}
			value.Add(typeof(T), val);
			return val;
		}

		[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		public static void DomainReset()
		{
			if (s_GameObjectComponentMap != null)
			{
				s_GameObjectComponentMap.Clear();
			}
			if (s_GameObjectParentComponentMap != null)
			{
				s_GameObjectParentComponentMap.Clear();
			}
			if (s_GameObjectInactiveParentComponentMap != null)
			{
				s_GameObjectInactiveParentComponentMap.Clear();
			}
			if (s_GameObjectComponentsMap != null)
			{
				s_GameObjectComponentsMap.Clear();
			}
			if (s_GameObjectParentComponentsMap != null)
			{
				s_GameObjectParentComponentsMap.Clear();
			}
		}
	}
}