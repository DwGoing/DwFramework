namespace DwFramework.DataFlow
{
    public interface ITaskQueue
    {
        public string ID { get; }
        public string AddInput(object input);
        public void ClearAllInputs();
        public void Excute();
        public object GetResult(out string inputId);
    }
}
