using System;
using System.Threading.Tasks;

namespace DeviceReestr.Services
{
    public interface ILongOperationService
    {
        /// <summary>
        /// Удаляет признак длительной операции
        /// </summary>
        void RemoveLongOperation();
        /// <summary>
        /// Признак длительной операции
        /// </summary>
        bool InProgress { get; }

        event Action<bool> LongOperationProgressChanged;
        event Action<string> LongOperationTextChanged;
        event Action<Exception> LongOperationExceptioned;
        Task ExecuteAsync(Func<Task> action, string text);
        Task<T> ExecuteAsync<T>(Func<T> action, string text);
        Task ExecuteAsync(Action action, string text);
    }
    public class LongOperationService : ILongOperationService
    {
        private const string DefaultText = "Выполняется длительная операция. Пожалуйста, подождите.";

        public void RemoveLongOperation()
        {
            InProgress = false;
        }

        public bool InProgress { get; private set; }

        public event Action<bool> LongOperationProgressChanged;
        public event Action<string> LongOperationTextChanged;
        public event Action<Exception> LongOperationExceptioned;

        public async Task ExecuteAsync(Action action, string text)
        {
            try
            {
                LongOperationTextChanged?.Invoke(text);
                LongOperationProgressChanged?.Invoke(true);
                await Task.Run(action);
            }
            catch (Exception ex)
            {
                LongOperationExceptioned?.Invoke(ex);
            }
            finally
            {
                LongOperationProgressChanged?.Invoke(false);
            }
        }

        public async Task ExecuteAsync(Func<Task> action, string text)
        {
            try
            {
                LongOperationTextChanged?.Invoke(text);
                LongOperationProgressChanged?.Invoke(true);
                await Task.Run(action);
            }
            catch (Exception ex)
            {
                LongOperationExceptioned?.Invoke(ex);
            }
            finally
            {
                LongOperationProgressChanged?.Invoke(false);
            }
        }

        public async Task<T> ExecuteAsync<T>(Func<T> action, string text)
        {
            T result = default;
            try
            {
                LongOperationTextChanged?.Invoke(text);
                LongOperationProgressChanged?.Invoke(true);
                result = await Task.Run(action);
            }
            catch (Exception ex)
            {
                LongOperationExceptioned?.Invoke(ex);
            }
            finally
            {
                LongOperationProgressChanged?.Invoke(false);
            }

            return result;
        }
    }
}