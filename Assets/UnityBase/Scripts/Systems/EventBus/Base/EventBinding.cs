using System;

namespace UnityBase.EventBus
{
    internal interface IEventBinding<T>
    {
        public Priority Priority { get; set; }
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
        public void ClearBindingData();
    }
    
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        private Action<T> _onEvent = _ => { };
        private Action _onEventNoArgs = () => { };
        private Priority _priority;
    
        Priority IEventBinding<T>.Priority
        {
            get => _priority;
            set => _priority = value;
        }
    
        Action<T> IEventBinding<T>.OnEvent 
        {
            get => _onEvent;
            set => _onEvent = value;
        }
    
        Action IEventBinding<T>.OnEventNoArgs 
        {
            get => _onEventNoArgs;
            set => _onEventNoArgs = value;
        }

        public EventBinding(Priority priority = Priority.Normal)
        {
            _priority = priority;
        }

        public EventBinding(Action<T> onEvent, Priority priority = Priority.Normal)
        {
            _onEvent = onEvent;
            _priority = priority;
        }

        public EventBinding(Action onEventNoArgs, Priority priority = Priority.Normal)
        {
            _onEventNoArgs = onEventNoArgs;
            _priority = priority;
        }

        public EventBinding<T> Add(Action<T> onEvent)
        {
             _onEvent += onEvent;
             return this;
        }

        public EventBinding<T> Add(Action onEventNoArgs)
        {
            _onEventNoArgs += onEventNoArgs;
            return this;
        }

        public EventBinding<T> Remove(Action<T> onEvent)
        {
            _onEvent -= onEvent;
            return this;
        }

        public EventBinding<T> Remove(Action onEventNoArgs)
        {
            _onEventNoArgs -= onEventNoArgs;
            return this;
        }

        public EventBinding<T> SetPriority(Priority priority)
        {
            _priority = priority;
            return this;
        }
        
        public void ClearBindingData()
        {
            _onEvent = null;
            _onEventNoArgs = null;
        }
    }
}