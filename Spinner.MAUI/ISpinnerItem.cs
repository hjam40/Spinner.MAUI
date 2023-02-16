namespace Spinner.MAUI;

public interface ISpinnerItem
{
    public ImageSource ImageSource { get; set; }
    public string Text { get; set; }
    public object Item { get; set; }
}
