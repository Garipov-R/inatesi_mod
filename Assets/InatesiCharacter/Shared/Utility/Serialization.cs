using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

namespace InatesiCharacter.Shared.Utility
{
    [Serializable]
    public class Serialization
	{
		private const string c_Version = "3.1";

		[Tooltip("The class type that the values represent.")]
		[SerializeField]
		protected string m_ObjectType;

		[Tooltip("A list of all value hashes that the serialization applies to.")]
		[SerializeField]
		protected int[] m_ValueHashes;

		[Tooltip("Maps the value hash to a position within the value array.")]
		[SerializeField]
		protected int[] m_ValuePositions;

		[Tooltip("An array of saved values.")]
		[SerializeField]
		protected byte[] m_Values;

		[Tooltip("Unity objects are serialized by Unity so store a reference to those objects.")]
		[SerializeField]
		private Object[] m_UnityObjects;

		[Tooltip("The version used for serialization.")]
		[SerializeField]
		protected string m_Version;

		private static Dictionary<Type, FieldInfo[]>[] s_SerializedFieldsMaps;

		private static Dictionary<Type, PropertyInfo[]>[] s_SerializedPropertiesMaps;

		private static Dictionary<string, int> s_StringHashMap;

		public static int HashMultiplier => 397;

		public string ObjectType
		{
			get
			{
				return m_ObjectType;
			}
			set
			{
				m_ObjectType = value;
			}
		}

		public int[] ValueHashes
		{
			get
			{
				return m_ValueHashes;
			}
			set
			{
				m_ValueHashes = value;
			}
		}

		public byte[] Values
		{
			get
			{
				return m_Values;
			}
			set
			{
				m_Values = value;
			}
		}

		public int[] ValuePositions
		{
			get
			{
				return m_ValuePositions;
			}
			set
			{
				m_ValuePositions = value;
			}
		}

		public Object[] UnityObjects
		{
			get
			{
				return m_UnityObjects;
			}
			set
			{
				m_UnityObjects = value;
			}
		}

		public string Version
		{
			get
			{
				return m_Version;
			}
			internal set
			{
				m_Version = value;
			}
		}

		public void Serialize(object obj, bool useFields, MemberVisibility visibility)
		{
			if (obj != null)
			{
				Type type = obj.GetType();
				if (type.IsGenericType)
				{
					m_ObjectType = $"{type.GetGenericTypeDefinition().FullName}[[{type.GetGenericArguments()[0].AssemblyQualifiedName}]]";
				}
				else
				{
					m_ObjectType = type.ToString();
				}
				m_Version = "3.1";
				if (visibility == MemberVisibility.None || visibility == MemberVisibility.Last)
				{
					m_ValueHashes = new int[0];
					m_ValuePositions = new int[0];
					m_Values = new byte[0];
					m_UnityObjects = (Object[])(object)new Object[0];
				}
				else if (useFields)
				{
					List<int> valueHashes = new List<int>();
					List<int> valuePositions = new List<int>();
					List<byte> values = new List<byte>();
					SerializeFields(obj, 0, ref valueHashes, ref valuePositions, ref values, ref m_UnityObjects, visibility);
					m_ValueHashes = valueHashes.ToArray();
					m_ValuePositions = valuePositions.ToArray();
					m_Values = values.ToArray();
				}
				else
				{
					List<int> valueHashes2 = new List<int>();
					List<int> valuePositions2 = new List<int>();
					List<byte> values2 = new List<byte>();
					SerializeProperties(obj, 0, ref valueHashes2, ref valuePositions2, ref values2, ref m_UnityObjects, visibility);
					m_ValueHashes = valueHashes2.ToArray();
					m_ValuePositions = valuePositions2.ToArray();
					m_Values = values2.ToArray();
				}
			}
		}

		public static void SerializeFields(object obj, int hashPrefix, ref List<int> valueHashes, ref List<int> valuePositions, ref List<byte> values, ref Object[] unityObjects, MemberVisibility visibility)
		{
			if (obj != null)
			{
				FieldInfo[] serializedFields = GetSerializedFields(obj.GetType(), visibility);
				for (int i = 0; i < serializedFields.Length; i++)
				{
					Serializer.SerializeValue(serializedFields[i].FieldType, serializedFields[i].GetValue(obj), ref valueHashes, ref valuePositions, ref values, ref unityObjects, hashPrefix, serializedFields[i].Name, serializeFields: true, visibility);
				}
			}
		}

