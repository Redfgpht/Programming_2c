using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace View.ViewModel
{
    /// <summary>
    /// Базовый класс для всех ViewModel, реализующий INotifyPropertyChanged.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Вызывает событие PropertyChanged для уведомления об изменении свойства.
        /// </summary>
        /// <param name="propertyName">Имя свойства (автоматически подставляется).</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
