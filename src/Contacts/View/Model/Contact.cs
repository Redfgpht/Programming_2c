using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace View.Model
{
    /// <summary>
    /// Представляет модель контакта с валидацией данных.
    /// </summary>
    public class Contact : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        /// <summary>
        /// Максимальная длина текстовых полей (имя, телефон, email).
        /// </summary>
        private const int MaxLength = 100;

        /// <summary>
        /// Максимальная длина текстовых телефона.
        /// </summary>
        private const int MaxLengthPhone = 40;

        /// <summary>
        /// Имя контакта.
        /// </summary>
        private string _name;

        /// <summary>
        /// Номер телефона контакта.
        /// </summary>
        private string _phoneNumber;

        /// <summary>
        /// Email адрес контакта.
        /// </summary>
        private string _email;

        /// <summary>
        /// Путь к файлу фотографии контакта.
        /// </summary>
        private string _photoPath;

        /// <summary>
        /// Словарь для хранения ошибок валидации по свойствам.
        /// </summary>
        private readonly Dictionary<string, List<string>> _errors = new();

        /// <summary>
        /// Регулярное выражение для проверки номера телефона.
        /// </summary>
        private readonly Regex _phoneRegex = new(@"^[0-9+\-\(\)\s]*$");

        /// <summary>
        /// Регулярное выражение для проверки email адреса.
        /// </summary>
        private readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

        /// <summary>
        /// Инициализирует новый экземпляр класса Contact с пустыми значениями.
        /// </summary>
        public Contact()
        {
            _name = string.Empty;
            _phoneNumber = string.Empty;
            _email = string.Empty;
            _photoPath = string.Empty;

            ValidateAllProperties();
        }

        /// <summary>
        /// Инициализирует новый экземпляр класса Contact с указанными значениями.
        /// </summary>
        /// <param name="name">Имя контакта. Не может быть null.</param>
        /// <param name="phoneNumber">Номер телефона контакта. Не может быть null.</param>
        /// <param name="email">Email адрес контакта. Не может быть null.</param>
        /// <param name="photoPath">Путь к файлу фотографии контакта. Необязательный параметр.</param>
        public Contact(string name, string phoneNumber, string email, string photoPath = "")
        {
            _name = name ?? string.Empty;
            _phoneNumber = phoneNumber ?? string.Empty;
            _email = email ?? string.Empty;
            _photoPath = photoPath ?? string.Empty;

            ValidateAllProperties();
        }

        /// <summary>
        /// Получает или задает имя контакта.
        /// </summary>
        /// <exception cref="ValidationException">Возникает при попытке установить недопустимое значение.</exception>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    ValidateName();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Получает или задает номер телефона контакта.
        /// </summary>
        /// <exception cref="ValidationException">Возникает при попытке установить недопустимое значение.</exception>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    ValidatePhoneNumber();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Получает или задает email адрес контакта.
        /// </summary>
        /// <exception cref="ValidationException">Возникает при попытке установить недопустимое значение.</exception>
        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    ValidateEmail();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Получает или задает путь к файлу фотографии контакта.
        /// </summary>
        public string PhotoPath
        {
            get => _photoPath;
            set
            {
                if (_photoPath != value)
                {
                    _photoPath = value;
                    OnPropertyChanged();
                }
            }
        }

        #region Validation Methods

        /// <summary>
        /// Выполняет валидацию имени контакта.
        /// </summary>
        private void ValidateName()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
            {
                errors.Add("Имя не может быть пустым");
            }
            else if (Name.Length > MaxLength)
            {
                errors.Add($"Имя не может быть длиннее {MaxLength} символов");
            }

            UpdateErrors(nameof(Name), errors);
        }

        /// <summary>
        /// Выполняет валидацию номера телефона.
        /// </summary>
        private void ValidatePhoneNumber()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                errors.Add("Телефон не может быть пустым");
            }
            else
            {
                if (PhoneNumber.Length > MaxLengthPhone)
                {
                    errors.Add($"Телефон не может быть длиннее {MaxLengthPhone} символов");
                }

                if (!_phoneRegex.IsMatch(PhoneNumber))
                {
                    errors.Add("Телефон может содержать только цифры и символы + - ( )");
                }
            }

            UpdateErrors(nameof(PhoneNumber), errors);
        }

        /// <summary>
        /// Выполняет валидацию email адреса.
        /// </summary>
        private void ValidateEmail()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Email))
            {
                errors.Add("Email не может быть пустым");
            }
            else
            {
                if (Email.Length > MaxLength)
                {
                    errors.Add($"Email не может быть длиннее {MaxLength} символов");
                }

                if (!Email.Contains('@'))
                {
                    errors.Add("Email должен содержать символ @");
                }

                if (!_emailRegex.IsMatch(Email))
                {
                    errors.Add("Введите корректный email адрес");
                }
            }

            UpdateErrors(nameof(Email), errors);
        }

        /// <summary>
        /// Выполняет валидацию всех свойств контакта.
        /// </summary>
        private void ValidateAllProperties()
        {
            ValidateName();
            ValidatePhoneNumber();
            ValidateEmail();
        }

        /// <summary>
        /// Обновляет список ошибок для указанного свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства, для которого обновляются ошибки.</param>
        /// <param name="errors">Список ошибок для свойства.</param>
        private void UpdateErrors(string propertyName, List<string> errors)
        {
            if (errors.Any())
            {
                if (_errors.ContainsKey(propertyName))
                {
                    _errors[propertyName] = errors;
                }
                else
                {
                    _errors.Add(propertyName, errors);
                }
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            else
            {
                if (_errors.Remove(propertyName))
                {
                    ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                }
            }
        }

        #endregion

        #region INotifyDataErrorInfo Implementation

        /// <summary>
        /// Получает значение, указывающее, имеет ли объект ошибки валидации.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public bool HasErrors => _errors.Any();

        /// <summary>
        /// Событие, возникающее при изменении состояния ошибок валидации.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        /// <summary>
        /// Возвращает список ошибок валидации для указанного свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства, для которого запрашиваются ошибки.</param>
        /// <returns>Коллекция строк с ошибками валидации.</returns>
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_errors.ContainsKey(propertyName))
                return Enumerable.Empty<string>();

            return _errors[propertyName];
        }

        #endregion

        #region INotifyPropertyChanged Implementation

        /// <summary>
        /// Событие, возникающее при изменении значения свойства.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged для указанного свойства.
        /// </summary>
        /// <param name="propertyName">Имя изменившегося свойства. Заполняется автоматически.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}