		public static void SerializeProperties(object obj, int hashPrefix, ref List<int> valueHashes, ref List<int> valuePositions, ref List<byte> values, ref Object[] unityObjects, MemberVisibility visibility)
		{
			if (obj == null)
			{
				return;
			}
			PropertyInfo[] serializedProperties = GetSerializedProperties(obj.GetType(), visibility);
			for (int i = 0; i < serializedProperties.Length; i++)
			{
				MethodInfo validGetMethod = GetValidGetMethod(serializedProperties[i], visibility);
				if (validGetMethod != null)
				{
					Serializer.SerializeValue(serializedProperties[i].PropertyType, validGetMethod.Invoke(obj, null), ref valueHashes, ref valuePositions, ref values, ref unityObjects, hashPrefix, serializedProperties[i].Name, serializeFields: false, visibility);
				}
			}
		}

		public static MethodInfo GetValidGetMethod(PropertyInfo property, MemberVisibility visibility)
		{
			MethodInfo getMethod = property.GetGetMethod(visibility != MemberVisibility.Public && visibility != MemberVisibility.AllPublic);
			if (getMethod == null)
			{
				return null;
			}
			MethodInfo setMethod = property.GetSetMethod(visibility != MemberVisibility.Public && visibility != MemberVisibility.AllPublic);
			if (setMethod == null)
			{
				return null;
			}
			Type type = property.PropertyType;
			if (typeof(IList).IsAssignableFrom(type))
			{
				if (type.IsArray)
				{
					type = type.GetElementType();
				}
				else
				{
					while (!type.IsGenericType)
					{
						type = type.BaseType;
					}
					type = type.GetGenericArguments()[0];
				}
			}
			if (type == typeof(Serialization))
			{
				return null;
			}
			if (typeof(UnityEventBase).IsAssignableFrom(type))
			{
				return null;
			}
			if (visibility != MemberVisibility.Snapshot && (type.IsAbstract || typeof(Delegate).IsAssignableFrom(type)))
			{
				return null;
			}
			return getMethod;
		}

		public object DeserializeFields(MemberVisibility visibility, Func<Type, object, object> onValidateCallback = null)
		{
			Type type = TypeUtility.GetType(m_ObjectType);
			if (type == null || type.IsAbstract)
			{
				return null;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (m_ValueHashes != null)
			{
				for (int i = 0; i < m_ValueHashes.Length; i++)
				{
					dictionary.Add(m_ValueHashes[i], i);
				}
			}
			object obj = Activator.CreateInstance(type, nonPublic: true);
			bool bitwiseHash = new Version(m_Version).CompareTo(new Version("3.1")) >= 0;
			DeserializeFields(obj, 0, dictionary, m_ValuePositions, m_Values, m_UnityObjects, visibility, bitwiseHash, onValidateCallback);
			return obj;
		}

		public static object DeserializeFields(object obj, int hashPrefix, Dictionary<int, int> valuePositionMap, int[] valuePositions, byte[] values, Object[] unityObjects, MemberVisibility visibility, bool bitwiseHash, Func<Type, object, object> onValidateCallback)
		{
			FieldInfo[] serializedFields = GetSerializedFields(obj.GetType(), visibility);
			for (int i = 0; i < serializedFields.Length; i++)
			{
				object obj2 = Serializer.BytesToValue(serializedFields[i].FieldType, serializedFields[i].Name, valuePositionMap, hashPrefix, values, valuePositions, unityObjects, useFields: true, visibility, bitwiseHash, onValidateCallback);
				if (onValidateCallback != null)
				{
					obj2 = onValidateCallback(serializedFields[i].FieldType, obj2);
				}
				if (obj2 != null && !obj2.Equals(null) && (serializedFields[i].FieldType.IsValueType || serializedFields[i].FieldType.IsAssignableFrom(obj2.GetType())))
				{
					serializedFields[i].SetValue(obj, obj2);
				}
			}
			return obj;
		}

