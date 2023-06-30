namespace UI.DataField
{
    public interface IDataField<T>
    {
        T Value { get; set; }
        void ShowError(string errorMessage);
    }
}