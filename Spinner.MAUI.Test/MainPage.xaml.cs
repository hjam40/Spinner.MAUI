namespace Spinner.MAUI.Test;

public partial class MainPage : ContentPage
{
    public List<ISpinnerItem> Items { get; set; }
    public List<ISpinnerItem> Hours { get; set; }
    public List<ISpinnerItem> Minutes { get; set; }
    public List<ISpinnerItem> Seconds { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Second { get; set; }

    public MainPage()

	{
		InitializeComponent();
		Items = new List<ISpinnerItem>();
        Hours = new List<ISpinnerItem>();
        Minutes = new List<ISpinnerItem>();
        Seconds = new List<ISpinnerItem>();
        for (int i = 0; i < 10; i++)
			Items.Add(new SpinnerItem { Text = "Item " + i.ToString(), ImageSource = ImageSource.FromFile("dotnet_bot.png") });
        OnPropertyChanged(nameof(Items));
		spinner.SelectedItem = Items[3];

        for (int i = 0; i < 24; i++)
            Hours.Add(new SpinnerItem { Text = i.ToString() });
        OnPropertyChanged(nameof(Hours));
        for (int i = 0; i < 60; i++)
        {
            Minutes.Add(new SpinnerItem { Text = i.ToString() });
            Seconds.Add(new SpinnerItem { Text = i.ToString() });
        }
        OnPropertyChanged(nameof(Minutes));
        OnPropertyChanged(nameof(Seconds));
        Hour = DateTime.Now.Hour;
        Minute = DateTime.Now.Minute;
        Second = DateTime.Now.Second;
        OnPropertyChanged(nameof(Hour));
        OnPropertyChanged(nameof(Minute));
        OnPropertyChanged(nameof(Second));
    }
}