		public static object DeserializeProperties(object obj, int hashPrefix, Dictionary<int, int> valuePositionMap, int[] valuePositions, byte[] values, Object[] unityObjects, MemberVisibility visibility, bool bitwiseHash, Func<Type, object, object> onValidateCallback)
		{
			PropertyInfo[] serializedProperties = GetSerializedProperties(obj.GetType(), visibility);
			for (int i = 0; i < serializedProperties.Length; i++)
			{
				object obj2 = Serializer.BytesToValue(serializedProperties[i].PropertyType, serializedProperties[i].Name, valuePositionMap, hashPrefix, values, valuePositions, unityObjects, useFields: false, visibility, bitwiseHash, onValidateCallback);
				if (onValidateCallback != null)
				{
					obj2 = onValidateCallback(serializedProperties[i].PropertyType, obj2);
				}
				if (obj2 != null)
				{
					MethodInfo setMethod = serializedProperties[i].GetSetMethod(visibility != MemberVisibility.Public && visibility != MemberVisibility.AllPublic);
					if (setMethod != null)
					{
						setMethod.Invoke(obj, new object[1] { obj2 });
					}
				}
			}
			return obj;
		}

		public static FieldInfo[] GetSerializedFields(Type type, MemberVisibility visibility)
		{
			if (s_SerializedFieldsMaps == null)
			{
				s_SerializedFieldsMaps = new Dictionary<Type, FieldInfo[]>[5];
			}
			if (s_SerializedFieldsMaps[(int)visibility] == null)
			{
				s_SerializedFieldsMaps[(int)visibility] = new Dictionary<Type, FieldInfo[]>();
			}
			if (!s_SerializedFieldsMaps[(int)visibility].TryGetValue(type, out var value))
			{
				List<FieldInfo> fieldList = new List<FieldInfo>();
				BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				GetSerializedFields(type, ref fieldList, flags, visibility);
				value = fieldList.ToArray();
				s_SerializedFieldsMaps[(int)visibility].Add(type, value);
			}
			return value;
		}

		private static void GetSerializedFields(Type type, ref List<FieldInfo> fieldList, BindingFlags flags, MemberVisibility visibility)
		{
			if (type == null)
			{
				return;
			}
			GetSerializedFields(type.BaseType, ref fieldList, flags, visibility);
			FieldInfo[] fields = type.GetFields(flags);
			for (int i = 0; i < fields.Length; i++)
			{
				if ((visibility == MemberVisibility.AllPublic || TypeUtility.GetAttribute(fields[i], typeof(NonSerializedAttribute)) == null) && (visibility != MemberVisibility.Public || (!fields[i].IsPrivate && !fields[i].IsFamily) || TypeUtility.GetAttribute(fields[i], typeof(SerializeField)) != null) && (visibility != MemberVisibility.Snapshot || TypeUtility.GetAttribute(fields[i], typeof(Snapshot)) != null) && (TypeUtility.GetAttribute(fields[i], typeof(ForceSerialized)) != null || (!(fields[i].FieldType == typeof(Serialization)) && !(fields[i].FieldType == typeof(Serialization[])))))
				{
					fieldList.Add(fields[i]);
				}
			}
		}

		public static PropertyInfo[] GetSerializedProperties(Type type, MemberVisibility visibility)
		{
			if (s_SerializedPropertiesMaps == null)
			{
				s_SerializedPropertiesMaps = new Dictionary<Type, PropertyInfo[]>[5];
			}
			if (s_SerializedPropertiesMaps[(int)visibility] == null)
			{
				s_SerializedPropertiesMaps[(int)visibility] = new Dictionary<Type, PropertyInfo[]>();
			}
			if (!s_SerializedPropertiesMaps[(int)visibility].TryGetValue(type, out var value))
			{
				BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;
				if (visibility != MemberVisibility.Public && visibility != MemberVisibility.AllPublic)
				{
					bindingFlags |= BindingFlags.NonPublic;
				}
				List<PropertyInfo> list = new List<PropertyInfo>();
				value = type.GetProperties(bindingFlags);
				for (int i = 0; i < value.Length; i++)
				{
					if ((visibility == MemberVisibility.Snapshot || visibility == MemberVisibility.AllPublic || (TypeUtility.GetAttribute(value[i], typeof(NonSerialized)) == null && TypeUtility.GetAttribute(value[i], typeof(Snapshot)) == null)) && !(value[i].DeclaringType == typeof(MonoBehaviour)) && !(value[i].DeclaringType == typeof(Behaviour)) && !(value[i].DeclaringType == typeof(Component)) && !(value[i].DeclaringType == typeof(Object)) && !typeof(UnityEventBase).IsAssignableFrom(value[i].PropertyType) && (visibility != MemberVisibility.Snapshot || TypeUtility.GetAttribute(value[i], typeof(Snapshot)) != null))
					{
						list.Add(value[i]);
					}
				}
				value = list.ToArray();
				s_SerializedPropertiesMaps[(int)visibility].Add(type, value);
			}
			return value;
		}

