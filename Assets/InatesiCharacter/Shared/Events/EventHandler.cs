namespace InatesiCharacter.Shared.Events
{
	using System;
	using System.Collections.Generic;
	using InatesiCharacter.Shared.Utility;
	using UnityEngine;

	public class EventHandler : MonoBehaviour
	{
		private static Dictionary<object, Dictionary<string, List<InvokableActionBase>>> s_EventTable = new Dictionary<object, Dictionary<string, List<InvokableActionBase>>>();

		private static Dictionary<string, List<InvokableActionBase>> s_GlobalEventTable = new Dictionary<string, List<InvokableActionBase>>();

		private static void RegisterEvent(string eventName, InvokableActionBase invokableAction)
		{
			if (s_GlobalEventTable.TryGetValue(eventName, out var value))
			{
				value.Add(invokableAction);
				return;
			}
			value = new List<InvokableActionBase>();
			value.Add(invokableAction);
			s_GlobalEventTable.Add(eventName, value);
		}

		private static void RegisterEvent(object obj, string eventName, InvokableActionBase invokableAction)
		{
			if (!s_EventTable.TryGetValue(obj, out var value))
			{
				value = new Dictionary<string, List<InvokableActionBase>>();
				s_EventTable.Add(obj, value);
			}
			if (value.TryGetValue(eventName, out var value2))
			{
				value2.Add(invokableAction);
				return;
			}
			value2 = new List<InvokableActionBase>();
			value2.Add(invokableAction);
			value.Add(eventName, value2);
		}

		private static List<InvokableActionBase> GetActionList(string eventName)
		{
			if (s_GlobalEventTable.TryGetValue(eventName, out var value))
			{
				return value;
			}
			return null;
		}

		private static void CheckForEventRemoval(string eventName, List<InvokableActionBase> actionList)
		{
			if (actionList.Count == 0)
			{
				s_GlobalEventTable.Remove(eventName);
			}
		}

		private static List<InvokableActionBase> GetActionList(object obj, string eventName)
		{
			if (s_EventTable.TryGetValue(obj, out var value) && value.TryGetValue(eventName, out var value2))
			{
				return value2;
			}
			return null;
		}

		private static void CheckForEventRemoval(object obj, string eventName, List<InvokableActionBase> actionList)
		{
			if (actionList.Count == 0 && s_EventTable.TryGetValue(obj, out var value))
			{
				value.Remove(eventName);
				if (value.Count == 0)
				{
					s_EventTable.Remove(obj);
				}
			}
		}

		public static void RegisterEvent(string eventName, Action action)
		{
			InvokableAction invokableAction = GenericObjectPool.Get<InvokableAction>();
			invokableAction.Initialize(action);
			RegisterEvent(eventName, invokableAction);
		}

		public static void RegisterEvent(object obj, string eventName, Action action)
		{
			Debug.Log($"{obj} {eventName}");
			InvokableAction invokableAction = GenericObjectPool.Get<InvokableAction>();
			invokableAction.Initialize(action);
			RegisterEvent(obj, eventName, invokableAction);
		}

		public static void RegisterEvent<T1>(string eventName, Action<T1> action)
		{
			InvokableAction<T1> invokableAction = GenericObjectPool.Get<InvokableAction<T1>>();
			invokableAction.Initialize(action);
			RegisterEvent(eventName, invokableAction);
		}

		public static void RegisterEvent<T1>(object obj, string eventName, Action<T1> action)
		{
			InvokableAction<T1> invokableAction = GenericObjectPool.Get<InvokableAction<T1>>();
			invokableAction.Initialize(action);
			RegisterEvent(obj, eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2>(string eventName, Action<T1, T2> action)
		{
			InvokableAction<T1, T2> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2>>();
			invokableAction.Initialize(action);
			RegisterEvent(eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2>(object obj, string eventName, Action<T1, T2> action)
		{
			InvokableAction<T1, T2> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2>>();
			invokableAction.Initialize(action);
			RegisterEvent(obj, eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2, T3>(string eventName, Action<T1, T2, T3> action)
		{
			InvokableAction<T1, T2, T3> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2, T3>>();
			invokableAction.Initialize(action);
			RegisterEvent(eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2, T3>(object obj, string eventName, Action<T1, T2, T3> action)
		{
			InvokableAction<T1, T2, T3> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2, T3>>();
			invokableAction.Initialize(action);
			RegisterEvent(obj, eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> action)
		{
			InvokableAction<T1, T2, T3, T4> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2, T3, T4>>();
			invokableAction.Initialize(action);
			RegisterEvent(eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2, T3, T4>(object obj, string eventName, Action<T1, T2, T3, T4> action)
		{
			InvokableAction<T1, T2, T3, T4> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2, T3, T4>>();
			invokableAction.Initialize(action);
			RegisterEvent(obj, eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2, T3, T4, T5>(string eventName, InatesiCharacter.Shared.Events.Action<T1, T2, T3, T4, T5> action)
		{
			InvokableAction<T1, T2, T3, T4, T5> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2, T3, T4, T5>>();
			invokableAction.Initialize(action);
			RegisterEvent(eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2, T3, T4, T5>(object obj, string eventName, InatesiCharacter.Shared.Events.Action<T1, T2, T3, T4, T5> action)
		{
			InvokableAction<T1, T2, T3, T4, T5> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2, T3, T4, T5>>();
			invokableAction.Initialize(action);
			RegisterEvent(obj, eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2, T3, T4, T5, T6>(string eventName, InatesiCharacter.Shared.Events.Action<T1, T2, T3, T4, T5, T6> action)
		{
			InvokableAction<T1, T2, T3, T4, T5, T6> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2, T3, T4, T5, T6>>();
			invokableAction.Initialize(action);
			RegisterEvent(eventName, invokableAction);
		}

		public static void RegisterEvent<T1, T2, T3, T4, T5, T6>(object obj, string eventName, InatesiCharacter.Shared.Events.Action<T1, T2, T3, T4, T5, T6> action)
		{
			InvokableAction<T1, T2, T3, T4, T5, T6> invokableAction = GenericObjectPool.Get<InvokableAction<T1, T2, T3, T4, T5, T6>>();
			invokableAction.Initialize(action);
			RegisterEvent(obj, eventName, invokableAction);
		}

		public static void ExecuteEvent(string eventName)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction).Invoke();
				}
			}
		}

		public static void ExecuteEvent(object obj, string eventName)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction).Invoke();
				}
			}
		}

		public static void ExecuteEvent<T1>(string eventName, T1 arg1)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1>).Invoke(arg1);
				}
			}
		}

		public static void ExecuteEvent<T1>(object obj, string eventName, T1 arg1)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1>).Invoke(arg1);
				}
			}
		}

		public static void ExecuteEvent<T1, T2>(string eventName, T1 arg1, T2 arg2)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1, T2>).Invoke(arg1, arg2);
				}
			}
		}

		public static void ExecuteEvent<T1, T2>(object obj, string eventName, T1 arg1, T2 arg2)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1, T2>).Invoke(arg1, arg2);
				}
			}
		}

		public static void ExecuteEvent<T1, T2, T3>(string eventName, T1 arg1, T2 arg2, T3 arg3)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1, T2, T3>).Invoke(arg1, arg2, arg3);
				}
			}
		}

		public static void ExecuteEvent<T1, T2, T3>(object obj, string eventName, T1 arg1, T2 arg2, T3 arg3)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList != null)
			{
				for (int i = 0; i < actionList.Count; i++)
				{
					(actionList[i] as InvokableAction<T1, T2, T3>).Invoke(arg1, arg2, arg3);
				}
			}
		}

		public static void ExecuteEvent<T1, T2, T3, T4>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1, T2, T3, T4>).Invoke(arg1, arg2, arg3, arg4);
				}
			}
		}

		public static void ExecuteEvent<T1, T2, T3, T4>(object obj, string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1, T2, T3, T4>).Invoke(arg1, arg2, arg3, arg4);
				}
			}
		}

		public static void ExecuteEvent<T1, T2, T3, T4, T5>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1, T2, T3, T4, T5>).Invoke(arg1, arg2, arg3, arg4, arg5);
				}
			}
		}

		public static void ExecuteEvent<T1, T2, T3, T4, T5>(object obj, string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1, T2, T3, T4, T5>).Invoke(arg1, arg2, arg3, arg4, arg5);
				}
			}
		}

		public static void ExecuteEvent<T1, T2, T3, T4, T5, T6>(string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1, T2, T3, T4, T5, T6>).Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
				}
			}
		}

		public static void ExecuteEvent<T1, T2, T3, T4, T5, T6>(object obj, string eventName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList != null)
			{
				for (int num = actionList.Count - 1; num >= 0; num--)
				{
					(actionList[num] as InvokableAction<T1, T2, T3, T4, T5, T6>).Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
				}
			}
		}

		public static void UnregisterEvent(string eventName, Action action)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction invokableAction = actionList[i] as InvokableAction;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(eventName, actionList);
		}

		public static void UnregisterEvent(object obj, string eventName, Action action)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction invokableAction = actionList[i] as InvokableAction;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(obj, eventName, actionList);
		}

		public static void UnregisterEvent<T1>(string eventName, Action<T1> action)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1> invokableAction = actionList[i] as InvokableAction<T1>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(eventName, actionList);
		}

		public static void UnregisterEvent<T1>(object obj, string eventName, Action<T1> action)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1> invokableAction = actionList[i] as InvokableAction<T1>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(obj, eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2>(string eventName, Action<T1, T2> action)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2> invokableAction = actionList[i] as InvokableAction<T1, T2>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2>(object obj, string eventName, Action<T1, T2> action)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2> invokableAction = actionList[i] as InvokableAction<T1, T2>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(obj, eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2, T3>(string eventName, Action<T1, T2, T3> action)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2, T3> invokableAction = actionList[i] as InvokableAction<T1, T2, T3>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2, T3>(object obj, string eventName, Action<T1, T2, T3> action)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2, T3> invokableAction = actionList[i] as InvokableAction<T1, T2, T3>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(obj, eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2, T3, T4>(string eventName, Action<T1, T2, T3, T4> action)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2, T3, T4> invokableAction = actionList[i] as InvokableAction<T1, T2, T3, T4>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2, T3, T4>(object obj, string eventName, Action<T1, T2, T3, T4> action)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2, T3, T4> invokableAction = actionList[i] as InvokableAction<T1, T2, T3, T4>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(obj, eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2, T3, T4, T5>(string eventName, InatesiCharacter.Shared.Events.Action<T1, T2, T3, T4, T5> action)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2, T3, T4, T5> invokableAction = actionList[i] as InvokableAction<T1, T2, T3, T4, T5>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2, T3, T4, T5>(object obj, string eventName, InatesiCharacter.Shared.Events.Action<T1, T2, T3, T4, T5> action)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2, T3, T4, T5> invokableAction = actionList[i] as InvokableAction<T1, T2, T3, T4, T5>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(obj, eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2, T3, T4, T5, T6>(string eventName, InatesiCharacter.Shared.Events.Action<T1, T2, T3, T4, T5, T6> action)
		{
			List<InvokableActionBase> actionList = GetActionList(eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2, T3, T4, T5, T6> invokableAction = actionList[i] as InvokableAction<T1, T2, T3, T4, T5, T6>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(eventName, actionList);
		}

		public static void UnregisterEvent<T1, T2, T3, T4, T5, T6>(object obj, string eventName, InatesiCharacter.Shared.Events.Action<T1, T2, T3, T4, T5, T6> action)
		{
			List<InvokableActionBase> actionList = GetActionList(obj, eventName);
			if (actionList == null)
			{
				return;
			}
			for (int i = 0; i < actionList.Count; i++)
			{
				InvokableAction<T1, T2, T3, T4, T5, T6> invokableAction = actionList[i] as InvokableAction<T1, T2, T3, T4, T5, T6>;
				if (invokableAction.IsAction(action))
				{
					GenericObjectPool.Return(invokableAction);
					actionList.RemoveAt(i);
					break;
				}
			}
			CheckForEventRemoval(obj, eventName, actionList);
		}

		private void OnDisable()
		{
			if(gameObject != null || gameObject.activeSelf == true)
            {
				ClearTable();
			}
		}

		private void OnDestroy()
		{
			ClearTable();
		}

		private void ClearTable()
		{
			s_EventTable.Clear();
		}

		//[RuntimeInitializeOnLoadMethod(/*Could not decode attribute arguments.*/)]
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		public static void DomainReset()
		{
			if (s_EventTable != null)
			{
				s_EventTable.Clear();
			}
			if (s_GlobalEventTable != null)
			{
				s_GlobalEventTable.Clear();
			}
		}
	}

}
