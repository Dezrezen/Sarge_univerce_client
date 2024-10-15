using UnityEngine.Events;

namespace Common.Model
{
    public class DataValue<T>
    {
        protected T _data;
        protected readonly UnityEvent _updateEvent = new();

        public DataValue(T data)
        {
            _data = data;
        }

        public T GetValue()
        {
            return _data;
        }

        public void SetValue(T value)
        {
            _data = value;
            _updateEvent?.Invoke();
        }

        public void SubscribeForUpdate(UnityAction action, bool autoInvoke = true)
        {
            _updateEvent.AddListener(action);
            if (autoInvoke)
                action?.Invoke();
        }

        public void UnSubscribeFromUpdate(UnityAction action)
        {
            _updateEvent.RemoveListener(action);
        }
    }
}