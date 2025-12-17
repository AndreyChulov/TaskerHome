using Avalonia.Controls;
using TaskerHome.ViewModels;

namespace TaskerHome.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}