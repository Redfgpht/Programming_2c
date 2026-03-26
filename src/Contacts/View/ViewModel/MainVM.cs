using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Contacts.Model;
using Contacts.Model.Services;
using System.Collections.ObjectModel;

namespace Contacts.ViewModel
{
    /// <summary>
    /// ViewModel для главного окна.
    /// </summary>
    public partial class MainVM : ObservableObject
    {
        private readonly ContactSerializer _serializer;

        [ObservableProperty]
        private ObservableCollection<Contact> _contacts;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentContact))]
        private Contact? _selectedContact;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentContact))]
        private Contact? _editingContact;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        private bool _isEditing;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddCommand))]
        [NotifyCanExecuteChangedFor(nameof(EditCommand))]
        [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        private bool _isAdding;

        public MainVM()
        {
            _serializer = new ContactSerializer();

            var loadedContacts = _serializer.LoadContacts();
            _contacts = new ObservableCollection<Contact>(loadedContacts);

            foreach (var contact in _contacts)
            {
                contact.PropertyChanged += Contact_PropertyChanged;
            }
        }

        public Contact? CurrentContact => IsEditing || IsAdding ? EditingContact : SelectedContact;

        public bool IsEditingOrAdding => IsEditing || IsAdding;
        public bool IsNotEditingOrAdding => !(IsEditing || IsAdding);
        public bool IsEditingMode => IsEditing || IsAdding;
        public bool IsViewMode => !(IsEditing || IsAdding);
        public bool IsApplyVisible => IsEditing || IsAdding;

        public bool IsApplyEnabled
        {
            get
            {
                if (IsEditing) return true;
                if (IsAdding && EditingContact != null)
                {
                    return !string.IsNullOrWhiteSpace(EditingContact.Name) &&
                           !string.IsNullOrWhiteSpace(EditingContact.PhoneNumber) &&
                           !string.IsNullOrWhiteSpace(EditingContact.Email) &&
                           !EditingContact.HasErrors;
                }
                return false;
            }
        }

        // Команды
        [RelayCommand(CanExecute = nameof(CanAddOrEdit))]
        private void Add()
        {
            EditingContact = new Contact();
            IsAdding = true;
            SelectedContact = null;
        }

        [RelayCommand(CanExecute = nameof(CanEdit))]
        private void Edit()
        {
            if (SelectedContact != null)
            {
                EditingContact = new Contact
                {
                    Name = SelectedContact.Name,
                    PhoneNumber = SelectedContact.PhoneNumber,
                    Email = SelectedContact.Email,
                    PhotoPath = SelectedContact.PhotoPath
                };
                IsEditing = true;
            }
        }

        [RelayCommand(CanExecute = nameof(CanRemove))]
        private void Remove()
        {
            if (SelectedContact != null)
            {
                int selectedIndex = Contacts.IndexOf(SelectedContact);
                SelectedContact.PropertyChanged -= Contact_PropertyChanged;
                Contacts.Remove(SelectedContact);

                if (Contacts.Count > 0)
                {
                    SelectedContact = selectedIndex < Contacts.Count
                        ? Contacts[selectedIndex]
                        : Contacts[Contacts.Count - 1];
                }
                else
                {
                    SelectedContact = null;
                }

                _serializer.SaveContacts(Contacts);
            }
        }

        [RelayCommand(CanExecute = nameof(CanApply))]
        private void Apply()
        {
            if (EditingContact == null)
            {
                Cancel();
                return;
            }

            if (IsAdding && (!IsContactValid(EditingContact) || EditingContact.HasErrors))
            {
                return;
            }

            if (IsAdding)
            {
                EditingContact.PropertyChanged += Contact_PropertyChanged;
                Contacts.Add(EditingContact);
                SelectedContact = EditingContact;
            }
            else if (IsEditing && SelectedContact != null)
            {
                SelectedContact.PropertyChanged -= Contact_PropertyChanged;
                SelectedContact.Name = EditingContact.Name;
                SelectedContact.PhoneNumber = EditingContact.PhoneNumber;
                SelectedContact.Email = EditingContact.Email;
                SelectedContact.PhotoPath = EditingContact.PhotoPath;
                SelectedContact.PropertyChanged += Contact_PropertyChanged;
            }

            _serializer.SaveContacts(Contacts);

            IsEditing = false;
            IsAdding = false;
            EditingContact = null;
        }

        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
            if (IsEditing || IsAdding)
            {
                IsEditing = false;
                IsAdding = false;
                EditingContact = null;
            }
        }

        // Методы CanExecute
        private bool CanAddOrEdit() => !IsEditing && !IsAdding;
        private bool CanEdit() => !IsEditing && !IsAdding && SelectedContact != null;
        private bool CanRemove() => !IsEditing && !IsAdding && SelectedContact != null;
        private bool CanApply() => IsApplyEnabled;
        private bool CanCancel() => IsEditing || IsAdding;

        // Вспомогательные методы
        private void Contact_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            _serializer.SaveContacts(Contacts);
        }

        private bool IsContactValid(Contact contact)
        {
            return !string.IsNullOrWhiteSpace(contact.Name) &&
                   !string.IsNullOrWhiteSpace(contact.PhoneNumber) &&
                   !string.IsNullOrWhiteSpace(contact.Email);
        }
    }
}