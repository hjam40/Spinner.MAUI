using System.Diagnostics;

namespace Spinner.MAUI;

public partial class SpinnerItemView : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SpinnerItemView), string.Empty, propertyChanged: TextChanged);

    private static void TextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        //Debug.WriteLine($"oldtext={oldValue} newtext={newValue}");
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SpinnerItemView), Colors.Black);
    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(SpinnerItemView), 16d);
    public static readonly BindableProperty FontAttributesProperty = BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(SpinnerItemView), FontAttributes.Bold);
    public static readonly BindableProperty LabelHorizontalOptionsProperty = BindableProperty.Create(nameof(LabelHorizontalOptions), typeof(LayoutOptions), typeof(SpinnerItemView), LayoutOptions.Fill);
    public static readonly BindableProperty LabelVerticalOptionsProperty = BindableProperty.Create(nameof(LabelVerticalOptions), typeof(LayoutOptions), typeof(SpinnerItemView), LayoutOptions.Center);
    public static readonly BindableProperty HorizontalTextAlignmentProperty = BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(SpinnerItemView), TextAlignment.Center);
    public static readonly BindableProperty VerticalTextAlignmentProperty = BindableProperty.Create(nameof(VerticalTextAlignment), typeof(TextAlignment), typeof(SpinnerItemView), TextAlignment.Center);
    public static readonly BindableProperty TextMarginProperty = BindableProperty.Create(nameof(TextMargin), typeof(Thickness), typeof(SpinnerItemView), new Thickness(0));

    public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create(nameof(ImageSource), typeof(ImageSource), typeof(SpinnerItemView), null, propertyChanged: ItemsSourceChanged);
    public static readonly BindableProperty ImageHorizontalOptionsProperty = BindableProperty.Create(nameof(ImageHorizontalOptions), typeof(LayoutOptions), typeof(SpinnerItemView), LayoutOptions.Start);
    public static readonly BindableProperty ImageVerticalOptionsProperty = BindableProperty.Create(nameof(ImageVerticalOptions), typeof(LayoutOptions), typeof(SpinnerItemView), LayoutOptions.Center);
    public static readonly BindableProperty ImageMarginProperty = BindableProperty.Create(nameof(ImageMargin), typeof(Thickness), typeof(SpinnerItemView), new Thickness(0));
    public static readonly BindableProperty ImageWidthProperty = BindableProperty.Create(nameof(ImageWidth), typeof(double), typeof(SpinnerItemView), -1d);
    public static readonly BindableProperty ImageHeightProperty = BindableProperty.Create(nameof(ImageHeight), typeof(double), typeof(SpinnerItemView), -1d);
    public static readonly BindableProperty ImageAspectProperty = BindableProperty.Create(nameof(ImageAspect), typeof(Aspect), typeof(SpinnerItemView), Aspect.AspectFit);
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }
    public double FontSize { get => (double)GetValue(FontSizeProperty); set => SetValue(FontSizeProperty, value); }
    public FontAttributes FontAttributes { get => (FontAttributes)GetValue(FontAttributesProperty); set => SetValue(FontAttributesProperty, value); }
    public LayoutOptions LabelHorizontalOptions { get => (LayoutOptions)GetValue(LabelHorizontalOptionsProperty); set => SetValue(LabelHorizontalOptionsProperty, value); }
    public LayoutOptions LabelVerticalOptions { get => (LayoutOptions)GetValue(LabelVerticalOptionsProperty); set => SetValue(LabelVerticalOptionsProperty, value); }
    public TextAlignment HorizontalTextAlignment { get => (TextAlignment)GetValue(HorizontalTextAlignmentProperty); set => SetValue(HorizontalTextAlignmentProperty, value); }
    public TextAlignment VerticalTextAlignment { get => (TextAlignment)GetValue(VerticalTextAlignmentProperty); set => SetValue(VerticalTextAlignmentProperty, value); }
    public Thickness TextMargin { get => (Thickness)GetValue(TextMarginProperty); set => SetValue(TextMarginProperty, value); }
    public ImageSource ImageSource { get => (ImageSource)GetValue(ImageSourceProperty); set { SetValue(ImageSourceProperty, value); ChangeGrid(); } }
    public Thickness ImageMargin { get => (Thickness)GetValue(ImageMarginProperty); set => SetValue(ImageMarginProperty, value); }
    public LayoutOptions ImageHorizontalOptions { get => (LayoutOptions)GetValue(ImageHorizontalOptionsProperty); set => SetValue(ImageHorizontalOptionsProperty, value); }
    public LayoutOptions ImageVerticalOptions { get => (LayoutOptions)GetValue(ImageVerticalOptionsProperty); set => SetValue(ImageVerticalOptionsProperty, value); }
    public double ImageWidth { get => (double)GetValue(ImageWidthProperty); set => SetValue(ImageWidthProperty, value); }
    public double ImageHeight { get => (double)GetValue(ImageHeightProperty); set => SetValue(ImageHeightProperty, value); }
    public Aspect ImageAspect { get => (Aspect)GetValue(ImageAspectProperty); set => SetValue(ImageAspectProperty, value); }
    
    public SpinnerItemView()
	{
		InitializeComponent();
	}
    public SpinnerItemView(string Text = "", ImageSource imageSource = null) : this()
    {
        this.Text = Text;
        ImageSource = imageSource;
    }
    public SpinnerItemView(ISpinnerItem item) : this()
    {
        Text = item.Text;
        ImageSource = item.ImageSource;
    }

    private void ChangeGrid()
    {
        if (ImageSource != null)
        {
            if (BaseGrid.ColumnDefinitions.Count <= 1) BaseGrid.ColumnDefinitions.Insert(0, new ColumnDefinition());
        }
        else
        {
            if (BaseGrid.ColumnDefinitions.Count > 1) BaseGrid.ColumnDefinitions.RemoveAt(0);
        }
    }

    internal bool IsEmpty()
    {
        return ImageSource == null && string.IsNullOrEmpty(Text);
    }

    private static void ItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue != newValue)
        {
            var itemView = bindable as SpinnerItemView;
            if (newValue == null)
                itemView.image.IsVisible = false;
            else if (newValue is ImageSource iSource)
            {
                itemView.image.Source = iSource;
                itemView.image.IsVisible = true;
            }
            else
                itemView.image.IsVisible = false;
            itemView.ChangeGrid();
        }
    }

}