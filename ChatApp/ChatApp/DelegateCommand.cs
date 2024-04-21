using System.Windows.Input;

namespace ChatApp;

public class DelegateCommand : ICommand
{
    private readonly Func<bool> canExecute;
    private readonly Action execute;

    public DelegateCommand(Action execute)
        : this(execute, () => true)
    {
    }

    private DelegateCommand(Action execute,
        Func<bool> canExecute)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

   

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return canExecute.Invoke();
    }

    public void Execute(object? parameter)
    {
        execute();
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}