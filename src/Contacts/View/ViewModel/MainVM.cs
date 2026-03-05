using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
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
            ApplyCommand = new RelayCommand(ApplyChanges, CanAlwaysExecute);
            CancelCommand = new RelayCommand(CancelEditing, CanCancel);
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

                    // Обновляем текущий контакт для отображения
                    OnPropertyChanged(nameof(CurrentContact));

                    // Обновляем состояние команд
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        /// <summary>
        /// Текущий отображаемый контакт (возвращает EditingContact в режиме редактирования, иначе SelectedContact)
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
                _editingContact = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentContact));
            }
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
                OnPropertyChanged(nameof(CurrentContact));

                // Обновляем команды при изменении режима
                (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
                OnPropertyChanged(nameof(CurrentContact));

                (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (RemoveCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (CancelCommand as RelayCommand)?.RaiseCanExecuteChanged();
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
        /// Режим редактирования (поля доступны для ввода).
        /// </summary>
        public bool IsEditingMode => _isEditing || _isAdding;

        /// <summary>
        /// Режим просмотра (поля только для чтения).
        /// </summary>
        public bool IsViewMode => !(_isEditing || _isAdding);

        /// <summary>
        /// Видимость кнопок Apply и Cancel.
        /// </summary>
        public bool IsApplyVisible => _isEditing || _isAdding;

        // Команды
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        /// <summary>
        /// Всегда возвращает true (кнопка всегда доступна)
        /// </summary>
        private bool CanAlwaysExecute(object? parameter) => true;

        /// <summary>
        /// Обработчик изменения свойств контакта
        /// </summary>
        private void Contact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // При изменении любого контакта сохраняем данные
            _serializer.SaveContacts(_contacts);
        }

        /// <summary>
        /// Команда добавления контакта.
        /// </summary>
        private void AddContact(object? parameter)
        {
            EditingContact = new Contact();
            IsAdding = true;

            SelectedContact = null;

            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Команда редактирования контакта.
        /// </summary>
        private void EditContact(object? parameter)
        {
            if (_selectedContact != null)
            {
                EditingContact = new Contact
                {
                    Name = _selectedContact.Name,
                    PhoneNumber = _selectedContact.PhoneNumber,
                    Email = _selectedContact.Email
                };
                IsEditing = true;

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
                CommandManager.InvalidateRequerySuggested();
            }
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

        /// <summary>
        /// Показывает сообщение об ошибке
        /// </summary>
        private void ShowValidationError()
        {
            MessageBox.Show(
                "Пожалуйста, заполните все поля (Имя, Телефон и Email)",
                "Ошибка ввода",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        /// <summary>
        /// Команда применения изменений.
        /// </summary>
        private void ApplyChanges(object? parameter)
        {
            if (_editingContact == null)
            {
                CancelEditing();
                return;
            }
            if (!IsContactValid(_editingContact))
            {
                ShowValidationError();
                return;
            }

            if (_isAdding)
            {
                // Подписываемся на изменения нового контакта
                _editingContact.PropertyChanged += Contact_PropertyChanged;

                // Добавляем новый контакт
                _contacts.Add(_editingContact);
                SelectedContact = _editingContact;
            }
            else if (_isEditing && _selectedContact != null)
            {
                // Отписываемся от старого контакта перед изменением
                _selectedContact.PropertyChanged -= Contact_PropertyChanged;

                // Обновляем существующий контакт
                _selectedContact.Name = _editingContact.Name;
                _selectedContact.PhoneNumber = _editingContact.PhoneNumber;
                _selectedContact.Email = _editingContact.Email;

                // Подписываемся снова
                _selectedContact.PropertyChanged += Contact_PropertyChanged;
            }

            _serializer.SaveContacts(_contacts);
            IsEditing = false;
            IsAdding = false;
            EditingContact = null;
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
                CommandManager.InvalidateRequerySuggested();
            }
        }

        /// <summary>
        /// Проверяет, можно ли добавить контакт.
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
            return !_isEditing && !_isAdding && _selectedContact != null;
        }

        /// <summary>
        /// Проверяет, можно ли удалить контакт.
        /// </summary>
        private bool CanRemove(object? parameter)
        {
            return !_isEditing && !_isAdding && _selectedContact != null;
        }

        /// <summary>
        /// Проверяет, можно ли отменить редактирование.
        /// </summary>
        private bool CanCancel(object? parameter)
        {
            return _isEditing || _isAdding;
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