namespace Services.Core.GUI
{
    public interface IGUIView
    {
        bool EnableOnAwake { get; }
        
        void Init();

        string GetId();
    }
}