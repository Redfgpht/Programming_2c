using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Contacts.Model;
using Contacts.Model.Services;

namespace Contacts.ViewModel;

/// <summary>
/// ViewModel главного окна приложения.
/// Управляет списком контактов, режимами просмотра/редактирования/добавления и сохранением данных.
/// </summary>
public partial class MainVM : ObservableObject
{
    private readonly ContactSerializer _serializer = new();

    /// <summary>Коллекция всех контактов, отображаемая в списке.</summary>
    [ObservableProperty]
    private ObservableCollection<Contact> _contacts;

    /// <summary>Выбранный в списке контакт (в режиме просмотра).</summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
    private Contact? _selectedContact;

    // Поля для управления редактированием/добавлением
    private Contact? _editingContact;
    private Contact? _originalContact;

    /// <summary>
    /// Активен ли режим редактирования существующего контакта.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentContact))]
    [NotifyPropertyChangedFor(nameof(IsEditingOrAdding))]
    [NotifyPropertyChangedFor(nameof(IsApplyEnabled))]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
    private bool _isEditing;

    /// <summary>
    /// Активен ли режим добавления нового контакта.
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentContact))]
    [NotifyPropertyChangedFor(nameof(IsEditingOrAdding))]
    [NotifyPropertyChangedFor(nameof(IsApplyEnabled))]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(RemoveCommand))]
    [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
    [NotifyCanExecuteChangedFor(nameof(ApplyCommand))]
    private bool _isAdding;

    /// <summary>
    /// Инициализирует новый экземпляр MainVM и загружает сохранённые контакты.
    /// </summary>
    public MainVM()
    {
        var loadedContacts = _serializer.LoadContacts();
        _contacts = new ObservableCollection<Contact>(loadedContacts);
    }

    /// <summary>
    /// Контакт, который в данный момент редактируется или добавляется.
    /// При смене экземпляра автоматически подписывается на его события
    /// для отслеживания изменений и валидации.
    /// </summary>
    public Contact? EditingContact
    {
        get => _editingContact;
        set
        {
            if (ReferenceEquals(_editingContact, value)) return;

            if (_editingContact != null)
            {
                _editingContact.ErrorsChanged -= OnEditingContactStateChanged;
                _editingContact.PropertyChanged -= OnEditingContactStateChanged;
            }

            _editingContact = value;

            if (_editingContact != null)
            {
                _editingContact.ErrorsChanged += OnEditingContactStateChanged;
                _editingContact.PropertyChanged += OnEditingContactStateChanged;
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentContact));
            RefreshApplyState();
        }
    }

    /// <summary>
    /// Текущий отображаемый контакт: редактируемый/добавляемый или выбранный.
    /// </summary>
    public Contact? CurrentContact => IsEditingOrAdding ? EditingContact : SelectedContact;

    /// <summary>
    /// Режим редактирования или добавления активен.
    /// Используется в представлении для управления видимостью и доступностью элементов.
    /// Инвертирование и преобразование в Visibility выполняется через конвертеры.
    /// </summary>
    public bool IsEditingOrAdding => IsEditing || IsAdding;

    /// <summary>
    /// Доступность кнопки «Применить».
    /// В режиме редактирования — всегда доступна.
    /// В режиме добавления — зависит от заполнения обязательных полей и отсутствия ошибок валидации.
    /// </summary>
    public bool IsApplyEnabled
    {
        get
        {
            if (IsEditing)
                return true;

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

    /// <summary>
    /// Команда добавления нового контакта.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanAddOrEdit))]
    private void Add()
    {
        EditingContact = new Contact();
        IsAdding = true;
        SelectedContact = null;
        _originalContact = null;
    }

    /// <summary>
    /// Команда редактирования выбранного контакта.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanEdit))]
    private void Edit()
    {
        if (SelectedContact != null)
        {
            _originalContact = SelectedContact;
            EditingContact = new Contact(SelectedContact);   // Используется конструктор копирования
            IsEditing = true;
        }
    }

    /// <summary>
    /// Команда удаления выбранного контакта.
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRemove))]
    private void Remove()
    {
        if (SelectedContact == null) return;

        int index = Contacts.IndexOf(SelectedContact);
        Contacts.Remove(SelectedContact);

        SelectedContact = Contacts.Count > 0
            ? (index < Contacts.Count ? Contacts[index] : Contacts[^1])
            : null;

        SaveContacts();
    }

    /// <summary>
    /// Команда применения изменений (сохранения добавленного или отредактированного контакта).
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanApply))]
    private void Apply()
    {
        if (EditingContact == null)
        {
            Cancel();
            return;
        }

        if (IsAdding)
        {
            Contacts.Add(EditingContact);
            SelectedContact = EditingContact;
        }
        else if (IsEditing && _originalContact != null)
        {
            _originalContact.Name = EditingContact.Name;
            _originalContact.PhoneNumber = EditingContact.PhoneNumber;
            _originalContact.Email = EditingContact.Email;
            _originalContact.PhotoData = EditingContact.PhotoData;
            SelectedContact = _originalContact;
        }

        SaveContacts();

        IsEditing = false;
        IsAdding = false;
        EditingContact = null;
        _originalContact = null;
    }

    /// <summary>Команда отмены редактирования или добавления.</summary>
    [RelayCommand(CanExecute = nameof(CanCancel))]
    private void Cancel()
    {
        IsEditing = false;
        IsAdding = false;
        EditingContact = null;
        _originalContact = null;
    }

    private bool CanAddOrEdit() => !IsEditingOrAdding;
    private bool CanEdit() => !IsEditingOrAdding && SelectedContact != null;
    private bool CanRemove() => !IsEditingOrAdding && SelectedContact != null;
    private bool CanApply() => IsApplyEnabled;
    private bool CanCancel() => IsEditingOrAdding;

    private void SaveContacts() => _serializer.SaveContacts(Contacts);

    private void OnEditingContactStateChanged(object? sender, EventArgs e)
        => RefreshApplyState();

    private void RefreshApplyState()
    {
        OnPropertyChanged(nameof(IsApplyEnabled));
        ApplyCommand.NotifyCanExecuteChanged();
    }
}