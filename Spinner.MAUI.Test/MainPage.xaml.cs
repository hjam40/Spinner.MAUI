namespace Spinner.MAUI.Test;

public partial class MainPage : ContentPage
{
    public List<SpinnerItem> Items { get; set; }
    public List<SpinnerItem> Hours { get; set; }
    public List<SpinnerItem> Minutes { get; set; }
    public List<SpinnerItem> Seconds { get; set; }
    public int Hour { get; set; }
    public int Minute { get; set; }
    public int Second { get; set; }

    public MainPage()

	{
		InitializeComponent();
		Items = new List<SpinnerItem>();
        Hours = new List<SpinnerItem>();
        Minutes = new List<SpinnerItem>();
        Seconds = new List<SpinnerItem>();
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

