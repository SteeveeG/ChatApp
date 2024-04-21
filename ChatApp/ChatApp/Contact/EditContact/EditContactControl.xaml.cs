using System.Windows;
using System.Windows.Controls;

namespace ChatApp.Contact.EditContact;

public partial class EditContactControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(EditContactViewModel), typeof(EditContactControl), new PropertyMetadata(default(EditContactViewModel)));

    public EditContactControl()
    {
        InitializeComponent();
    }

    public EditContactViewModel ViewModel
    {
        get => (EditContactViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
    
    
    private void Click(object sender, RoutedEventArgs routedEventArgs)
    {
        var item = (Button)sender;
        ViewModel.Action((EditContactViewModel)item.DataContext);
    }
}