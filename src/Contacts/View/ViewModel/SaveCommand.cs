using System;
using System.Windows.Input;
using View.Model;
using View.Model.Services;

namespace View.ViewModel
{
    /// <summary>
    /// Команда сохранения контакта в файл.
    /// </summary>
    public class SaveCommand : ICommand
    {
        private readonly MainVM _mainVM;
        private readonly ContactSerializer _serializer;

        /// <summary>
        /// Создает новый экземпляр команды сохранения.
        /// </summary>
        /// <param name="mainVM">Ссылка на MainVM для доступа к контакту.</param>
        public SaveCommand(MainVM mainVM)
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
            Contact contactToSave = _mainVM.GetContact();
            _serializer.SaveContact(contactToSave);
            _mainVM.ClearContact();
        }
    }
}
