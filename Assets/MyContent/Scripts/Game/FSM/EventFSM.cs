using System;

namespace FSM {
	public class EventFSM<T> {
		public readonly State<T> any;
		State<T> _current;
		bool _feeding = false;

		public State<T> current { get { return _current; } }

		public EventFSM(State<T> initial, State<T> any = null) {
			_current = initial;
			_current._Enter();
			this.any = any != null ? any : new State<T>("<any>");
			this.any.OnEnter += () => { throw new Exception("Can't make transition to fsm's <any> state"); };
		}
        public void Update() {
			_current._Update();
		}

		public bool Feed(T input) {
			if(_feeding)
				throw new Exception("Error: Feeding from OnEnter or OnExit, will cause repeated or missing calls");
			
			State<T>.Transition transition;
			if(
				_current._TryGetTransition(input, out transition)
				|| any._TryGetTransition(input, out transition)
			) {
				_feeding = true;		//Not multi-thread safe...

				_current._Exit();
				#if FP_VERBOSE
				Debug.Log("FSM state: " + _current.name + "---" + input + "---> " + transition.targetState.name);
				#endif
				transition._Transition();
				_current = transition.targetState;
				_current._Enter();
				
				_feeding = false;

				return true;
			}
			return false;
		}
	}
}