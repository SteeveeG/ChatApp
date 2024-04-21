using System.Windows;
using System.Windows.Controls;
using ChatApp.Contact.NewContact;

namespace ChatApp.Contact;

public partial class ContactControl : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ContactViewModel), typeof(ContactControl), new PropertyMetadata(default(ContactViewModel)));

    public ContactControl()
    {
        InitializeComponent();
    }

    public ContactViewModel ViewModel
    {
        get => (ContactViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private void OpenNewContactWindow(object sender, RoutedEventArgs e)
    {
        var newContact = new NewContact.NewContact();
        newContact.ViewModel = new NewContactViewModel(ViewModel.AccUser , () => ViewModel.UpdateContactList());
        newContact.Show();
    }
}