using System;
using System.Collections.Generic;
using UnityEngine;

namespace FSM {
	public class State<T> {
		public class Transition {
			public readonly State<T> targetState;
			public event Action OnTransition = delegate {};
			public Transition(State<T> targetState) {
				this.targetState = targetState;
			}
			internal void _Transition() { OnTransition(); }
		}

		public event Action OnEnter = delegate {};
		public event Action OnUpdate = delegate {};
		public event Action OnExit = delegate {};
		public readonly string name;
		
		Dictionary<T, Transition> _transitions = new Dictionary<T, Transition>();
		
		public State(string name = "<undefined>") {
			this.name = name;
		}

		public State<T> ResetTransition(T input) {
			_transitions.Remove(input);
			return this;
		}

		public State<T> SetTransition(T input, State<T> targetState) {
			Debug.Assert(targetState != null);
			#if FP_VERBOSE
			if(_transitions.ContainsKey(input))
				Debug.Log("Transition " + input.ToString() + " for state " + name + " already exists.");
			#endif
			_transitions[input] = new Transition(targetState);
			return this;
		}

		public Transition this[T input] {
			get {  return _transitions[input]; }
		}

		internal bool _TryGetTransition(T input, out Transition transition) {
			return _transitions.TryGetValue(input, out transition);
		}
		internal void _Enter() { OnEnter(); }
		internal void _Exit() { OnExit(); }
		internal void _Update() { OnUpdate(); }
	}
}