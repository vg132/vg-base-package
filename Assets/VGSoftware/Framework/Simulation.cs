using System;
using System.Collections.Generic;
using UnityEngine;

namespace VGSoftware.Framework
{
	public class Simulation : MonoBehaviour
  {
		private static HeapQueue<Event> eventQueue = new HeapQueue<Event>();
		private static Dictionary<Type, Stack<Event>> eventPools = new Dictionary<Type, Stack<Event>>();

		public static T New<T>(Action<T> initializer = null) where T : Event, new()
		{
			Stack<Event> pool;
			if (!eventPools.TryGetValue(typeof(T), out pool))
			{
				pool = new Stack<Event>(4);
				pool.Push(new T());
				eventPools[typeof(T)] = pool;
			}
			T instance = (pool.Count > 0) ? (T)pool.Pop() : new T();
			initializer?.Invoke(instance);
			return instance;
		}

		public static void Clear()
		{
			eventQueue.Clear();
		}

		public static void CreateAndSchedule<T>(Action<T> initializer = null, float tick = 0) where T : Event, new()
		{
			UnityMainThreadDispatcher.Instance.Enqueue(() =>
			{
				var ev = New<T>(initializer);
				Schedule(ev, tick);
			});
		}

		public static void Schedule<T>(float tick = 0) where T : Event, new()
		{
			Schedule<T>(null, tick);
		}

		private static void Schedule<T>(T ev = null, float tick = 0) where T : Event, new()
		{
			UnityMainThreadDispatcher.Instance.Enqueue(() =>
			{
				if (ev == null)
				{
					ev = New<T>();
				}
				ev.tick = Time.time + tick;
				eventQueue.Push(ev);
			});
		}

		public static int Tick()
		{
			var time = Time.time;
			var executedEventCount = 0;
			while (eventQueue.Count > 0 && eventQueue.Peek().tick <= time)
			{
				var ev = eventQueue.Pop();
				var tick = ev.tick;
				ev.ExecuteEvent();
				if (ev.tick > tick)
				{
					eventQueue.Push(ev);
				}
				else
				{
					ev.Cleanup();
					try
					{
						eventPools[ev.GetType()].Push(ev);
					}
					catch (KeyNotFoundException)
					{
						GameLog.LogError($"No Pool for: '{ev.GetType()}'");
					}
				}
				executedEventCount++;
			}
			return eventQueue.Count;
		}

		public abstract class Event : IComparable<Event>
		{
			public virtual void Execute() { }
			public virtual bool Precondition() => false;
			public virtual bool Delay() => false;
			internal float tick;

			public int CompareTo(Event other) => tick.CompareTo(other.tick);

			internal virtual void ExecuteEvent()
			{
				if (Precondition())
				{
					Execute();
				}
			}

			internal virtual void Cleanup()
			{
			}
		}

		public abstract class Event<T> : Event where T : Event<T>
		{
			public static Action<T> OnExecute;

			internal override void ExecuteEvent()
			{
				if (Delay())
				{
					tick += 1.0f;
					GameLog.Log($"*** Delay event '{GetType().Name}'. New Tick: {tick}");
					return;
				}
				if (Precondition())
				{
					Execute();
				}
				if (OnExecute != null)
				{
					OnExecute.Invoke((T)this);
				}
				else
				{
					GameLog.Log($"*** No handler found for event '{GetType().Name}'");
				}
			}
		}
	}
}