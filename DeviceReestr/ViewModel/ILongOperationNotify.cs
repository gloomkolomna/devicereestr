using System.ComponentModel;

namespace DeviceReestr.ViewModel
{
    public interface ILongOperationNotify : INotifyPropertyChanged
    {
        /// <summary>
        /// Признак выполнения длительной операции
        /// </summary>
        bool LongOperationInProgress { get; set; }
        /// <summary>
        /// Текст длительной операции
        /// </summary>
        string LongOperationText { get; set; }
    }
}