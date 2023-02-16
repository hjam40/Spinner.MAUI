﻿namespace Spinner.MAUI;

internal class SpinnerItem : ISpinnerItem
{
    public ImageSource ImageSource { get; set; }
    public string Text { get; set; } = string.Empty;
    public object Item { get; set; }
}