		public static void AddByteValue(ICollection<byte> bytes, int hash, ref List<int> valueHashes, ref List<int> valuePositions, ref List<byte> values)
		{
			valueHashes.Add(hash);
			valuePositions.Add(values.Count);
			if (bytes != null)
			{
				values.AddRange(bytes);
			}
		}

		public static int GetValueSize(int index, byte[] values, int[] valuePositions)
		{
			return (index != valuePositions.Length - 1) ? (valuePositions[index + 1] - valuePositions[index]) : (values.Length - valuePositions[index]);
		}

		public static void GetUnityObjectIndexes(ref List<int> unityObjectIndexes, Type type, string name, int hashPrefix, Dictionary<int, int> valuePositionMap, int[] valueHashes, int[] valuePositions, byte[] values, bool useFields, MemberVisibility visibility, bool bitwiseHash)
		{
			int position;
			int valuePosition = GetValuePosition(bitwiseHash, hashPrefix, type, name, valuePositionMap, out position);
			if (valuePosition == 0)
			{
				return;
			}
			if (typeof(Object).IsAssignableFrom(type))
			{
				int num = Serializer.BytesToInt(values, valuePositions[position]);
				if (num != -1)
				{
					unityObjectIndexes.Add(num);
				}
			}
			else if (typeof(IList).IsAssignableFrom(type))
			{
				int num2 = Serializer.BytesToInt(values, valuePositions[position]);
				Type type2;
				if (type.IsArray)
				{
					type2 = type.GetElementType();
				}
				else
				{
					Type type3 = type;
					while (!type3.IsGenericType)
					{
						type3 = type3.BaseType;
					}
					type2 = type3.GetGenericArguments()[0];
				}
				for (int i = 0; i < num2; i++)
				{
					int hashPrefix2 = ((!bitwiseHash) ? (valuePosition / (i + 2)) : (valuePosition + i + 1));
					GetUnityObjectIndexes(ref unityObjectIndexes, type2, name, hashPrefix2, valuePositionMap, valueHashes, valuePositions, values, useFields, visibility, bitwiseHash);
				}
			}
			else
			{
				if (Serializer.IsSerializedType(type) || (!type.IsClass && (!type.IsValueType || type.IsPrimitive)))
				{
					return;
				}
				if (useFields)
				{
					FieldInfo[] serializedFields = GetSerializedFields(type, visibility);
					for (int j = 0; j < serializedFields.Length; j++)
					{
						GetUnityObjectIndexes(ref unityObjectIndexes, serializedFields[j].FieldType, serializedFields[j].Name, valuePosition, valuePositionMap, valueHashes, valuePositions, values, useFields, visibility, bitwiseHash);
					}
					return;
				}
				PropertyInfo[] serializedProperties = GetSerializedProperties(type, visibility);
				for (int k = 0; k < serializedProperties.Length; k++)
				{
					MethodInfo validGetMethod = GetValidGetMethod(serializedProperties[k], visibility);
					if (validGetMethod != null)
					{
						GetUnityObjectIndexes(ref unityObjectIndexes, serializedProperties[k].PropertyType, serializedProperties[k].Name, valuePosition, valuePositionMap, valueHashes, valuePositions, values, useFields, visibility, bitwiseHash);
					}
				}
			}
		}

		public static int GetValuePosition(bool bitwiseHash, int hashPrefix, Type type, string name, Dictionary<int, int> valuePositionMap, out int position)
		{
			int num = ((!bitwiseHash) ? (hashPrefix + StringHash(type.FullName) + StringHash(name)) : ((hashPrefix * HashMultiplier) ^ (StringHash(type.FullName) + StringHash(name))));
			if (!valuePositionMap.TryGetValue(num, out position))
			{
				FormerlySerializedAs formerlySerializedAs = TypeUtility.GetAttribute((!(type.GetElementType() != null)) ? type : type.GetElementType(), typeof(FormerlySerializedAs)) as FormerlySerializedAs;
				if (formerlySerializedAs == null)
				{
					return 0;
				}
				string value = formerlySerializedAs.FullName + ((!type.IsArray) ? string.Empty : "[]");
				num = ((!bitwiseHash) ? (hashPrefix + StringHash(value) + StringHash(name)) : ((hashPrefix * HashMultiplier) ^ (StringHash(value) + StringHash(name))));
				if (!valuePositionMap.TryGetValue(num, out position))
				{
					return 0;
				}
			}
			return num;
		}

