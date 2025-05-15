using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InatesiCharacter.Shared.Utility
{
	public static class Serializer
	{
		private static byte[] s_BigEndianTwoByteArray;

		private static byte[] s_BigEndianFourByteArray;

		private static byte[] s_BigEndianEightByteArray;

		private static byte[] BigEndianTwoByteArray
		{
			get
			{
				if (s_BigEndianTwoByteArray == null)
				{
					s_BigEndianTwoByteArray = new byte[2];
				}
				return s_BigEndianTwoByteArray;
			}
			set
			{
				s_BigEndianTwoByteArray = value;
			}
		}

		private static byte[] BigEndianFourByteArray
		{
			get
			{
				if (s_BigEndianFourByteArray == null)
				{
					s_BigEndianFourByteArray = new byte[4];
				}
				return s_BigEndianFourByteArray;
			}
			set
			{
				s_BigEndianFourByteArray = value;
			}
		}

		private static byte[] BigEndianEightByteArray
		{
			get
			{
				if (s_BigEndianEightByteArray == null)
				{
					s_BigEndianEightByteArray = new byte[8];
				}
				return s_BigEndianEightByteArray;
			}
			set
			{
				s_BigEndianEightByteArray = value;
			}
		}

		public static bool IsSerializedType(Type type)
		{
			return type == typeof(int) || type.IsEnum || type == typeof(uint) || type == typeof(float) || type == typeof(double) || type == typeof(long) || type == typeof(bool) || type == typeof(string) || type == typeof(byte) || type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4) || type == typeof(Quaternion) || type == typeof(Color) || type == typeof(Rect) || type == typeof(Matrix4x4) || type == typeof(AnimationCurve) || type == typeof(LayerMask) || typeof(Object).IsAssignableFrom(type);
		}

		public static void SerializeValue(Type type, object value, ref List<int> valueHashes, ref List<int> valuePositions, ref List<byte> values, ref Object[] unityObjects, int hashPrefix, string name, bool serializeFields, MemberVisibility visibility)
		{
			//IL_0364: Unknown result type (might be due to invalid IL or missing references)
			//IL_0393: Unknown result type (might be due to invalid IL or missing references)
			//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0420: Unknown result type (might be due to invalid IL or missing references)
			//IL_044f: Unknown result type (might be due to invalid IL or missing references)
			//IL_047e: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_050b: Unknown result type (might be due to invalid IL or missing references)
			//IL_053a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0569: Unknown result type (might be due to invalid IL or missing references)
			//IL_0598: Unknown result type (might be due to invalid IL or missing references)
			//IL_05a2: Expected O, but got Unknown
			//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0629: Unknown result type (might be due to invalid IL or missing references)
			//IL_0635: Expected O, but got Unknown
			int num = (hashPrefix * Serialization.HashMultiplier) ^ (Serialization.StringHash(type.FullName) + Serialization.StringHash(name));
			if (typeof(IList).IsAssignableFrom(type))
			{
				IList list = value as IList;
				if (list == null)
				{
					Serialization.AddByteValue(IntToBytes(0), num, ref valueHashes, ref valuePositions, ref values);
					return;
				}
				Serialization.AddByteValue(IntToBytes(list.Count), num, ref valueHashes, ref valuePositions, ref values);
				if (list.Count <= 0)
				{
					return;
				}
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
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] == null)
					{
						Serialization.AddByteValue(IntToBytes(-1), num + i + 1, ref valueHashes, ref valuePositions, ref values);
					}
					else
					{
						SerializeValue(type2, list[i], ref valueHashes, ref valuePositions, ref values, ref unityObjects, num + i + 1, name, serializeFields, visibility);
					}
				}
			}
			else if (type == typeof(int))
			{
				Serialization.AddByteValue(IntToBytes((int)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(uint))
			{
				Serialization.AddByteValue(UIntToBytes((uint)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(float))
			{
				Serialization.AddByteValue(FloatToBytes((float)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(double))
			{
				Serialization.AddByteValue(DoubleToBytes((double)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(long))
			{
				Serialization.AddByteValue(LongToBytes((long)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(ulong))
			{
				Serialization.AddByteValue(ULongToBytes((ulong)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(short))
			{
				Serialization.AddByteValue(ShortToBytes((short)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(ushort))
			{
				Serialization.AddByteValue(UShortToBytes((ushort)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(bool))
			{
				Serialization.AddByteValue(BoolToBytes((bool)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(string))
			{
				Serialization.AddByteValue(StringToBytes((string)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(byte))
			{
				Serialization.AddByteValue(ByteToBytes((byte)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type.IsEnum)
			{
				Serialization.AddByteValue(null, num, ref valueHashes, ref valuePositions, ref values);
				SerializeValue(Enum.GetUnderlyingType(type), value, ref valueHashes, ref valuePositions, ref values, ref unityObjects, num, name, serializeFields, visibility);
			}
			else if (type == typeof(Vector2))
			{
				Serialization.AddByteValue(Vector2ToBytes((Vector2)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Vector2Int))
			{
				Serialization.AddByteValue(Vector2IntToBytes((Vector2Int)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Vector3))
			{
				Serialization.AddByteValue(Vector3ToBytes((Vector3)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Vector3Int))
			{
				Serialization.AddByteValue(Vector3IntToBytes((Vector3Int)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Vector4))
			{
				Serialization.AddByteValue(Vector4ToBytes((Vector4)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Quaternion))
			{
				Serialization.AddByteValue(QuaternionToBytes((Quaternion)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Color))
			{
				Serialization.AddByteValue(ColorToBytes((Color)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Rect))
			{
				Serialization.AddByteValue(RectToBytes((Rect)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(RectInt))
			{
				Serialization.AddByteValue(RectIntToBytes((RectInt)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Bounds))
			{
				Serialization.AddByteValue(BoundsToBytes((Bounds)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(BoundsInt))
			{
				Serialization.AddByteValue(BoundsIntToBytes((BoundsInt)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Matrix4x4))
			{
				Serialization.AddByteValue(Matrix4x4ToBytes((Matrix4x4)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(AnimationCurve))
			{
				Serialization.AddByteValue(AnimationCurveToBytes((AnimationCurve)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(LayerMask))
			{
				Serialization.AddByteValue(LayerMaskToBytes((LayerMask)value), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type == typeof(Guid))
			{
				Serialization.AddByteValue(((Guid)value).ToByteArray(), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (typeof(Object).IsAssignableFrom(type))
			{
				Serialization.AddByteValue(UnityObjectToBytes((Object)value, ref unityObjects), num, ref valueHashes, ref valuePositions, ref values);
			}
			else if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
			{
				if (type.IsAbstract && value != null)
				{
					Serialization.AddByteValue(StringToBytes(value.GetType().FullName), num, ref valueHashes, ref valuePositions, ref values);
				}
				else
				{
					Serialization.AddByteValue(null, num, ref valueHashes, ref valuePositions, ref values);
				}
				if (serializeFields)
				{
					Serialization.SerializeFields(value, num, ref valueHashes, ref valuePositions, ref values, ref unityObjects, visibility);
				}
				else
				{
					Serialization.SerializeProperties(value, num, ref valueHashes, ref valuePositions, ref values, ref unityObjects, visibility);
				}
			}
		}

		public static object BytesToValue(Type type, string name, Dictionary<int, int> valuePositionMap, int hashPrefix, byte[] values, int[] valuePositions, Object[] unityObjects, bool useFields, MemberVisibility visibility, bool bitwiseHash, Func<Type, object, object> onValidateCallback = null)
		{
			//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0422: Unknown result type (might be due to invalid IL or missing references)
			//IL_0448: Unknown result type (might be due to invalid IL or missing references)
			//IL_046e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0494: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0506: Unknown result type (might be due to invalid IL or missing references)
			//IL_052c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0552: Unknown result type (might be due to invalid IL or missing references)
			//IL_0578: Unknown result type (might be due to invalid IL or missing references)
			//IL_05bf: Unknown result type (might be due to invalid IL or missing references)
			int position;
			int valuePosition = Serialization.GetValuePosition(bitwiseHash, hashPrefix, type, name, valuePositionMap, out position);
			if (valuePosition == 0)
			{
				return null;
			}
			if (typeof(IList).IsAssignableFrom(type))
			{
				int num = BytesToInt(values, valuePositions[position]);
				//object obj = null;
				if (type.IsArray)
				{
					Type elementType = type.GetElementType();
					Array array = Array.CreateInstance(elementType, num);
					for (int i = 0; i < num; i++)
					{
						int hashPrefix2 = ((!bitwiseHash) ? (valuePosition / (i + 2)) : (valuePosition + i + 1));
						object obj2 = BytesToValue(elementType, name, valuePositionMap, hashPrefix2, values, valuePositions, unityObjects, useFields, visibility, bitwiseHash, onValidateCallback);
						if (obj2 == null || !elementType.IsClass || elementType.IsAssignableFrom(obj2.GetType()))
						{
							array.SetValue(obj2, i);
						}
					}
					return array;
				}
				Type type2 = type;
				while (!type2.IsGenericType)
				{
					type2 = type2.BaseType;
				}
				Type type3 = type2.GetGenericArguments()[0];
				IList list = ((!type.IsGenericType) ? (Activator.CreateInstance(type) as IList) : (Activator.CreateInstance(typeof(List<>).MakeGenericType(type3)) as IList));
				for (int j = 0; j < num; j++)
				{
					int hashPrefix3 = ((!bitwiseHash) ? (valuePosition / (j + 2)) : (valuePosition + j + 1));
					object obj3 = BytesToValue(type3, name, valuePositionMap, hashPrefix3, values, valuePositions, unityObjects, useFields, visibility, bitwiseHash, onValidateCallback);
					if (obj3 == null || !type3.IsClass || type3.IsAssignableFrom(obj3.GetType()))
					{
						list.Add(obj3);
					}
				}
				return list;
			}
			if (type == typeof(int))
			{
				return BytesToInt(values, valuePositions[position]);
			}
			if (type == typeof(uint))
			{
				return BytesToUInt(values, valuePositions[position]);
			}
			if (type == typeof(float))
			{
				return BytesToFloat(values, valuePositions[position]);
			}
			if (type == typeof(double))
			{
				return BytesToDouble(values, valuePositions[position]);
			}
			if (type == typeof(long))
			{
				return BytesToLong(values, valuePositions[position]);
			}
			if (type == typeof(ulong))
			{
				return BytesToULong(values, valuePositions[position]);
			}
			if (type == typeof(short))
			{
				return BytesToShort(values, valuePositions[position]);
			}
			if (type == typeof(ushort))
			{
				return BytesToUShort(values, valuePositions[position]);
			}
			if (type == typeof(bool))
			{
				return BytesToBool(values, valuePositions[position]);
			}
			if (type == typeof(string))
			{
				return BytesToString(values, valuePositions[position], Serialization.GetValueSize(position, values, valuePositions));
			}
			if (type == typeof(byte))
			{
				return BytesToByte(values, valuePositions[position]);
			}
			if (type.IsEnum)
			{
				object obj4 = BytesToValue(Enum.GetUnderlyingType(type), name, valuePositionMap, valuePosition, values, valuePositions, unityObjects, useFields, visibility, bitwiseHash, onValidateCallback);
				if (obj4 == null)
				{
					return Enum.ToObject(type, BytesToInt(values, valuePositions[position]));
				}
				return obj4;
			}
			if (type == typeof(Vector2))
			{
				return BytesToVector2(values, valuePositions[position]);
			}
			if (type == typeof(Vector2Int))
			{
				return BytesToVector2Int(values, valuePositions[position]);
			}
			if (type == typeof(Vector3))
			{
				return BytesToVector3(values, valuePositions[position]);
			}
			if (type == typeof(Vector3Int))
			{
				return BytesToVector3Int(values, valuePositions[position]);
			}
			if (type == typeof(Vector4))
			{
				return BytesToVector4(values, valuePositions[position]);
			}
			if (type == typeof(Quaternion))
			{
				return BytesToQuaternion(values, valuePositions[position]);
			}
			if (type == typeof(Color))
			{
				return BytesToColor(values, valuePositions[position]);
			}
			if (type == typeof(Rect))
			{
				return BytesToRect(values, valuePositions[position]);
			}
			if (type == typeof(RectInt))
			{
				return BytesToRectInt(values, valuePositions[position]);
			}
			if (type == typeof(Bounds))
			{
				return BytesToBounds(values, valuePositions[position]);
			}
			if (type == typeof(BoundsInt))
			{
				return BytesToBoundsInt(values, valuePositions[position]);
			}
			if (type == typeof(Matrix4x4))
			{
				return BytesToMatrix4x4(values, valuePositions[position]);
			}
			if (type == typeof(AnimationCurve))
			{
				return BytesToAnimationCurve(values, valuePositions[position]);
			}
			if (type == typeof(LayerMask))
			{
				return BytesToLayerMask(values, valuePositions[position]);
			}
			if (type == typeof(Guid))
			{
				return BytesToGuid(values, valuePositions[position]);
			}
			if (typeof(Object).IsAssignableFrom(type))
			{
				return BytesToUnityObject(values, valuePositions[position], unityObjects);
			}
			if (type.IsClass || (type.IsValueType && !type.IsPrimitive))
			{
				if (type.IsAbstract)
				{
					type = TypeUtility.GetType(BytesToString(values, valuePositions[position], Serialization.GetValueSize(position, values, valuePositions)));
					if (type == null)
					{
						return null;
					}
				}
				object obj5 = Activator.CreateInstance(type, nonPublic: true);
				if (useFields)
				{
					return Serialization.DeserializeFields(obj5, valuePosition, valuePositionMap, valuePositions, values, unityObjects, visibility, bitwiseHash, onValidateCallback);
				}
				return Serialization.DeserializeProperties(obj5, valuePosition, valuePositionMap, valuePositions, values, unityObjects, visibility, bitwiseHash, onValidateCallback);
			}
			return null;
		}

		public static byte[] IntToBytes(int value)
		{
			return BitConverter.GetBytes(value);
		}

		public static int BytesToInt(byte[] values, int valuePosition)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Copy(values, valuePosition, BigEndianFourByteArray, 0, 4);
				Array.Reverse(BigEndianFourByteArray);
				return BitConverter.ToInt32(BigEndianFourByteArray, 0);
			}
			return BitConverter.ToInt32(values, valuePosition);
		}

		public static byte[] UIntToBytes(uint value)
		{
			return BitConverter.GetBytes(value);
		}

		public static uint BytesToUInt(byte[] values, int valuePosition)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Copy(values, valuePosition, BigEndianFourByteArray, 0, 4);
				Array.Reverse(BigEndianFourByteArray);
				return BitConverter.ToUInt32(BigEndianFourByteArray, 0);
			}
			return BitConverter.ToUInt32(values, valuePosition);
		}

		public static byte[] FloatToBytes(float value)
		{
			return BitConverter.GetBytes(value);
		}

		public static float BytesToFloat(byte[] values, int valuePosition)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Copy(values, valuePosition, BigEndianFourByteArray, 0, 4);
				Array.Reverse(BigEndianFourByteArray);
				return BitConverter.ToSingle(BigEndianFourByteArray, 0);
			}
			return BitConverter.ToSingle(values, valuePosition);
		}

		public static byte[] DoubleToBytes(double value)
		{
			return BitConverter.GetBytes(value);
		}

		public static double BytesToDouble(byte[] values, int valuePosition)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Copy(values, valuePosition, BigEndianEightByteArray, 0, 8);
				Array.Reverse(BigEndianEightByteArray);
				return BitConverter.ToDouble(BigEndianEightByteArray, 0);
			}
			return BitConverter.ToDouble(values, valuePosition);
		}

		public static byte[] LongToBytes(long value)
		{
			return BitConverter.GetBytes(value);
		}

		public static long BytesToLong(byte[] values, int valuePosition)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Copy(values, valuePosition, BigEndianEightByteArray, 0, 8);
				Array.Reverse(BigEndianEightByteArray);
				return BitConverter.ToInt64(BigEndianEightByteArray, 0);
			}
			return BitConverter.ToInt64(values, valuePosition);
		}

		public static byte[] ULongToBytes(ulong value)
		{
			return BitConverter.GetBytes(value);
		}

		public static ulong BytesToULong(byte[] values, int valuePosition)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Copy(values, valuePosition, BigEndianEightByteArray, 0, 8);
				Array.Reverse(BigEndianEightByteArray);
				return BitConverter.ToUInt64(BigEndianEightByteArray, 0);
			}
			return BitConverter.ToUInt64(values, valuePosition);
		}

		public static byte[] ShortToBytes(short value)
		{
			return BitConverter.GetBytes(value);
		}

		public static short BytesToShort(byte[] values, int valuePosition)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Copy(values, valuePosition, s_BigEndianTwoByteArray, 0, 2);
				Array.Reverse(s_BigEndianTwoByteArray);
				return BitConverter.ToInt16(s_BigEndianTwoByteArray, 0);
			}
			return BitConverter.ToInt16(values, valuePosition);
		}

		public static byte[] UShortToBytes(ushort value)
		{
			return BitConverter.GetBytes(value);
		}

		public static ushort BytesToUShort(byte[] values, int valuePosition)
		{
			if (!BitConverter.IsLittleEndian)
			{
				Array.Copy(values, valuePosition, s_BigEndianTwoByteArray, 0, 2);
				Array.Reverse(s_BigEndianTwoByteArray);
				return BitConverter.ToUInt16(BigEndianEightByteArray, 0);
			}
			return BitConverter.ToUInt16(values, valuePosition);
		}

		public static byte[] BoolToBytes(bool value)
		{
			return BitConverter.GetBytes(value);
		}

		public static bool BytesToBool(byte[] values, int valuePosition)
		{
			return BitConverter.ToBoolean(values, valuePosition);
		}

		public static byte[] StringToBytes(string str)
		{
			if (str == null)
			{
				str = string.Empty;
			}
			return Encoding.UTF8.GetBytes(str);
		}

		public static byte[] ByteToBytes(byte value)
		{
			return new byte[1] { value };
		}

		public static byte BytesToByte(byte[] values, int valuePosition)
		{
			return values[valuePosition];
		}

		public static string BytesToString(byte[] values, int valuePosition, int valueSize)
		{
			return Encoding.UTF8.GetString(values, valuePosition, valueSize);
		}

		public static ICollection<byte> Vector2ToBytes(Vector2 vector2)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(vector2.x));
			list.AddRange(BitConverter.GetBytes(vector2.y));
			return list;
		}

		public static Vector2 BytesToVector2(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			Vector2 zero = Vector2.zero;
			zero.x = BytesToFloat(values, valuePosition);
			zero.y = BytesToFloat(values, valuePosition + 4);
			return zero;
		}

		public static ICollection<byte> Vector2IntToBytes(Vector2Int vector2int)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(vector2int.x));
			list.AddRange(BitConverter.GetBytes(vector2int.y));
			return list;
		}

		public static Vector2Int BytesToVector2Int(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			Vector2Int zero = Vector2Int.zero;
			zero.x = (BytesToInt(values, valuePosition));
			zero.y = (BytesToInt(values, valuePosition + 4));
			return zero;
		}

		public static ICollection<byte> Vector3ToBytes(Vector3 vector3)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(vector3.x));
			list.AddRange(BitConverter.GetBytes(vector3.y));
			list.AddRange(BitConverter.GetBytes(vector3.z));
			return list;
		}

		public static Vector3 BytesToVector3(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			Vector3 zero = Vector3.zero;
			zero.x = BytesToFloat(values, valuePosition);
			zero.y = BytesToFloat(values, valuePosition + 4);
			zero.z = BytesToFloat(values, valuePosition + 8);
			return zero;
		}

		public static ICollection<byte> Vector3IntToBytes(Vector3Int vector3int)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(vector3int.x));
			list.AddRange(BitConverter.GetBytes(vector3int.y));
			list.AddRange(BitConverter.GetBytes(vector3int.z));
			return list;
		}

		public static Vector3Int BytesToVector3Int(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			Vector3Int zero = Vector3Int.zero;
			zero.x =(BytesToInt(values, valuePosition));
			zero.y = (BytesToInt(values, valuePosition + 4));
			zero.z = (BytesToInt(values, valuePosition + 8));
			return zero;
		}

		public static ICollection<byte> Vector4ToBytes(Vector4 vector4)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(vector4.x));
			list.AddRange(BitConverter.GetBytes(vector4.y));
			list.AddRange(BitConverter.GetBytes(vector4.z));
			list.AddRange(BitConverter.GetBytes(vector4.w));
			return list;
		}

		public static Vector4 BytesToVector4(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			Vector4 zero = Vector4.zero;
			zero.x = BytesToFloat(values, valuePosition);
			zero.y = BytesToFloat(values, valuePosition + 4);
			zero.z = BytesToFloat(values, valuePosition + 8);
			zero.w = BytesToFloat(values, valuePosition + 12);
			return zero;
		}

		public static ICollection<byte> QuaternionToBytes(Quaternion quaternion)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(quaternion.x));
			list.AddRange(BitConverter.GetBytes(quaternion.y));
			list.AddRange(BitConverter.GetBytes(quaternion.z));
			list.AddRange(BitConverter.GetBytes(quaternion.w));
			return list;
		}

		public static Quaternion BytesToQuaternion(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			Quaternion identity = Quaternion.identity;
			identity.x = BytesToFloat(values, valuePosition);
			identity.y = BytesToFloat(values, valuePosition + 4);
			identity.z = BytesToFloat(values, valuePosition + 8);
			identity.w = BytesToFloat(values, valuePosition + 12);
			return identity;
		}

		public static ICollection<byte> ColorToBytes(Color color)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(color.r));
			list.AddRange(BitConverter.GetBytes(color.g));
			list.AddRange(BitConverter.GetBytes(color.b));
			list.AddRange(BitConverter.GetBytes(color.a));
			return list;
		}

		public static Color BytesToColor(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			Color black = Color.black;
			black.r = BytesToFloat(values, valuePosition);
			black.g = BytesToFloat(values, valuePosition + 4);
			black.b = BytesToFloat(values, valuePosition + 8);
			black.a = BytesToFloat(values, valuePosition + 12);
			return black;
		}

		public static ICollection<byte> RectToBytes(Rect rect)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(rect.x));
			list.AddRange(BitConverter.GetBytes(rect.y));
			list.AddRange(BitConverter.GetBytes(rect.width));
			list.AddRange(BitConverter.GetBytes(rect.height));
			return list;
		}

		public static Rect BytesToRect(byte[] values, int valuePosition)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			Rect result = default(Rect);
			result.x = (BytesToFloat(values, valuePosition));
			result.y = (BytesToFloat(values, valuePosition + 4));
			result.width = (BytesToFloat(values, valuePosition + 8));
			result.height = (BytesToFloat(values, valuePosition + 12));
			return result;
		}

		public static ICollection<byte> RectIntToBytes(RectInt rectInt)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(rectInt.x));
			list.AddRange(BitConverter.GetBytes(rectInt.y));
			list.AddRange(BitConverter.GetBytes(rectInt.width));
			list.AddRange(BitConverter.GetBytes(rectInt.height));
			return list;
		}

		public static RectInt BytesToRectInt(byte[] values, int valuePosition)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			RectInt result = default(RectInt);
			result.x =(BytesToInt(values, valuePosition));
			result.y = (BytesToInt(values, valuePosition + 4));
			result.width = (BytesToInt(values, valuePosition + 8));
			result.height = (BytesToInt(values, valuePosition + 12));
			return result;
		}

		public static ICollection<byte> BoundsToBytes(Bounds bounds)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(bounds.center.x));
			list.AddRange(BitConverter.GetBytes(bounds.center.y));
			list.AddRange(BitConverter.GetBytes(bounds.center.z));
			list.AddRange(BitConverter.GetBytes(bounds.size.x));
			list.AddRange(BitConverter.GetBytes(bounds.size.y));
			list.AddRange(BitConverter.GetBytes(bounds.size.z));
			return list;
		}

		public static Bounds BytesToBounds(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			Vector3 zero = Vector3.zero;
			zero.x = BytesToFloat(values, valuePosition);
			zero.y = BytesToFloat(values, valuePosition + 4);
			zero.z = BytesToFloat(values, valuePosition + 8);
			Vector3 zero2 = Vector3.zero;
			zero2.x = BytesToFloat(values, valuePosition + 12);
			zero2.y = BytesToFloat(values, valuePosition + 16);
			zero2.z = BytesToFloat(values, valuePosition + 20);
			return new Bounds(zero, zero2);
		}

		public static ICollection<byte> BoundsIntToBytes(BoundsInt bounds)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(bounds.center.x));
			list.AddRange(BitConverter.GetBytes(bounds.center.y));
			list.AddRange(BitConverter.GetBytes(bounds.center.z));
			Vector3Int size = bounds.size;
			list.AddRange(BitConverter.GetBytes(size.x));
			Vector3Int size2 = bounds.size;
			list.AddRange(BitConverter.GetBytes(size2.y));
			Vector3Int size3 = bounds.size;
			list.AddRange(BitConverter.GetBytes(size2.z));
			return list;
		}

		public static BoundsInt BytesToBoundsInt(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			Vector3Int zero = Vector3Int.zero;
			zero.x = (BytesToInt(values, valuePosition));
			zero.y = (BytesToInt(values, valuePosition + 4));
			zero.z = (BytesToInt(values, valuePosition + 8));
			Vector3Int zero2 = Vector3Int.zero;
			zero2.x = (BytesToInt(values, valuePosition + 12));
			zero2.y = (BytesToInt(values, valuePosition + 16));
			zero2.z = (BytesToInt(values, valuePosition + 20));
			return new BoundsInt(zero, zero2);
		}

		public static ICollection<byte> Matrix4x4ToBytes(Matrix4x4 matrix4x4)
		{
			List<byte> list = new List<byte>();
			list.AddRange(BitConverter.GetBytes(matrix4x4.m00));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m01));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m02));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m03));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m10));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m11));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m12));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m13));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m20));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m21));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m22));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m23));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m30));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m31));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m32));
			list.AddRange(BitConverter.GetBytes(matrix4x4.m33));
			return list;
		}

		public static Matrix4x4 BytesToMatrix4x4(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			Matrix4x4 identity = Matrix4x4.identity;
			identity.m00 = BytesToFloat(values, valuePosition);
			identity.m01 = BytesToFloat(values, valuePosition + 4);
			identity.m02 = BytesToFloat(values, valuePosition + 8);
			identity.m03 = BytesToFloat(values, valuePosition + 12);
			identity.m10 = BytesToFloat(values, valuePosition + 16);
			identity.m11 = BytesToFloat(values, valuePosition + 20);
			identity.m12 = BytesToFloat(values, valuePosition + 24);
			identity.m13 = BytesToFloat(values, valuePosition + 28);
			identity.m20 = BytesToFloat(values, valuePosition + 32);
			identity.m21 = BytesToFloat(values, valuePosition + 36);
			identity.m22 = BytesToFloat(values, valuePosition + 40);
			identity.m23 = BytesToFloat(values, valuePosition + 44);
			identity.m30 = BytesToFloat(values, valuePosition + 48);
			identity.m31 = BytesToFloat(values, valuePosition + 52);
			identity.m32 = BytesToFloat(values, valuePosition + 56);
			identity.m33 = BytesToFloat(values, valuePosition + 60);
			return identity;
		}

		public static ICollection<byte> AnimationCurveToBytes(AnimationCurve animationCurve)
		{
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Expected I4, but got Unknown
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Expected I4, but got Unknown
			if (animationCurve == null)
			{
				return null;
			}
			List<byte> list = new List<byte>();
			Keyframe[] keys = animationCurve.keys;
			if (keys != null)
			{
				list.AddRange(BitConverter.GetBytes(keys.Length));
				for (int i = 0; i < keys.Length; i++)
				{
					list.AddRange(BitConverter.GetBytes(keys[i].time));
					list.AddRange(BitConverter.GetBytes(keys[i].value));
					list.AddRange(BitConverter.GetBytes(keys[i].inTangent));
					list.AddRange(BitConverter.GetBytes(keys[i].outTangent));
				}
			}
			else
			{
				list.AddRange(BitConverter.GetBytes(0));
			}
			list.AddRange(BitConverter.GetBytes((int)animationCurve.preWrapMode));
			list.AddRange(BitConverter.GetBytes((int)animationCurve.preWrapMode));
			return list;
		}

		public static AnimationCurve BytesToAnimationCurve(byte[] values, int valuePosition)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			AnimationCurve val = new AnimationCurve();
			int num = BytesToInt(values, valuePosition);
			for (int i = 0; i < num; i++)
			{
				Keyframe val2 = default(Keyframe);
				val2.time = BytesToFloat(values, valuePosition + 4);
				val2.value = (BytesToFloat(values, valuePosition + 8));
				val2.inTangent = (BytesToFloat(values, valuePosition + 12));
				val2.outTangent = (BytesToFloat(values, valuePosition + 16));
				val.AddKey(val2);
				valuePosition += 16;
			}
			val.preWrapMode = (WrapMode)BytesToInt(values, valuePosition + 4);
			val.preWrapMode = (WrapMode)BytesToInt(values, valuePosition + 8);
			return val;
		}

		public static ICollection<byte> LayerMaskToBytes(LayerMask value)
		{
			return IntToBytes(value.value);
		}

		public static LayerMask BytesToLayerMask(byte[] values, int valuePosition)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			LayerMask result = default(LayerMask);
			result.value = (BytesToInt(values, valuePosition));
			return result;
		}

		public static Guid BytesToGuid(byte[] values, int valuePosition)
		{
			byte[] array = new byte[16];
			Array.Copy(values, valuePosition, array, 0, 16);
			return new Guid(array);
		}

		public static ICollection<byte> UnityObjectToBytes(Object unityObject, ref Object[] unityObjects)
		{
			if (unityObject == (Object)null)
			{
				return IntToBytes(-1);
			}
			if (unityObjects == null)
			{
				unityObjects = (Object[])(object)new Object[1];
			}
			else
			{
				Array.Resize(ref unityObjects, unityObjects.Length + 1);
			}
			unityObjects[unityObjects.Length - 1] = unityObject;
			return IntToBytes(unityObjects.Length - 1);
		}

		public static Object BytesToUnityObject(byte[] values, int valuePosition, Object[] unityObjects)
		{
			int num = BytesToInt(values, valuePosition);
			if (unityObjects != null && num >= 0 && num < unityObjects.Length)
			{
				return unityObjects[num];
			}
			return null;
		}
	}
}

