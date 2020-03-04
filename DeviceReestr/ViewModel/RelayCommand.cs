using System;
using System.Windows.Input;

namespace DeviceReestr.ViewModel
{
    public class RelayCommand : ICommand
    {
        #region Поля

        /// <summary> Проверка на возможность запуска. </summary>
        private readonly Predicate<object> _canExecute;

        /// <summary> Обработчик команды. </summary>
        private readonly Action<object> _execute;

        #endregion

        #region Конструкторы

        /// <summary> Создаёт новый экземпляр класса <see cref="RelayCommand"/>. </summary>
        /// <param name="execute"> Действие производимое командой. </param>
        /// <exception cref="System.ArgumentNullException"> Значение <paramref name="execute"/> не задано. </exception>
        public RelayCommand(Action<object> execute)
          : this(execute, null)
        {
        }

        /// <summary> Создаёт новый экземпляр класса <see cref="RelayCommand"/>. </summary>
        /// <param name="execute"> &gt;Действие производимое командой.. </param>
        /// <param name="canExecute"> Проверка на возможность исполнения команды. </param>
        /// <exception cref="System.ArgumentNullException"> Значение <paramref name="execute"/> не задано. </exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region События

        /// <summary> Событие, срабатывающее при изменении возможности исполнения команды. </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Свойства

        /// <summary> Возвращает или назначает активную клавишу. </summary>
        public Key GestureKey { get; set; }

        /// <summary> Возвращает или назначает модификатор активной клавиши. </summary>
        public ModifierKeys GestureModifier { get; set; }

        /// <summary> Возвращает описание активной клавиши. </summary>
        public string GestureString
        {
            get
            {
                if (GestureKey == Key.None)
                    return string.Empty;

                return ((GestureModifier & ModifierKeys.Shift) == ModifierKeys.Shift ? "Shift+" : string.Empty)
                       + ((GestureModifier & ModifierKeys.Control) == ModifierKeys.Control ? "Ctrl+" : string.Empty)
                       + ((GestureModifier & ModifierKeys.Alt) == ModifierKeys.Alt ? "Alt+" : string.Empty)
                       + ((GestureKey == Key.Enter || GestureKey == Key.Return) ? "Enter" : Enum.GetName(typeof(Key), GestureKey));
            }
        }

        /// <summary> Действие мыши. </summary>
        public MouseAction MouseGesture { get; set; }

        #endregion

        #region Методы

        /// <summary> Реализует <see cref="ICommand.CanExecute"/>. </summary>
        /// <param name="parameter"> Данные для команды. </param>
        /// <returns> true - если команда может быть выполнена, иначе - false. </returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        /// <summary> Исполняет команду. </summary>
        /// <param name="parameter"> Данные для команды. </param>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion
    }
}