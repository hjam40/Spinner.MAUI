namespace Spinner.MAUI.Test;

public class SpinnerItem : ISpinnerItem
{
    public ImageSource ImageSource { get; set; }
    public string Text { get; set; } = string.Empty;
    public object Item { get; set; }
}