		public static void UpdateUnityObjectIndexes(int indexDiff, int startPosition, Type type, string name, int hashPrefix, Dictionary<int, int> valuePositionMap, int[] valueHashes, int[] valuePositions, ref byte[] values, bool useFields, MemberVisibility visibility, bool bitwiseHash)
		{
			int position;
			int valuePosition = GetValuePosition(bitwiseHash, hashPrefix, type, name, valuePositionMap, out position);
			if (valuePosition == 0 || position <= startPosition)
			{
				return;
			}
			if (typeof(Object).IsAssignableFrom(type))
			{
				int num = Serializer.BytesToInt(values, valuePositions[position]);
				if (num != -1)
				{
					byte[] array = Serializer.IntToBytes(num + indexDiff);
					for (int i = 0; i < array.Length; i++)
					{
						values[valuePositions[position] + i] = array[i];
					}
				}
			}
			else if (typeof(IList).IsAssignableFrom(type))
			{
				int num2 = Serializer.BytesToInt(values, valuePositions[position]);
				Type type2;
				if (type.IsArray)
				{
					type2 = type.GetElementType();
				}
				else
				{
					Type type3 = type;
					while (!type3.IsGenericType)
					{
						type3 = type3.BaseType;
					}
					type2 = type3.GetGenericArguments()[0];
				}
				for (int j = 0; j < num2; j++)
				{
					int hashPrefix2 = ((!bitwiseHash) ? (valuePosition / (j + 2)) : (valuePosition + j + 1));
					UpdateUnityObjectIndexes(indexDiff, startPosition, type2, name, hashPrefix2, valuePositionMap, valueHashes, valuePositions, ref values, useFields, visibility, bitwiseHash);
				}
			}
			else
			{
				if (Serializer.IsSerializedType(type) || (!type.IsClass && (!type.IsValueType || type.IsPrimitive)))
				{
					return;
				}
				if (useFields)
				{
					FieldInfo[] serializedFields = GetSerializedFields(type, visibility);
					for (int k = 0; k < serializedFields.Length; k++)
					{
						UpdateUnityObjectIndexes(indexDiff, startPosition, serializedFields[k].FieldType, serializedFields[k].Name, valuePosition, valuePositionMap, valueHashes, valuePositions, ref values, useFields, visibility, bitwiseHash);
					}
					return;
				}
				PropertyInfo[] serializedProperties = GetSerializedProperties(type, visibility);
				for (int l = 0; l < serializedProperties.Length; l++)
				{
					MethodInfo validGetMethod = GetValidGetMethod(serializedProperties[l], visibility);
					if (validGetMethod != null)
					{
						UpdateUnityObjectIndexes(indexDiff, startPosition, serializedProperties[l].PropertyType, serializedProperties[l].Name, valuePosition, valuePositionMap, valueHashes, valuePositions, ref values, useFields, visibility, bitwiseHash);
					}
				}
			}
		}

