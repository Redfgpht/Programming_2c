using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Contacts.Model;
using Contacts.Model.Services;

namespace Contacts.ViewModel
{
    /// <summary>
    /// ViewModel для главного окна приложения.
    /// </summary>
    public partial class MainVM : ObservableObject
    {
        #region Private Fields

        /// <summary>
        /// Сериализатор для сохранения и загрузки контактов.
        /// </summary>
        private readonly ContactSerializer _serializer;

        [ObservableProperty]
        private ObservableCollection<Contact> _contacts;

        private Contact? _selectedContact;
        // TODO: чем редактируемый контакт отличается от выбранного? Должны быть поясняющие комментарии.
        private Contact? _editingContact;
        private Contact? _originalEditingContact;

        private bool _isEditing;
        private bool _isAdding;

        #endregion

        #region Constructors

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="MainVM"/>.
        /// Загружает список контактов из хранилища и подписывается на изменения свойств каждого контакта.
        /// </summary>
        public MainVM()
        {
            // TODO: инициализацию вынести в поле
            _serializer = new ContactSerializer();

            var loadedContacts = _serializer.LoadContacts();
            _contacts = new ObservableCollection<Contact>(loadedContacts);

            foreach (var contact in _contacts)
            {
                contact.PropertyChanged += Contact_PropertyChanged;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Выбранный в списке контакт.
        /// </summary>
        public Contact? SelectedContact
        {
            get => _selectedContact;
            set
            {
                // TODO: инвертировать условие, надо стараться уменьшать вложенность логики
                if (_selectedContact != value)
                {
                    _selectedContact = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentContact));
                    // TODO: есть автоматические механизмы по обновлению состояния команд,
                    // используй их. Ручного вызова здесь и нигде дальше быть не должно.
                    (EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Указывает, находится ли приложение в режиме редактирования существующего контакта.
        /// </summary>
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;

                    // TODO: таких портянок с обновлениями быть не должно.
                    // Почему обновилось одно свойство, а ты сообщаешь об обновлении 8 объектов?
                    // Исправить. Здесь и везде ниже
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsEditingOrAdding));
                    OnPropertyChanged(nameof(IsNotEditingOrAdding));
                    OnPropertyChanged(nameof(IsEditingMode));
                    OnPropertyChanged(nameof(IsViewMode));
                    OnPropertyChanged(nameof(IsApplyVisible));
                    OnPropertyChanged(nameof(IsApplyEnabled));
                    OnPropertyChanged(nameof(CurrentContact));

                    (AddCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (CancelCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Указывает, находится ли приложение в режиме добавления нового контакта.
        /// </summary>
        public bool IsAdding
        {
            get => _isAdding;
            set
            {
                if (_isAdding != value)
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

                    (AddCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (EditCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (RemoveCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (CancelCommand as RelayCommand)?.NotifyCanExecuteChanged();
                    (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Контакт, который в данный момент редактируется или добавляется.
        /// </summary>
        public Contact? EditingContact
        {
            get => _editingContact;
            set
            {
                if (_editingContact != value)
                {
                    if (_editingContact != null)
                        _editingContact.PropertyChanged -= EditingContact_PropertyChanged;

                    _editingContact = value;

                    if (_editingContact != null)
                        _editingContact.PropertyChanged += EditingContact_PropertyChanged;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CurrentContact));
                    OnPropertyChanged(nameof(IsApplyEnabled));
                    (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Текущий отображаемый контакт: редактируемый/добавляемый или выбранный в зависимости от режима.
        /// </summary>
        public Contact? CurrentContact => IsEditing || IsAdding ? EditingContact : SelectedContact;

        // TODO: слишком много булевых флагов ниже. Достаточно только двух.
        // Если в View тебе понадобятся инвертированные значения, это можно сделать конвертерами
        /// <summary>
        /// Указывает, выполняется ли редактирование или добавление контакта.
        /// </summary>
        public bool IsEditingOrAdding => IsEditing || IsAdding;

        /// <summary>
        /// Указывает, не выполняется ли редактирование и не добавляется ли контакт.
        /// </summary>
        public bool IsNotEditingOrAdding => !(IsEditing || IsAdding);

        /// <summary>
        /// Указывает, находится ли приложение в режиме редактирования или добавления.
        /// </summary>
        public bool IsEditingMode => IsEditing || IsAdding;

        /// <summary>
        /// Указывает, находится ли приложение в режиме просмотра (не редактирование и не добавление).
        /// </summary>
        public bool IsViewMode => !(IsEditing || IsAdding);

        /// <summary>
        /// Определяет, видима ли кнопка применения изменений (видна в режимах редактирования или добавления).
        /// </summary>
        public bool IsApplyVisible => IsEditing || IsAdding;

        /// <summary>
        /// Определяет, доступна ли кнопка применения изменений.
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

        #endregion

        #region Commands

        /// <summary>
        /// Команда для добавления нового контакта.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanAddOrEdit))]
        private void Add()
        {
            EditingContact = new Contact();
            IsAdding = true;
            SelectedContact = null;
            _originalEditingContact = null;
        }

        /// <summary>
        /// Команда для редактирования выбранного контакта.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanEdit))]
        private void Edit()
        {
            if (SelectedContact != null)
            {
                _originalEditingContact = SelectedContact;
                // TODO: сделать конструктор копирования или ICloneable
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

        /// <summary>
        /// Команда для удаления выбранного контакта.
        /// </summary>
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

        /// <summary>
        /// Команда для применения изменений (сохранения добавленного или отредактированного контакта).
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanApply))]
        private void Apply()
        {
            // TODO: как-то всё слишком сложно.
            // Команда Apply срабатывает при условии, что всё введено корректно.
            // Значит, надо просто добавить новый контакт/заменить старый контакт в списке.
            // А здесь какая-то куча проверок. Исправить
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
            else if (IsEditing && _originalEditingContact != null)
            {
                _originalEditingContact.PropertyChanged -= Contact_PropertyChanged;
                _originalEditingContact.Name = EditingContact.Name;
                _originalEditingContact.PhoneNumber = EditingContact.PhoneNumber;
                _originalEditingContact.Email = EditingContact.Email;
                _originalEditingContact.PhotoPath = EditingContact.PhotoPath;
                _originalEditingContact.PropertyChanged += Contact_PropertyChanged;
                SelectedContact = _originalEditingContact;
            }

            _serializer.SaveContacts(Contacts);

            IsEditing = false;
            IsAdding = false;
            EditingContact = null;
            _originalEditingContact = null;
        }

        /// <summary>
        /// Команда для отмены редактирования или добавления.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel()
        {
            if (IsEditing || IsAdding)
            {
                IsEditing = false;
                IsAdding = false;
                EditingContact = null;
                _originalEditingContact = null;
            }
        }

        #endregion

        #region CanExecute Methods

        /// <summary>
        /// Определяет, может ли выполняться команда добавления.
        /// </summary>
        private bool CanAddOrEdit() => !IsEditing && !IsAdding;

        /// <summary>
        /// Определяет, может ли выполняться команда редактирования.
        /// </summary>
        private bool CanEdit() => !IsEditing && !IsAdding && SelectedContact != null;

        /// <summary>
        /// Определяет, может ли выполняться команда удаления.
        /// </summary>
        private bool CanRemove() => !IsEditing && !IsAdding && SelectedContact != null;

        /// <summary>
        /// Определяет, может ли выполняться команда применения.
        /// </summary>
        private bool CanApply() => IsApplyEnabled;

        /// <summary>
        /// Определяет, может ли выполняться команда отмены.
        /// </summary>
        private bool CanCancel() => IsEditing || IsAdding;

        #endregion

        #region Private Methods

        /// <summary>
        /// Обработчик изменения свойств контакта. Сохраняет изменения в хранилище.
        /// </summary>
        private void Contact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // TODO: не должно быть такого обработчика, который на изменение любого поля в контакте
            // пересохраняет все контакты в файле. Это же не рационально!
            // Сохранение должно вызываться напрямую в конкретных методах - закрытие программы (корректное и некорректное) и Apply
            _serializer.SaveContacts(Contacts);
        }

        /// <summary>
        /// Обработчик изменения свойств редактируемого контакта. Обновляет состояние кнопки применения.
        /// </summary>
        private void EditingContact_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // TODO: лишний обработчик, переделать без него
            OnPropertyChanged(nameof(IsApplyEnabled));
            (ApplyCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        // TODO: Проверка валидности контакта - не должно быть обязанностью MainVM. Вынести в отдельный класс.
        // TODO: Название метода говорит о проверки валидности контакта,
        // но здесь проверяется только не пустые строки, а валидация контакта - гораздо шире.
        // Метод неправильно назван или метод неправильно делает валидацию?
        // Разве класс контакта уже не делает валидацию самого себя? Почему не использовать информацию об ошибках из самого объекта?
        /// <summary>
        /// Проверяет, заполнены ли обязательные поля контакта.
        /// </summary>
        /// <param name="contact">Проверяемый контакт.</param>
        /// <returns>True, если все обязательные поля заполнены, иначе false.</returns>
        private bool IsContactValid(Contact contact)
        {
            return !string.IsNullOrWhiteSpace(contact.Name) &&
                   !string.IsNullOrWhiteSpace(contact.PhoneNumber) &&
                   !string.IsNullOrWhiteSpace(contact.Email);
        }

        #endregion
    }
}