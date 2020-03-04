using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DeviceReestr.ViewModel
{
    public class ObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        #region События

        /// <summary> Сигнализирует об изменении свойства объекта. </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary> Сигнализирует об начале изменения свойства объекта. </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

        #region Методы

        protected static string GetPropertyName<T>(Expression<Func<T>> expr)
        {
            if (expr.NodeType != ExpressionType.Lambda)
                throw new ArgumentException("Значение должно быть lambda выражением", paramName: nameof(expr));

            if (!(expr.Body is MemberExpression))
                throw new ArgumentException("Тело выражения должно быть ссылкой на свойство класса", paramName: nameof(expr));

            var body = (MemberExpression)expr.Body;
            return body.Member.Name;
        }

        /// <summary> Вызывает событие изменения свойства. </summary>
        /// <param name="propertyName"> Наименование изменившегося свойства. </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var temp = Interlocked.CompareExchange(ref PropertyChanged, null, null);
            if (temp == null)
                return;
            temp(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary> Вызывает событие начала изменения свойства. </summary>
        /// <param name="propertyName"> Наименование изменяющегося свойства. </param>
        protected virtual void OnPropertyChanging([CallerMemberName] string propertyName = "")
        {
            var temp = Interlocked.CompareExchange(ref PropertyChanging, null, null);
            if (temp == null)
                return;
            temp(this, new PropertyChangingEventArgs(propertyName));
        }

        #endregion
    }
}