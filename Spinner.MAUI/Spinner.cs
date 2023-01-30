using Microsoft.Maui.Controls.Shapes;
using System.Collections.Specialized;
using System.Diagnostics;

using Path = Microsoft.Maui.Controls.Shapes.Path;

namespace Spinner.MAUI;

public class Spinner : ContentView
{
    private const string UPBUTTONDATA = "M11.4,7.45a1.99,1.99,0,0,0-2.85,0L5.69,10.29a0.98,0.98,0,0,0,1.4,1.4L9.95,8.85,12.8,11.69a0.99,0.99,0,0,0,1.4-1.4Z";
    private const double MINANIMATIONTIME = 200;
    private const double MAXANIMATIONTIME = 3000;
    //Default styles
    private static bool STYLESINITIATED = false;
    private static readonly Style BUTTONSTYLE = new(typeof(Border));
    private static readonly Style BUTTONPATHSTYLE = new(typeof(Path));
    private static readonly Style SELECTIONBOXSTYLE = new(typeof(Border));

    public static readonly BindableProperty NumItemsToShowProperty = BindableProperty.Create(nameof(NumItemsToShow), typeof(int), typeof(Spinner), 3, propertyChanged: ItemsToShowChanged);
    public static readonly BindableProperty IsCyclicProperty = BindableProperty.Create(nameof(IsCyclic), typeof(bool), typeof(Spinner), false, propertyChanged: (b, o, n) => { if (o != n) ((Spinner)b).RefreshItems(true); });
    public static readonly BindableProperty UseHapticProperty = BindableProperty.Create(nameof(UseHaptic), typeof(bool), typeof(Spinner), true);
    public static readonly BindableProperty UseAccelerationProperty = BindableProperty.Create(nameof(UseAcceleration), typeof(bool), typeof(Spinner), true);
    public static readonly BindableProperty ItemsProperty = BindableProperty.Create(nameof(Items), typeof(IEnumerable<SpinnerItem>), typeof(Spinner), null, propertyChanged: ItemsChanged);
    public static readonly BindableProperty SelectedItemIndexProperty = BindableProperty.Create(nameof(SelectedItemIndex), typeof(int), typeof(Spinner), 0, propertyChanged: SelectedItemIdxChanged);
    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(SpinnerItem), typeof(Spinner), null, propertyChanged: SelectedItemChange);
    
    public static readonly BindableProperty UpButtonDataProperty = BindableProperty.Create(nameof(UpButtonData), typeof(string), typeof(Spinner), UPBUTTONDATA, propertyChanged: (b, o, n) => { if (o != n) ((Spinner)b).RefreshSizes(); });
    public static readonly BindableProperty ButtonsStyleProperty = BindableProperty.Create(nameof(ButtonsStyle), typeof(Style), typeof(Spinner), BUTTONSTYLE, propertyChanged: (b, o, n) => { if (o != n) ((Spinner)b).RefreshSizes(); });
    public static readonly BindableProperty ButtonsPathStyleProperty = BindableProperty.Create(nameof(ButtonsPathStyle), typeof(Style), typeof(Spinner), BUTTONPATHSTYLE, propertyChanged: (b, o, n) => { if (o != n) ((Spinner)b).RefreshSizes(); });
    public static readonly BindableProperty ButtonsAreVisibleProperty = BindableProperty.Create(nameof(ButtonsAreVisible), typeof(bool), typeof(Spinner), false, propertyChanged: (b, o, n) => { if (o != n) ((Spinner)b).RefreshSizes(); });

    public static readonly BindableProperty ItemStyleProperty = BindableProperty.Create(nameof(ItemStyle), typeof(Style), typeof(Spinner), null, propertyChanged: (b, o, n) => { if (o != n) ((Spinner)b).RefreshSizes(); });

    public static readonly BindableProperty SelectionBoxStyleProperty = BindableProperty.Create(nameof(SelectionBoxStyle), typeof(Style), typeof(Spinner), SELECTIONBOXSTYLE);
    public static readonly BindableProperty SelectionBoxIsVisibleProperty = BindableProperty.Create(nameof(SelectionBoxIsVisible), typeof(bool), typeof(Spinner), true);
    /// <summary>
    /// Set the number of spinner items visibles.
    /// </summary>
    public int NumItemsToShow { get => (int)GetValue(NumItemsToShowProperty); set => SetValue(NumItemsToShowProperty, value); }
    /// <summary>
    /// Indicates if the spinner is cyclic or not.
    /// </summary>
    public bool IsCyclic { get => (bool)GetValue(IsCyclicProperty); set => SetValue(IsCyclicProperty, value); }
    /// <summary>
    /// Indicates the use of Haptic vibration when spinner selected item changes.
    /// </summary>
    public bool UseHaptic { get => (bool)GetValue(UseHapticProperty); set => SetValue(UseHapticProperty, value); }
    /// <summary>
    /// Indicates the use of acceleration in the spinner rotation.
    /// </summary>
    public bool UseAcceleration { get => (bool)GetValue(UseAccelerationProperty); set => SetValue(UseAccelerationProperty, value); }
    /// <summary>
    /// A collection of SpinnerItem elemtes for the spinner.
    /// </summary>
    public IEnumerable<SpinnerItem> Items { get => (IEnumerable<SpinnerItem>)GetValue(ItemsProperty); set => SetValue(ItemsProperty, value); }
    /// <summary>
    /// Indicates the selected item index when user changes it.
    /// </summary>
    public int SelectedItemIndex { get => (int)GetValue(SelectedItemIndexProperty); set => SetValue(SelectedItemIndexProperty, value); }
    /// <summary>
    /// Indicates the selected item when user changes it.
    /// </summary>
    public SpinnerItem SelectedItem { get => (SpinnerItem)GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }
    public string UpButtonData
    {
        get => (string)GetValue(UpButtonDataProperty);
        set
        {
            SetValue(UpButtonDataProperty, value);
            try
            {
                UpPath.Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(value);
                DownPath.Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(value);
            }
            catch
            {
                UpPath.Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(UPBUTTONDATA);
                DownPath.Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(UPBUTTONDATA);
            }
        }
    }
    /// <summary>
    /// Style Target Type Border. Define the aspect for the up and down buttons.
    /// </summary>
    public Style ButtonsStyle { get => (Style)GetValue(ButtonsStyleProperty); set => SetValue(ButtonsStyleProperty, value); }
    /// <summary>
    /// Style Target Type Path. Define the aspect for the up and down buttons icon.
    /// </summary>
    public Style ButtonsPathStyle { get => (Style)GetValue(ButtonsPathStyleProperty); set => SetValue(ButtonsPathStyleProperty, value); }
    /// <summary>
    /// Set visivilite for the up and down buttons.
    /// </summary>
    public bool ButtonsAreVisible { get => (bool)GetValue(ButtonsAreVisibleProperty); set => SetValue(ButtonsAreVisibleProperty, value); }
    /// <summary>
    /// Style Target Type SpinnerItemView. Define the aspect for spinner items.
    /// </summary>
    public Style ItemStyle { get => (Style)GetValue(ItemStyleProperty); set => SetValue(ItemStyleProperty, value); }
    /// <summary>
    /// Style Target Type Border. Define the aspect for the spinner central Border box.
    /// </summary>
    public Style SelectionBoxStyle { get => (Style)GetValue(SelectionBoxStyleProperty); set => SetValue(SelectionBoxStyleProperty, value); }
    /// <summary>
    /// Indicates if the spinner central Border box is visible.
    /// </summary>
    public bool SelectionBoxIsVisible { get => (bool)GetValue(SelectionBoxIsVisibleProperty); set => SetValue(SelectionBoxIsVisibleProperty, value); }
    /// <summary>
    /// Trigger when user change the selected item. The event does not trigger if SelectedItem or SelectedItemIndex property is set.
    /// </summary>
    public event EventHandler SelectionChanged;

    private double itemHeight;
    private int numShowingItems = 5;
    private int tmpSelectedIndex = 0, oldSelectedIndex = -1;
    private bool animatingUpButton = false;
    private bool animatingDownButton = false;
    
    private double totalPan = 0;
    private readonly object translationLock = new();
    private DateTime panStart;
    private bool endingPan = false;
    private bool panning = false;
    private bool panStarted = false;
    private double lastTotalPan = -100000;
    private double panVel = 0;
    private bool changingSelection = false;

    private readonly Grid BaseGrid;
    private readonly Border SelectionBorder;
    private List<SpinnerItemView> showItems;
    private Path UpPath;
    private Border UpBorder;
    private Path DownPath;
    private Border DownBorder;

    public Spinner()
    {
        DefineDefaultStyles();
        BaseGrid = new Grid { BindingContext = this, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
        BaseGrid.SetBinding(BackgroundProperty, nameof(Background));
        Background = Brush.Transparent;
        PanGestureRecognizer panGesture = new();
        panGesture.PanUpdated += PanGesture_PanUpdated;
        PointerGestureRecognizer pointerGesture = new();
        pointerGesture.PointerExited += PointerGesture_PointerExited;
        BaseGrid.GestureRecognizers.Add(panGesture);

        if (DeviceInfo.Current.Platform == DevicePlatform.WinUI)
            BaseGrid.GestureRecognizers.Add(pointerGesture);


        SelectionBorder = new Border { BindingContext = this, Content = new Label { IsVisible = false } };
        SelectionBorder.SetBinding(StyleProperty, nameof(SelectionBoxStyle));
        SelectionBorder.SetBinding(IsVisibleProperty, nameof(SelectionBoxIsVisible));
        InitItemViews();

        UpPath = new Path
        {
            BindingContext = this,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(UPBUTTONDATA)
        };
        UpPath.SetBinding(StyleProperty, nameof(ButtonsPathStyle));
        UpBorder = new Border { BindingContext = this, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Start };
        UpBorder.SetBinding(StyleProperty, nameof(ButtonsStyle));
        UpBorder.SetBinding(IsVisibleProperty, nameof(ButtonsAreVisible));
        UpBorder.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { UpClicked(); }) });
        var UpPathStk = new StackLayout
        {
            BindingContext = this,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Start
        };
        UpPathStk.Add(UpPath);
        UpBorder.Content = UpPathStk;
        BaseGrid.Add(UpBorder);
        DownPath = new Path
        {
            BindingContext = this,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Data = (Geometry)new PathGeometryConverter().ConvertFromInvariantString(UPBUTTONDATA),
            RotationX = 180
        };
        DownPath.SetBinding(StyleProperty, nameof(ButtonsPathStyle));
        DownBorder = new Border { BindingContext = this, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.End };
        DownBorder.SetBinding(StyleProperty, nameof(ButtonsStyle));
        DownBorder.SetBinding(IsVisibleProperty, nameof(ButtonsAreVisible));
        DownBorder.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => { DownClicked(); }) });
        var DownPathStk = new StackLayout
        {
            BindingContext = this,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.End
        };
        DownPathStk.Add(DownPath);
        DownBorder.Content = DownPathStk;
        BaseGrid.Add(DownBorder);

        //BaseGrid.Add(PanStk);
        Content = BaseGrid;
    }

    private static void ItemsToShowChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue != newValue && newValue is int nItems)
        {
            Spinner spinner = (Spinner)bindable;
            if (nItems % 2 == 0) nItems++;
            nItems = Math.Max(nItems, 1);
            lock (spinner.translationLock)
            {
                spinner.NumItemsToShow = nItems;
                spinner.InitItemViews();
                spinner.RefreshItems(true);
                spinner.RefreshSizes();
                spinner.lastTotalPan = 100000;
                spinner.totalPan = 0;
                spinner.StopSpinner();
            }
        }
    }
    private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue != newValue)
        {
            var spinner = bindable as Spinner;
            if (newValue != null && newValue is INotifyCollectionChanged items)
            {
                items.CollectionChanged += spinner.Events_CollectionChanged; ;
            }
            if (oldValue != null && oldValue is INotifyCollectionChanged itemsOld)
            {
                itemsOld.CollectionChanged -= spinner.Events_CollectionChanged;
            }
            lock (spinner.translationLock) spinner.RefreshItems(true);
        }
    }
    private void Events_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        lock (translationLock) RefreshItems(true);
    }
    private static void SelectedItemIdxChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var spinner = bindable as Spinner;
        if (oldValue != newValue && !spinner.changingSelection && newValue is int idx)
        {
            spinner.changingSelection = true;
            if (spinner.Items != null && idx >= 0 && idx < spinner.Items.Count())
            {
                lock (spinner.translationLock)
                {
                    spinner.tmpSelectedIndex = idx;
                    spinner.SetSelection(true);
                    spinner.RefreshItems(true);
                }
            }
            else
            {
                lock (spinner.translationLock)
                {
                    spinner.tmpSelectedIndex = 0;
                    spinner.SetSelection(true);
                    spinner.RefreshItems(true);
                }
            }
            spinner.changingSelection = false;
        }
    }

    private static void SelectedItemChange(BindableObject bindable, object oldValue, object newValue)
    {
        var spinner = bindable as Spinner;
        if (oldValue != newValue && !spinner.changingSelection && newValue is SpinnerItem item)
        {
            spinner.changingSelection = true;
            if (spinner.Items != null)
            {
                var selItem = spinner.Items.FirstOrDefault(i => i == item);
                if (selItem != null)
                {
                    lock (spinner.translationLock)
                    {
                        spinner.tmpSelectedIndex = spinner.IndexOf(selItem);
                        spinner.SetSelection(true);
                        spinner.RefreshItems(true);
                    }
                }
                else
                {
                    lock (spinner.translationLock)
                    {
                        spinner.tmpSelectedIndex = 0;
                        spinner.SetSelection(true);
                        spinner.RefreshItems(true);
                    }
                }
            }
            else
            {
                lock (spinner.translationLock)
                {
                    spinner.tmpSelectedIndex = 0;
                    spinner.SetSelection(true);
                    spinner.SelectedItem = null;
                }
            }
            spinner.changingSelection = false;
        }
    }
    private void InitItemViews()
    {
        numShowingItems = NumItemsToShow + 2;
        if (showItems != null) showItems[showItems.Count / 2].SizeChanged -= Items_SizeChanged;
        BaseGrid.Children.Clear();
        showItems = new List<SpinnerItemView>();
        for (int i = 0; i < NumItemsToShow + 2; i++)
        {
            SpinnerItemView item = new() { BindingContext = this, VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Fill };
            item.SetBinding(StyleProperty, nameof(ItemStyle));
            showItems.Add(item);
            BaseGrid.Add(showItems[i]);
        }
        showItems[numShowingItems / 2].SizeChanged += Items_SizeChanged;
        BaseGrid.Add(SelectionBorder);
    }
    private void RefreshItems(bool force)
    {
        if (oldSelectedIndex != tmpSelectedIndex || force)
        {
            oldSelectedIndex = tmpSelectedIndex;
            if (showItems[0].TranslationY < -itemHeight * ((double)NumItemsToShow / 2d) - itemHeight/2)
            {
                var item = showItems[0];
                showItems.RemoveAt(0);
                showItems.Add(item);
            }
            else if (showItems[numShowingItems - 1].TranslationY > itemHeight * ((double)NumItemsToShow / 2d) + itemHeight/2)
            {
                var item = showItems[numShowingItems - 1];
                showItems.RemoveAt(numShowingItems - 1);
                showItems.Insert(0, item);
            }

            if (Items != null && Items.Any())
            {
                showItems[numShowingItems / 2].Text = Items.ElementAt(tmpSelectedIndex).Text;
                showItems[numShowingItems / 2].ImageSource = Items.ElementAt(tmpSelectedIndex).ImageSource;
                int count = 1;
                int startIndex = tmpSelectedIndex;
                for (int i = (numShowingItems / 2) - 1; i >= 0; i--)
                {
                    if (startIndex - count < 0)
                    {
                        if (IsCyclic)
                        {
                            startIndex = Items.Count();
                            count = 1;
                            showItems[i].Text = Items.ElementAt(startIndex - count).Text;
                            showItems[i].ImageSource = Items.ElementAt(startIndex - count).ImageSource;
                        }
                        else
                        {
                            showItems[i].Text = string.Empty;
                            showItems[i].ImageSource = null;
                        }
                    }
                    else
                    {
                        showItems[i].Text = Items.ElementAt(startIndex - count).Text;
                        showItems[i].ImageSource = Items.ElementAt(startIndex - count).ImageSource;
                    }
                    count++;
                }
                count = 1;
                startIndex = tmpSelectedIndex;
                for (int i = (numShowingItems / 2) + 1; i < numShowingItems; i++)
                {
                    if (startIndex + count >= Items.Count())
                    {
                        if (IsCyclic)
                        {
                            startIndex = 0;
                            count = 0;
                            showItems[i].Text = Items.ElementAt(startIndex + count).Text;
                            showItems[i].ImageSource = Items.ElementAt(startIndex + count).ImageSource;
                        }
                        else
                        {
                            showItems[i].Text = string.Empty;
                            showItems[i].ImageSource = null;
                        }
                    }
                    else
                    {
                        showItems[i].Text = Items.ElementAt(startIndex + count).Text;
                        showItems[i].ImageSource = Items.ElementAt(startIndex + count).ImageSource;
                    }
                    count++;
                }
            }
            else
            {
                for (int i = 0; i < numShowingItems; i++) showItems[i].Text = string.Empty;
            }
        }
    }
    private int IndexOf(SpinnerItem spinnerItem)
    {
        int idx = 0;
        if (spinnerItem != null)
        {
            foreach (SpinnerItem item in Items)
            {
                if (item == spinnerItem) break;
                idx++;
            }
        }
        return idx;
    }
    private void PanGesture_PanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                lock (translationLock)
                {
                    panStarted = true;
                    totalPan = 0;
                    panStart = DateTime.Now;
                    lastTotalPan = -100000;
                    panVel = 0;
                    endingPan = false;
                }
                SetSelection();
                break;
            case GestureStatus.Running:
                if (panStarted)
                {
                    lock (translationLock)
                    {
                        panVel = (Math.Abs(e.TotalY - totalPan)) / (DateTime.Now - panStart).TotalSeconds;
                        panStart = DateTime.Now;
                        totalPan = e.TotalY;
                        RefreshTranslations();
                    }
                }
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                if (panStarted)
                    lock (translationLock) StopSpinner();
                break;
        }
    }
    private void UpClicked()
    {
        if (IsCyclic || SelectedItemIndex > 0)
        {
            if (!animatingUpButton)
            {
                animatingUpButton = true;
                new Animation
            {
                { 0, 0.5, new Animation (v => UpBorder.Scale = v, 1, 0.9) },
                { 0.5, 1, new Animation (v => UpBorder.Scale = v, 0.9, 1) },
            }.Commit(this, "ButtonsAnim", 16, 200, null, (a, b) => { animatingUpButton = false; });
            }
            lock (translationLock) StopSpinner(1);
        }
    }
    private void DownClicked()
    {
        if (IsCyclic || SelectedItemIndex < Items.Count() - 1)
        {
            if (!animatingDownButton)
            {
                animatingDownButton = true;
                new Animation
            {
                { 0, 0.5, new Animation (v => DownBorder.Scale = v, 1, 0.9) },
                { 0.5, 1, new Animation (v => DownBorder.Scale = v, 0.9, 1) },
            }.Commit(this, "ButtonsAnim", 16, 200, null, (a, b) => { animatingDownButton = false; });
            }
            lock (translationLock) StopSpinner(-1);
        }
    }
    private void PointerGesture_PointerExited(object sender, PointerEventArgs e)
    {
        if (panStarted)
            lock (translationLock) StopSpinner();
    }
    private void StopSpinner(double itemsToMove = 0)
    {
        double initAnim = totalPan;
        double endAnim, animTime;
        panStarted = false;
        if (UseAcceleration && itemsToMove == 0)
        {
            double time = (DateTime.Now - panStart).TotalSeconds;
            double weight = 25;
            double fk = 30 * weight * 9.8;
            double accel = -fk / weight;
            if (IsCyclic)
            {
                animTime = -panVel / accel;
                endAnim = panVel * animTime + 0.5 * accel * animTime * animTime;
                endAnim = Math.Round((totalPan + (totalPan > 0 ? endAnim : -endAnim)) / itemHeight) * itemHeight;
            }
            else
            {
                animTime = -panVel / accel;
                endAnim = panVel * animTime + 0.5 * accel * animTime * animTime;
                int iCount = Items != null ? Items.Count() : 0;
                if (totalPan > 0 && endAnim > tmpSelectedIndex * itemHeight)
                {
                    endAnim = tmpSelectedIndex * itemHeight;
                    animTime = (-panVel + Math.Sqrt(panVel * panVel - 2 * accel * endAnim)) / accel;
                    endAnim += Math.Round(totalPan / itemHeight) * itemHeight;
                    endAnim = Math.Min(endAnim, SelectedItemIndex * itemHeight);
                }
                else if (totalPan < 0 && -endAnim < (iCount - tmpSelectedIndex - 1) * -itemHeight)
                {
                    endAnim = (iCount - tmpSelectedIndex - 1) * -itemHeight;
                    animTime = (-panVel + Math.Sqrt(panVel * panVel - 2 * accel * endAnim)) / accel;
                    endAnim += Math.Round(totalPan / itemHeight) * itemHeight;
                    endAnim = Math.Max(endAnim, (iCount - SelectedItemIndex - 1) * -itemHeight);
                }
                else
                    endAnim = Math.Round((totalPan + (totalPan > 0 ? endAnim : -endAnim)) / itemHeight) * itemHeight;
            }
            animTime = Math.Max(MINANIMATIONTIME, Math.Abs(animTime) * 1000);
        }
        else
        {
            endAnim = Math.Round((totalPan + itemsToMove * itemHeight) / itemHeight) * itemHeight;
            animTime = MINANIMATIONTIME;
        }

        this.AbortAnimation("EndPanAnim");
        bool animCanceled = false;
        endingPan = true;
        animTime = Math.Min(animTime, MAXANIMATIONTIME);
        var anim = new Animation(v =>
        {
            lock (translationLock)
            {
                if (endingPan && !animCanceled)
                {
                    totalPan = v;
                    RefreshTranslations();
                }
                else
                    animCanceled = true;
            }
        }, initAnim, endAnim);
        anim.Commit(this, "EndPanAnim", 16, (uint)animTime, Easing.CubicOut, finished: (v, c) => { if (endingPan && !animCanceled) SetSelection(); });
    }
    private void SetSelection(bool changingSelection = false)
    {
        lock (translationLock)
        {
            SelectedItemIndex = tmpSelectedIndex;
            SelectedItem = Items?.ElementAt(SelectedItemIndex);
            totalPan = 0;
            if(!changingSelection) SelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    private void RefreshTranslations()
    {
        if (itemHeight > 0 && !panning && lastTotalPan != totalPan)
        {
            panning = true;
            lastTotalPan = totalPan;

            int iCount = Items != null ? Items.Count() : 0;
            if (!IsCyclic && !endingPan)
            {
                if (totalPan > 0)
                    totalPan = Math.Min(totalPan, SelectedItemIndex * itemHeight + itemHeight * 0.3);
                else
                    totalPan = Math.Max(totalPan, ((iCount - SelectedItemIndex - 1) * -itemHeight) - itemHeight * 0.3);
            }
            int oldTmpeSelectedIndex = tmpSelectedIndex;
            tmpSelectedIndex = SelectedItemIndex + -(int)Math.Truncate((totalPan / itemHeight));
            if (tmpSelectedIndex < 0)
            {
                if (IsCyclic)
                    while (tmpSelectedIndex < 0 && iCount > 0) tmpSelectedIndex = iCount + tmpSelectedIndex;
                tmpSelectedIndex = Math.Max(tmpSelectedIndex, 0);
            }
            else if (tmpSelectedIndex >= iCount)
            {
                if (IsCyclic)
                    while (tmpSelectedIndex >= iCount && iCount > 0) tmpSelectedIndex -= iCount;
                tmpSelectedIndex = Math.Max(0, Math.Min(iCount - 1, tmpSelectedIndex));
            }
            RefreshItems(false);
            for (int i = 0; i < numShowingItems; i++)
            {
                showItems[i].TranslationY = totalPan - Math.Truncate(totalPan / itemHeight) * itemHeight - itemHeight * ((numShowingItems / 2) - i);
                showItems[i].RotationX = Math.Min(90d, 90 * -showItems[i].TranslationY / (itemHeight * (double)NumItemsToShow / 2d));
                if (showItems[i].RotationX < 0) showItems[i].RotationX = Math.Max(270d, showItems[i].RotationX = 360 + showItems[i].RotationX);
                showItems[i].IsVisible = showItems[i].RotationX < 87d || showItems[i].RotationX > 273d;
            }
            if (UseHaptic && oldTmpeSelectedIndex != tmpSelectedIndex)
                new Task(()=>HapticFeedback.Default.Perform(HapticFeedbackType.Click)).Start();

            panning = false;
        }
    }
    private void Items_SizeChanged(object sender, EventArgs e)
    {
        if (sender is SpinnerItemView sItem)
        {
            if (itemHeight < sItem.Height)
            {
                itemHeight = sItem.Height;
                RefreshSizes();
            }
        }
    }
    private void RefreshSizes()
    {
        if (itemHeight > 0)
        {
            SelectionBorder.MinimumHeightRequest = itemHeight + SelectionBorder.StrokeThickness * 2d + SelectionBorder.Padding.Top + SelectionBorder.Padding.Bottom;
            BaseGrid.MinimumHeightRequest = itemHeight * NumItemsToShow + (ButtonsAreVisible ? UpBorder.HeightRequest * 2 : 0);
            if (SelectionBorder.HeightRequest > BaseGrid.MinimumHeightRequest)
                BaseGrid.MinimumHeightRequest = SelectionBorder.HeightRequest;
            lock (translationLock)
            {
                lastTotalPan = 100000;
                RefreshTranslations();
            }
        }
    }
    private static void DefineDefaultStyles()
    {
        if (!STYLESINITIATED)
        {
            SELECTIONBOXSTYLE.Setters.Add(HorizontalOptionsProperty, LayoutOptions.Fill);
            SELECTIONBOXSTYLE.Setters.Add(VerticalOptionsProperty, LayoutOptions.Center);
            SELECTIONBOXSTYLE.Setters.Add(PaddingProperty, new Thickness(10, 3, 10, 3));
            SELECTIONBOXSTYLE.Setters.Add(Border.StrokeProperty, Brush.DarkGray);
            SELECTIONBOXSTYLE.Setters.Add(Border.StrokeThicknessProperty, 1.5d);
            SELECTIONBOXSTYLE.Setters.Add(BackgroundProperty, (Brush)Color.FromRgba("#99999933"));
            SELECTIONBOXSTYLE.Setters.Add(Border.StrokeShapeProperty, new RoundRectangle { CornerRadius = new CornerRadius(7) });

            BUTTONSTYLE.Setters.Add(Border.StrokeProperty, Brush.Gray);
            BUTTONSTYLE.Setters.Add(Border.StrokeThicknessProperty, 1.5d);
            BUTTONSTYLE.Setters.Add(BackgroundProperty, Brush.LightGray);
            BUTTONSTYLE.Setters.Add(Border.StrokeShapeProperty, new RoundRectangle { CornerRadius = new CornerRadius(10) });
            BUTTONSTYLE.Setters.Add(HeightRequestProperty, 25d);

            BUTTONPATHSTYLE.Setters.Add(Path.StrokeProperty, Brush.Black);
            BUTTONPATHSTYLE.Setters.Add(Path.FillProperty, Brush.Black);

            STYLESINITIATED = true;
        }
    }
}