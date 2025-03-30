using Avalonia.Controls;
using CatalogoGatos.ViewModel;


namespace CatalogoGatos.View;

public partial class MainView : Window
{
    public MainView()
    {
        InitializeComponent();
    }
    
    private void Window_OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.GuardarCommand.Execute(null);
        }
    }
    
}