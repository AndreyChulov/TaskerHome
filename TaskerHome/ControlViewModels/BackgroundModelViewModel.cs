using TaskerHome.ViewModels;

namespace TaskerHome.ControlViewModels;

public class BackgroundModelViewModel : ViewModelBase
{
    private bool _isSpinning = true;
    public bool IsSpinning
    {
        get => _isSpinning;
        set => SetProperty(ref _isSpinning, value);
    }
}