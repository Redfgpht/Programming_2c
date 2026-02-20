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

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Contact contactToSave = _mainVM.GetContact();
            _serializer.SaveContact(contactToSave);
            _mainVM.ClearContact();
        }
    }
}
