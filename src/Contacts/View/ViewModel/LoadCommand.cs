using System;
using System.Windows.Input;
using View.Model;
using View.Model.Services;

namespace View.ViewModel
{
    /// <summary>
    /// Команда загрузки контакта из файла.
    /// </summary>
    public class LoadCommand : ICommand
    {
        private readonly MainVM _mainVM;
        private readonly ContactSerializer _serializer;

        /// <summary>
        /// Создает новый экземпляр команды загрузки.
        /// </summary>
        /// <param name="mainVM">Ссылка на MainVM для установки загруженного контакта.</param>
        public LoadCommand(MainVM mainVM)
        {
            _mainVM = mainVM;
            _serializer = new ContactSerializer();
        }

        /// <summary>
        /// Создание события CanExecuteChanged.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Определяет, может ли команда выполняться в текущем состоянии.
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Выполняет логику команды загрузки контакта.
        /// </summary>
        public void Execute(object? parameter)
        {
            Contact loadedContact = _serializer.LoadContact();
            _mainVM.SetContact(loadedContact);
        }
    }
}
