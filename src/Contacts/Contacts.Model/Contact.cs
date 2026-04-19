namespace Contacts.Model;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Представляет модель контакта с валидацией данных.
/// Реализует интерфейсы INotifyPropertyChanged для уведомления об изменениях свойств
/// и INotifyDataErrorInfo для валидации данных на уровне модели.
/// </summary>
public partial class Contact : ObservableObject, INotifyDataErrorInfo
{
    /// <summary>
    /// Максимальная длина текстовых полей (имя, телефон, email).
    /// </summary>
    private const int MaxLength = 100;

    private string _name = string.Empty;
    private string _phoneNumber = string.Empty;
    private string _email = string.Empty;
    private byte[]? _photoData;

    /// <summary>
    /// Словарь для хранения ошибок валидации по свойствам.
    /// Ключ - имя свойства, значение - список ошибок.
    /// </summary>
    private readonly Dictionary<string, List<string>> _errors = new();

    /// <summary>
    /// Регулярное выражение для проверки номера телефона.
    /// Разрешает цифры и символы: + - ( )
    /// </summary>
    private readonly Regex _phoneRegex = new(@"^[0-9+\-\(\)\s]*$");

    /// <summary>
    /// Регулярное выражение для проверки email адреса.
    /// Проверяет наличие символа @ и доменной части.
    /// </summary>
    private readonly Regex _emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

    /// <summary>
    /// Инициализирует новый экземпляр класса Contact с пустыми значениями.
    /// Выполняет начальную валидацию всех свойств.
    /// </summary>
    public Contact()
    {
        ValidateAllProperties();
    }

    /// <summary>
    /// Инициализирует новый экземпляр класса Contact с указанными значениями.
    /// </summary>
    /// <param name="name">Имя контакта. Не может быть null.</param>
    /// <param name="phoneNumber">Номер телефона контакта. Не может быть null.</param>
    /// <param name="email">Email адрес контакта. Не может быть null.</param>
    /// <param name="photoData">Данные фотографии в виде массива байтов.</param>
    public Contact(string name, string phoneNumber, string email, byte[]? photoData = null)
    {
        _name = name ?? string.Empty;
        _phoneNumber = phoneNumber ?? string.Empty;
        _email = email ?? string.Empty;
        _photoData = photoData;

        ValidateAllProperties();
    }

    /// <summary>
    /// Конструктор копирования.
    /// </summary>
    /// <param name="other">Копируемый контакт.</param>
    public Contact(Contact other)
    {
        _name = other._name;
        _phoneNumber = other._phoneNumber;
        _email = other._email;
        _photoData = other._photoData?.ToArray(); // Копируем массив, если есть
        ValidateAllProperties();
    }

    /// <summary>
    /// Получает или задает имя контакта.
    /// При изменении значения выполняет валидацию и вызывает событие PropertyChanged.
    /// </summary>
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
    /// При изменении значения выполняет валидацию и вызывает событие PropertyChanged.
    /// </summary>
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
    /// При изменении значения выполняет валидацию и вызывает событие PropertyChanged.
    /// </summary>
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
    /// Получает или задает данные фотографии в виде массива байтов.
    /// </summary>
    public byte[]? PhotoData
    {
        get => _photoData;
        set => SetProperty(ref _photoData, value);
    }

    /// <summary>
    /// Выполняет валидацию имени контакта.
    /// Проверяет, что имя не пустое и не превышает максимальную длину.
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
    /// Проверяет, что номер не пустой, не превышает максимальную длину
    /// и содержит только допустимые символы (цифры, +, -, (, )).
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
            if (PhoneNumber.Length > MaxLength)
            {
                errors.Add($"Телефон не может быть длиннее {MaxLength} символов");
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
    /// Проверяет, что email не пустой, не превышает максимальную длину,
    /// содержит символ @ и соответствует формату email.
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
    /// Вызывается при создании объекта для начальной проверки.
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
            _errors[propertyName] = errors;
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

    /// <summary>
    /// Получает значение, указывающее, имеет ли объект ошибки валидации.
    /// </summary>
    public bool HasErrors => _errors.Count != 0;

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
        if (string.IsNullOrEmpty(propertyName) || !_errors.TryGetValue(propertyName, out var errorList))
            return Enumerable.Empty<string>();

        return errorList;
    }
}