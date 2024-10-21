using System;
using System.Collections.Generic;
using System.Linq;

namespace GNET.Common
{
    public abstract class Observable
	{
		protected bool changed = false;
		private LinkedList<Observer> obs = new LinkedList<Observer> ();
		private object olock = new object ();

		public void addObserver (Observer o)
		{
			if (o == null)
				throw new ArgumentNullException ("Observer", "Add null observer");
			
			lock (olock) {
				if (!obs.Contains (o)) {
					obs.AddLast (o);
				}
			}
		}

		public void deleteObserver (Observer o)
		{
			lock (olock) {
				obs.Remove (o);
			}
		}

		public void notifyObservers (object arg)
		{
			Observer[] arrLocal;
			lock (olock) {
				if (!changed)
					return;
				
				arrLocal = obs.ToArray ();
				changed = false;
			}
			
			for (int i = arrLocal.Length - 1; i >= 0; i--) {
				arrLocal[i].update (this, arg);
			}
		}

		public void notifyObservers ()
		{
			notifyObservers (null);
		}

		public void deleteObservers ()
		{
			lock (olock) {
				obs.Clear ();
			}
		}

		public bool hasChanged ()
		{
			lock (olock) {
				return changed;
			}
		}

		public int countObservers ()
		{
			lock (olock) {
				return obs.Count;
			}
		}
	}
}
