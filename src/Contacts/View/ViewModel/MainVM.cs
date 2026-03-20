using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using View.Model;
using View.Model.Services;

namespace View.ViewModel
{
    /// <summary>
    /// ViewModel для главного окна.
    /// </summary>
    public class MainVM : INotifyPropertyChanged
    {
        private readonly ContactSerializer _serializer;
        private ObservableCollection<Contact> _contacts;
        private Contact? _selectedContact;
        private Contact? _editingContact;
        private bool _isEditing;
        private bool _isAdding;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public MainVM()
        {
            _serializer = new ContactSerializer();

            // Загружаем контакты
            var loadedContacts = _serializer.LoadContacts();
            _contacts = new ObservableCollection<Contact>(loadedContacts);

            // Подписываемся на изменения свойств контактов
            foreach (var contact in _contacts)
            {
                contact.PropertyChanged += Contact_PropertyChanged;
            }

            // Инициализация команд
            AddCommand = new RelayCommand(AddContact, CanAddOrEdit);
            EditCommand = new RelayCommand(EditContact, CanEdit);
            RemoveCommand = new RelayCommand(RemoveContact, CanRemove);
            ApplyCommand = new RelayCommand(ApplyChanges, CanApply);
            CancelCommand = new RelayCommand(CancelEditing, CanCancel);

            // Обновляем команды при запуске
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Коллекция контактов.
        /// </summary>
        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set
            {
                _contacts = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Выбранный контакт.
        /// </summary>
        public Contact? SelectedContact
        {
            get => _selectedContact;
            set
            {
                if (_selectedContact != value)
                {
                    // Если были изменения, отменяем их
                    if (_isEditing || _isAdding)
                    {
                        CancelEditing();
                    }

                    _selectedContact = value;
                    OnPropertyChanged();

                    OnPropertyChanged(nameof(CurrentContact));

                    // Обновляем состояние команд
                    (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Текущий отображаемый контакт.
        /// </summary>
        public Contact? CurrentContact
        {
            get
            {
                if (_isEditing || _isAdding)
                    return _editingContact;
                return _selectedContact;
            }
        }

        /// <summary>
        /// Контакт, который редактируется в данный момент.
        /// </summary>
        public Contact? EditingContact
        {
            get => _editingContact;
            set
            {
                if (_editingContact != value)
                {
                    // Отписываемся от старого контакта
                    if (_editingContact != null)
                    {
                        _editingContact.PropertyChanged -= EditingContact_PropertyChanged;
                    }

                    _editingContact = value;

                    // Подписываемся на изменения нового контакта
                    if (_editingContact != null)
                    {
                        _editingContact.PropertyChanged += EditingContact_PropertyChanged;
                    }

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentContact));
                    OnPropertyChanged(nameof(IsApplyEnabled));
                    (ApplyCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Обработчик изменений редактируемого контакта
        /// </summary>
        private void EditingContact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(IsApplyEnabled));
            (ApplyCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Режим редактирования.
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEditingOrAdding));
                OnPropertyChanged(nameof(IsNotEditingOrAdding));
                OnPropertyChanged(nameof(IsEditingMode));
                OnPropertyChanged(nameof(IsViewMode));
                OnPropertyChanged(nameof(IsApplyVisible));
                OnPropertyChanged(nameof(IsApplyEnabled));
                OnPropertyChanged(nameof(CurrentContact));

                // Обновляем команды при изменении режима
                (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ApplyCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Режим добавления.
        /// </summary>
        public bool IsAdding
        {
            get => _isAdding;
            set
            {
                _isAdding = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsEditingOrAdding));
                OnPropertyChanged(nameof(IsNotEditingOrAdding));
                OnPropertyChanged(nameof(IsEditingMode));
                OnPropertyChanged(nameof(IsViewMode));
                OnPropertyChanged(nameof(IsApplyVisible));
                OnPropertyChanged(nameof(IsApplyEnabled));
                OnPropertyChanged(nameof(CurrentContact));

                // Обновляем команды при изменении режима
                (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (ApplyCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Находится ли приложение в режиме редактирования или добавления.
        /// </summary>
        public bool IsEditingOrAdding => _isEditing || _isAdding;

        /// <summary>
        /// Не находится ли приложение в режиме редактирования или добавления.
        /// </summary>
        public bool IsNotEditingOrAdding => !(_isEditing || _isAdding);

        /// <summary>
        /// Режим редактирования.
        /// </summary>
        public bool IsEditingMode => _isEditing || _isAdding;

        /// <summary>
        /// Режим просмотра.
        /// </summary>
        public bool IsViewMode => !(_isEditing || _isAdding);

        /// <summary>
        /// Видимость кнопок Apply и Cancel.
        /// </summary>
        public bool IsApplyVisible => _isEditing || _isAdding;

        /// <summary>
        /// Доступность кнопки Apply
        /// </summary>
        public bool IsApplyEnabled
        {
            get
            {
                // Если режим редактирования - кнопка всегда доступна
                if (_isEditing)
                    return true;

                // Если режим добавления - проверяем заполненность полей и отсутствие ошибок
                if (_isAdding && _editingContact != null)
                {
                    return !string.IsNullOrWhiteSpace(_editingContact.Name) &&
                           !string.IsNullOrWhiteSpace(_editingContact.PhoneNumber) &&
                           !string.IsNullOrWhiteSpace(_editingContact.Email) &&
                           !_editingContact.HasErrors;
                }

                return false;
            }
        }

        // Команды
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Обработчик изменения свойств контакта.
        /// </summary>
        private void Contact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            _serializer.SaveContacts(_contacts);
        }

        /// <summary>
        /// Команда добавления контакта.
        /// </summary>
        private void AddContact(object? parameter)
        {
            // Создаем временный контакт для редактирования
            EditingContact = new Contact();
            IsAdding = true;

            // Снимаем выделение
            SelectedContact = null;

            // Обновляем команды
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Команда редактирования контакта.
        /// </summary>
        private void EditContact(object? parameter)
        {
            if (_selectedContact != null)
            {
                // Создаем копию контакта для редактирования
                EditingContact = new Contact
                {
                    Name = _selectedContact.Name,
                    PhoneNumber = _selectedContact.PhoneNumber,
                    Email = _selectedContact.Email,
                    PhotoPath = _selectedContact.PhotoPath
                };
                IsEditing = true;

                // Обновляем команды
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Команда удаления контакта.
        /// </summary>
        private void RemoveContact(object? parameter)
        {
            if (_selectedContact != null)
            {
                int selectedIndex = _contacts.IndexOf(_selectedContact);

                // Отписываемся от событий удаляемого контакта
                _selectedContact.PropertyChanged -= Contact_PropertyChanged;

                _contacts.Remove(_selectedContact);

                // Устанавливаем выделение на следующий контакт
                if (_contacts.Count > 0)
                {
                    if (selectedIndex < _contacts.Count)
                    {
                        SelectedContact = _contacts[selectedIndex];
                    }
                    else
                    {
                        SelectedContact = _contacts[_contacts.Count - 1];
                    }
                }
                else
                {
                    SelectedContact = null;
                }

                _serializer.SaveContacts(_contacts);

                // Обновляем команды
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Проверяет, можно ли применить изменения.
        /// </summary>
        private bool CanApply(object? parameter)
        {
            return IsApplyEnabled;
        }

        /// <summary>
        /// Команда применения изменений.
        /// </summary>
        private void ApplyChanges(object? parameter)
        {
            // Проверяем, что есть что редактировать
            if (_editingContact == null)
            {
                CancelEditing();
                return;
            }

            // Для режима добавления проверяем заполненность полей и отсутствие ошибок
            if (_isAdding && (!IsContactValid(_editingContact) || _editingContact.HasErrors))
            {
                return;
            }

            if (_isAdding)
            {
                // Подписываемся на изменения нового контакта
                _editingContact.PropertyChanged += Contact_PropertyChanged;

                _contacts.Add(_editingContact);
                SelectedContact = _editingContact;
            }
            else if (_isEditing && _selectedContact != null)
            {
                // Отписываемся от старого контакта перед изменением
                _selectedContact.PropertyChanged -= Contact_PropertyChanged;

                _selectedContact.Name = _editingContact.Name;
                _selectedContact.PhoneNumber = _editingContact.PhoneNumber;
                _selectedContact.Email = _editingContact.Email;
                _selectedContact.PhotoPath = _editingContact.PhotoPath;

                // Подписываемся снова
                _selectedContact.PropertyChanged += Contact_PropertyChanged;
            }

            _serializer.SaveContacts(_contacts);

            // Выходим из режима редактирования
            IsEditing = false;
            IsAdding = false;
            EditingContact = null;

            // Обновляем команды
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Команда отмены редактирования.
        /// </summary>
        private void CancelEditing(object? parameter = null)
        {
            if (_isEditing || _isAdding)
            {
                IsEditing = false;
                IsAdding = false;
                EditingContact = null;

                // Обновляем команды
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Проверяет, можно ли добавить/редактировать контакт.
        /// </summary>
        private bool CanAddOrEdit(object? parameter)
        {
            return !_isEditing && !_isAdding;
        }

        /// <summary>
        /// Проверяет, можно ли редактировать контакт.
        /// </summary>
        private bool CanEdit(object? parameter)
        {
            bool canEdit = !_isEditing && !_isAdding && _selectedContact != null;
            System.Diagnostics.Debug.WriteLine($"CanEdit: {canEdit}, IsEditing={_isEditing}, " +
                $"IsAdding={_isAdding}, SelectedContact={_selectedContact != null}");
            return canEdit;
        }

        /// <summary>
        /// Проверяет, можно ли удалить контакт.
        /// </summary>
        private bool CanRemove(object? parameter)
        {
            bool canRemove = !_isEditing && !_isAdding && _selectedContact != null;
            System.Diagnostics.Debug.WriteLine($"CanRemove: {canRemove}, IsEditing={_isEditing}, " +
                $"IsAdding={_isAdding}, SelectedContact={_selectedContact != null}");
            return canRemove;
        }

        /// <summary>
        /// Проверяет, можно ли отменить редактирование.
        /// </summary>
        private bool CanCancel(object? parameter)
        {
            return _isEditing || _isAdding;
        }

        /// <summary>
        /// Проверяет, заполнены ли все поля контакта
        /// </summary>
        private bool IsContactValid(Contact contact)
        {
            return !string.IsNullOrWhiteSpace(contact.Name) &&
                   !string.IsNullOrWhiteSpace(contact.PhoneNumber) &&
                   !string.IsNullOrWhiteSpace(contact.Email);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}