using System.ComponentModel;
using System.Runtime.CompilerServices;
using View.Model;
using View.Model.Services;
using System.Windows.Input;

namespace View.ViewModel
{
    /// <summary>
    /// ViewModel для главного окна.
    /// </summary>
    public class MainVM : INotifyPropertyChanged
    {
        /// <summary>
        /// Текущий контакт.
        /// </summary>
        private Contact _currentContact;

        private readonly ContactSerializer _serializer;

        /// <summary>
        /// Команда сохранения контакта.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Команда загрузки контакта.
        /// </summary>
        public ICommand LoadCommand { get; }

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public MainVM()
        {
            _currentContact = new Contact();
            _serializer = new ContactSerializer();

            SaveCommand = new SaveCommand(this);
            LoadCommand = new LoadCommand(this);
        }

        /// <summary>
        /// Сохраняет текущий контакт в файл.
        /// </summary>
        private void SaveContact(object? parameter)
        {
            _serializer.SaveContact(_currentContact);
        }

        /// <summary>
        /// Загружает контакт из файла.
        /// </summary>
        private void LoadContact(object? parameter)
        {
            var loadedContact = _serializer.LoadContact();
            SetContact(loadedContact);
        }

        /// <summary>
        /// Имя контакта.
        /// </summary>
        public string Name
        {
            get => _currentContact.Name;
            set
            {
                if (_currentContact.Name != value)
                {
                    _currentContact.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Номер телефона контакта.
        /// </summary>
        public string PhoneNumber
        {
            get => _currentContact.PhoneNumber;
            set
            {
                if (_currentContact.PhoneNumber != value)
                {
                    _currentContact.PhoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Email контакта.
        /// </summary>
        public string Email
        {
            get => _currentContact.Email;
            set
            {
                if (_currentContact.Email != value)
                {
                    _currentContact.Email = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Получает текущий объект контакта.
        /// </summary>
        public Contact GetContact()
        {
            return _currentContact;
        }

        /// <summary>
        /// Устанавливает новый объект контакта и обновляет свойства.
        /// </summary>
        public void SetContact(Contact contact)
        {
            _currentContact = contact ?? new Contact();

            // Уведомляем об изменении всех свойств
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(PhoneNumber));
            OnPropertyChanged(nameof(Email));
        }

        /// <summary>
        /// Очищает все поля контакта.
        /// </summary>
        public void ClearContact()
        {
            _currentContact.Name = string.Empty;
            _currentContact.PhoneNumber = string.Empty;
            _currentContact.Email = string.Empty;

            // Уведомляем об изменении всех свойств
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(PhoneNumber));
            OnPropertyChanged(nameof(Email));
        }

        /// <summary>
        /// Создание события PropertyChanged.
        /// </summary>
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