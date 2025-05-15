// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// Opsive.Shared.Utility.TypeUtility

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace InatesiCharacter.Shared.Utility
{
	public static class TypeUtility
	{
		public const int c_StartArraySize = 10;

		private static Dictionary<string, Type> s_TypeLookup = new Dictionary<string, Type>();

		private static List<Assembly> s_LoadedAssemblies = null;

		private static Dictionary<MemberInfo, Dictionary<Type, Attribute[]>> s_AttributesByType;

		public static void ResizeIfNecessary<T>(ref T[] array, int length)
		{
			if (array == null || array.Length == 0)
			{
				array = new T[Mathf.Max(10, length + 10)];
			}
			else if (array.Length <= length)
			{
				Array.Resize(ref array, length * 2);
			}
		}

		public static void ResizeIfNecessary<T>(this T[] arrayObject, ref T[] array, int length)
		{
			if (array == null || array.Length == 0)
			{
				array = new T[Mathf.Max(10, length + 10)];
			}
			else if (array.Length <= length)
			{
				Array.Resize(ref array, length * 2);
			}
		}

		public static void ResizeIfNecessary<T>(ref IList<T> list, int length)
		{
			object obj = list as T[];
			T[] array = default(T[]);
			if (obj != null)
			{
				array = (T[])obj;
				obj = array;
			}
			if (obj != null)
			{
				ResizeIfNecessary(ref array, length);
				return;
			}
			if (list == null || list.Count == 0)
			{
				list = new List<T>();
			}
			if (list.Count <= length)
			{
				for (int i = list.Count; i < length + 1; i++)
				{
					list.Add(default(T));
				}
			}
		}

		public static Type GetType(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			if (s_TypeLookup.TryGetValue(name, out var value))
			{
				return value;
			}
			value = Type.GetType(name);
			if (value == null)
			{
				if (s_LoadedAssemblies == null || s_LoadedAssemblies.Count == 0)
				{
					s_LoadedAssemblies = new List<Assembly>();
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
					for (int i = 0; i < assemblies.Length; i++)
					{
						s_LoadedAssemblies.Add(assemblies[i]);
					}
				}
				for (int j = 0; j < s_LoadedAssemblies.Count; j++)
				{
					value = s_LoadedAssemblies[j].GetType(name);
					if (value != null)
					{
						break;
					}
				}
				if (value == null && name.Contains("Opsive.UltimateCharacterController.Input"))
				{
					return GetType(name.Replace("Opsive.UltimateCharacterController.Input", "Opsive.Shared.Input"));
				}
			}
			if (value != null)
			{
				s_TypeLookup.Add(name, value);
			}
			return value;
		}

		public static Attribute GetAttribute(MemberInfo info, Type attributeType, int index = 0)
		{
			if (info == null)
			{
				return null;
			}
			if (s_AttributesByType == null)
			{
				s_AttributesByType = new Dictionary<MemberInfo, Dictionary<Type, Attribute[]>>();
			}
			if (!s_AttributesByType.TryGetValue(info, out var value))
			{
				value = new Dictionary<Type, Attribute[]>();
				s_AttributesByType.Add(info, value);
			}
			if (!value.TryGetValue(attributeType, out var value2))
			{
				value2 = info.GetCustomAttributes(attributeType, inherit: false) as Attribute[];
				value.Add(attributeType, value2);
			}
			if (value2 == null || value2.Length <= index)
			{
				return null;
			}
			return value2[index];
		}

		public static void ClearCache()
		{
			s_TypeLookup.Clear();
			if (s_LoadedAssemblies != null)
			{
				s_LoadedAssemblies.Clear();
			}
			if (s_AttributesByType != null)
			{
				s_AttributesByType.Clear();
			}
		}

		public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
		{
			while (toCheck != typeof(object))
			{
				Type type = ((!toCheck.IsGenericType) ? toCheck : toCheck.GetGenericTypeDefinition());
				if (baseType == type)
				{
					return true;
				}
				toCheck = toCheck.BaseType;
			}
			return false;
		}
	}


}