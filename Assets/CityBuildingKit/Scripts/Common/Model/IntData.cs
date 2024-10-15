namespace Common.Model
{
    public class IntData : DataValue<int>
    {
        public IntData(int data) : base(data) { }
        
        public void Set(int val)
        {
            _data = val;
            _updateEvent?.Invoke();
        }
        
        public void Add(int val)
        {
            _data += val;
            _updateEvent?.Invoke();
        }
    }
}