		public static void RemoveProperty(int index, List<int> unityObjectIndexes, Serialization serialization, MemberVisibility visibility, bool bitwiseHash)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>(serialization.ValueHashes.Length);
			for (int i = 0; i < serialization.ValueHashes.Length; i++)
			{
				dictionary.Add(serialization.ValueHashes[i], i);
			}
			PropertyInfo[] serializedProperties = GetSerializedProperties(TypeUtility.GetType(serialization.ObjectType), visibility);
			int key = StringHash(serializedProperties[index].PropertyType.FullName) + StringHash(serializedProperties[index].Name);
			if (!dictionary.TryGetValue(key, out var value))
			{
				return;
			}
			int num = 0;
			for (int j = 0; j < serializedProperties.Length; j++)
			{
				key = StringHash(serializedProperties[j].PropertyType.FullName) + StringHash(serializedProperties[j].Name);
				if (dictionary.TryGetValue(key, out var value2) && value2 > value && (num == 0 || value2 < num))
				{
					num = value2;
				}
			}
			if (unityObjectIndexes.Count > 0)
			{
				List<Object> list = new List<Object>(serialization.UnityObjects);
				for (int num2 = unityObjectIndexes.Count - 1; num2 >= 0; num2--)
				{
					list.RemoveAt(unityObjectIndexes[num2]);
				}
				serialization.UnityObjects = list.ToArray();
				byte[] values = serialization.Values;
				for (int k = 0; k < serializedProperties.Length; k++)
				{
					UpdateUnityObjectIndexes(-unityObjectIndexes.Count, value, serializedProperties[k].PropertyType, serializedProperties[k].Name, 0, dictionary, serialization.ValueHashes, serialization.ValuePositions, ref values, useFields: false, visibility, bitwiseHash);
				}
			}
			List<int> list2 = new List<int>(serialization.ValueHashes);
			List<byte> list3 = new List<byte>(serialization.Values);
			List<int> list4 = new List<int>(serialization.ValuePositions);
			int num3 = ((num <= value) ? (serialization.ValueHashes.Length - value) : (num - value));
			for (int num4 = num3 - 1; num4 >= 0; num4--)
			{
				int num5 = value + num4;
				list2.RemoveAt(num5);
				int valueSize = GetValueSize(num5, serialization.Values, serialization.ValuePositions);
				list3.RemoveRange(serialization.ValuePositions[num5], valueSize);
				for (int l = num5 + 1; l < list4.Count; l++)
				{
					list4[l] -= valueSize;
				}
				list4.RemoveAt(num5);
			}
			serialization.ValueHashes = list2.ToArray();
			serialization.Values = list3.ToArray();
			serialization.ValuePositions = list4.ToArray();
			serialization.Version = "3.1";
		}

		public static void AddProperty(PropertyInfo property, object value, List<int> unityObjectIndexes, Serialization serialization, MemberVisibility visibility)
		{
			List<int> valueHashes = new List<int>();
			List<int> valuePositions = new List<int>();
			List<byte> values = new List<byte>();
			Object[] unityObjects = (Object[])(object)new Object[(serialization.UnityObjects != null) ? serialization.UnityObjects.Length : 0];
			if (serialization.UnityObjects != null)
			{
				Array.Copy(serialization.UnityObjects, unityObjects, serialization.UnityObjects.Length);
			}
			Serializer.SerializeValue(property.PropertyType, value, ref valueHashes, ref valuePositions, ref values, ref unityObjects, 0, property.Name, serializeFields: false, visibility);
			if (serialization.ValuePositions.Length > 0)
			{
				for (int i = 0; i < valuePositions.Count; i++)
				{
					valuePositions[i] += serialization.Values.Length;
				}
			}
			List<int> list = new List<int>(serialization.ValueHashes);
			List<byte> list2 = new List<byte>(serialization.Values);
			List<int> list3 = new List<int>(serialization.ValuePositions);
			list.AddRange(valueHashes);
			list3.AddRange(valuePositions);
			list2.AddRange(values);
			serialization.ValueHashes = list.ToArray();
			serialization.ValuePositions = list3.ToArray();
			serialization.Values = list2.ToArray();
			serialization.UnityObjects = unityObjects;
			serialization.Version = "3.1";
		}

		public static int StringHash(string value)
		{
			if (s_StringHashMap == null)
			{
				s_StringHashMap = new Dictionary<string, int>();
			}
			if (!s_StringHashMap.TryGetValue(value, out var value2))
			{
				if (!string.IsNullOrEmpty(value))
				{
					value2 = 23;
					int length = value.Length;
					for (int i = 0; i < length; i++)
					{
						value2 = value2 * 31 + value[i];
					}
					return value2;
				}
				value2 = 0;
				s_StringHashMap.Add(value, value2);
			}
			return value2;
		}

		public static Serialization Serialize<T>(T obj)
		{
			if (obj == null)
			{
				return null;
			}
			Serialization serialization = new Serialization();
			serialization.Serialize(obj, useFields: true, MemberVisibility.Public);
			return serialization;
		}

		public static Serialization[] Serialize<T>(IList<T> list)
		{
			if (list == null)
			{
				return null;
			}
			List<Serialization> list2 = new List<Serialization>();
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null)
				{
					Serialization serialization = new Serialization();
					serialization.Serialize(list[i], useFields: true, MemberVisibility.Public);
					list2.Add(serialization);
				}
			}
			return list2.ToArray();
		}
	}
}

