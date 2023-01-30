Spinner.MAUI

A plugin for .NET MAUI providing a Spinner control.

For use the control after install de nuget package follow this steps:

XAML:

Add de namespace
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:sc="clr-namespace:Spinner.MAUI;assembly=Spinner.MAUI"
             x:Class="Spinner.MAUI.Test.MainPage">
  
Use it:
        <Border HorizontalOptions="Center" VerticalOptions="Center" Grid.Row="1" Margin="10" Background="white">
            <Grid>
                <Border VerticalOptions="Center" HeightRequest="30" Stroke="gray" Background="#EEEEEE" Margin="2,0,2,0" StrokeShape="RoundRectangle 10">
                    <Label/>
                </Border>
                <HorizontalStackLayout VerticalOptions="Center" BindingContext="{x:Reference spn1}" HeightRequest="120">
                    <sc:Spinner x:Name="spn1" BindingContext="{x:Reference mainpage}" Items="{Binding Hours}" SelectedItemIndex="{Binding Hour}" VerticalOptions="Center" WidthRequest="60" IsCyclic="True" SelectionBoxIsVisible="False" ItemStyle="{StaticResource Key=SpinnerTimeItemStyle}"/>
                    <Label VerticalOptions="Center" Text=":" FontSize="25" FontAttributes="Bold" TextColor="Black"/>
                    <sc:Spinner BindingContext="{x:Reference mainpage}" Items="{Binding Minutes}" SelectedItemIndex="{Binding Minute}" VerticalOptions="Center" WidthRequest="60" IsCyclic="True" SelectionBoxIsVisible="False" ItemStyle="{StaticResource Key=SpinnerTimeItemStyle}"/>
                    <Label VerticalOptions="Center" Text=":" FontSize="25" FontAttributes="Bold" TextColor="Black"/>
                    <sc:Spinner BindingContext="{x:Reference mainpage}" Items="{Binding Seconds}" SelectedItemIndex="{Binding Second}" VerticalOptions="Center" WidthRequest="60" IsCyclic="True" SelectionBoxIsVisible="False" ItemStyle="{StaticResource Key=SpinnerTimeItemStyle}"/>
                </HorizontalStackLayout>
            </Grid>
        </Border>

Code:
  
Add the using command:
  
using Spinner.MAUI;
  
Use it:

Spinner spinner=new Spinner { NumItemsToShow = 5, WidthRequest = 200 , HeightRequest = 150 };

  
