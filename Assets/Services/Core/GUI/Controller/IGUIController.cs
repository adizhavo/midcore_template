namespace Services.Core.GUI
{
    public interface IGUIController
    {
        void OnRefresh();

        void OnAppear();

        void OnDisappear();
    }
}