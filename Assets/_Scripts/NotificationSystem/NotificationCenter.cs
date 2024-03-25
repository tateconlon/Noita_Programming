using System;
using System.Collections.Generic;
using UnityEngine;

//TATER
public class NotificationCenter
{
	private Dictionary<string, Dictionary<object, List<Action<object, object>>>> _table = new Dictionary<string, Dictionary<object, List<Action<object, object>>>>();

	private HashSet<List<Action<object, object>>> _invoking = new HashSet<List<Action<object, object>>>();

	public static readonly NotificationCenter instance = new NotificationCenter();

	private NotificationCenter()
	{
	}

	public void AddObserver(Action<object, object> handler, string notificationName)
	{
		AddObserver(handler, notificationName, null);
	}

	public void AddObserver(Action<object, object> handler, string notificationName, object sender)
	{
		if (handler == null)
		{
			Debug.LogError("Can't add a null event handler for notification, " + notificationName);
			return;
		}
		if (string.IsNullOrEmpty(notificationName))
		{
			Debug.LogError("Can't observe an unnamed notification");
			return;
		}
		if (!_table.ContainsKey(notificationName))
		{
			_table.Add(notificationName, new Dictionary<object, List<Action<object, object>>>());
		}
		
		Dictionary<object, List<Action<object, object>>> dictionary = _table[notificationName];
		object key = ((sender != null) ? sender : this);
		
		if (!dictionary.ContainsKey(key))
		{
			dictionary.Add(key, new List<Action<object, object>>());
		}
		List<Action<object, object>> list = dictionary[key];
		if (_invoking.Contains(list))
		{
			list = (dictionary[key] = new List<Action<object, object>>(list));
		}
		list.Add(handler);
	}

	public void RemoveObserver(Action<object, object> handler, string notificationName)
	{
		RemoveObserver(handler, notificationName, null);
	}

	public void RemoveObserver(Action<object, object> handler, string notificationName, object sender)
	{
		if (handler == null)
		{
			Debug.LogError("Can't remove a null event handler for notification, " + notificationName);
		}
		else if (string.IsNullOrEmpty(notificationName))
		{
			Debug.LogError("A notification name is required to stop observation");
		}
		else
		{
			if (!_table.ContainsKey(notificationName))
			{
				return;
			}
			Dictionary<object, List<Action<object, object>>> dictionary = _table[notificationName];
			object key = ((sender != null) ? sender : this);
			if (!dictionary.ContainsKey(key))
			{
				return;
			}
			List<Action<object, object>> list = dictionary[key];
			int num = list.IndexOf(handler);
			if (num != -1)
			{
				if (_invoking.Contains(list))
				{
					list = (dictionary[key] = new List<Action<object, object>>(list));
				}
				list.RemoveAt(num);
			}
		}
	}

	public void Clean()
	{
		string[] array = new string[_table.Keys.Count];
		_table.Keys.CopyTo(array, 0);
		for (int num = array.Length - 1; num >= 0; num--)
		{
			string key = array[num];
			Dictionary<object, List<Action<object, object>>> dictionary = _table[key];
			object[] array2 = new object[dictionary.Keys.Count];
			dictionary.Keys.CopyTo(array2, 0);
			for (int num2 = array2.Length - 1; num2 >= 0; num2--)
			{
				object key2 = array2[num2];
				if (dictionary[key2].Count == 0)
				{
					dictionary.Remove(key2);
				}
			}
			if (dictionary.Count == 0)
			{
				_table.Remove(key);
			}
		}
	}

	public void PostNotification(string notificationName)
	{
		PostNotification(notificationName, null);
	}

	public void PostNotification(string notificationName, object sender)
	{
		PostNotification(notificationName, sender, null);
	}

	public void PostNotification(string notificationName, object sender, object e)
	{
		if (string.IsNullOrEmpty(notificationName))
		{
			Debug.LogError("A notification name is required");
		}
		else
		{
			if (!_table.ContainsKey(notificationName))
			{
				return;
			}
			Dictionary<object, List<Action<object, object>>> dictionary = _table[notificationName];
			if (sender != null && dictionary.ContainsKey(sender))
			{
				List<Action<object, object>> list = dictionary[sender];
				_invoking.Add(list);
				for (int i = 0; i < list.Count; i++)
				{
					list[i](sender, e);
				}
				_invoking.Remove(list);
			}
			if (dictionary.ContainsKey(this))
			{
				List<Action<object, object>> list2 = dictionary[this];
				_invoking.Add(list2);
				for (int j = 0; j < list2.Count; j++)
				{
					list2[j](sender, e);
				}
				_invoking.Remove(list2);
			}
		}
	}
}
