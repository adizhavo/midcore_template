namespace Services.Core.Databinding
{
    public interface BindingComponent<T>
    {
        void OnValueChanged(string branch, T value);
    }
}