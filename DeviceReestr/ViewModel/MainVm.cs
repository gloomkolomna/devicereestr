using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DeviceReestr.Services;

namespace DeviceReestr.ViewModel
{
    public interface ILoginPanel
    {
        bool HasAutorizationUser { get; set; }
        string Login { get; set; }
        string Password { get; set; }
        ICommand AuthorizationCommand { get; }
        ICommand CreateUserCommand { get; }
    }
    public interface IBaseVm
    {
        void Close();
    }

    public interface IMainVm : IBaseVm, ILongOperationNotify, ILoginPanel
    {
        IBaseVm SelectedView { get; set; }
        SynchronizationContext GetContext();
    }

    public class MainVm : ObservableObject, IMainVm
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILongOperationService _longOperationService;
        private readonly IDialogService _dialogService;
        private readonly IUserService _userService;
        private static readonly SynchronizationContext SynchronizationContext = SynchronizationContext.Current;
        private bool _longOperationInProgress;
        private string _longOperationText;
        private IBaseVm _selectedView;
        private bool _hasAutorizationUser;
        private string _login;
        private string _password;
        private ICommand _authorizationCommand;
        private ICommand _createUserCommand;

        public MainVm(IServiceProvider serviceProvider, ILongOperationService longOperationService,
            IDialogService dialogService, IUserService userService)
        {
            _serviceProvider = serviceProvider;
            _longOperationService = longOperationService;
            _dialogService = dialogService;
            _userService = userService;

            Subscribe();
        }

        public void Init()
        {
            if (HasAutorizationUser)
            {
                if (_serviceProvider.GetService(typeof(IWorkAreaVm)) is WorkAreaVm workAreaVm)
                {
                    SelectedView = workAreaVm;
                }
            }
        }

        public bool LongOperationInProgress
        {
            get => _longOperationInProgress;
            set
            {
                _longOperationInProgress = value;
                OnPropertyChanged();
            }
        }

        public string LongOperationText
        {
            get => _longOperationText;
            set
            {
                _longOperationText = value;
                OnPropertyChanged();
            }
        }
        public IBaseVm SelectedView
        {
            get => _selectedView;
            set
            {
                _selectedView = value;
                OnPropertyChanged();
            }
        }
        public SynchronizationContext GetContext()
        {
            return SynchronizationContext;
        }

        public void Close()
        {
            UnSubscribe();
            SelectedView.Close();
        }

        private void Subscribe()
        {
            _longOperationService.LongOperationProgressChanged += OnLongOperationProgressChanged;
            _longOperationService.LongOperationTextChanged += OnLongOperationTextChanged;
            _longOperationService.LongOperationExceptioned += OnLongOperationExceptioned;
        }

        private void UnSubscribe()
        {
            _longOperationService.LongOperationProgressChanged -= OnLongOperationProgressChanged;
            _longOperationService.LongOperationTextChanged -= OnLongOperationTextChanged;
            _longOperationService.LongOperationExceptioned -= OnLongOperationExceptioned;
        }

        private void OnLongOperationExceptioned(Exception ex)
        {
            _dialogService.Show(DialogIcon.Error, "Ошибка", ex.Message);
            LongOperationInProgress = false;
        }

        private void OnLongOperationProgressChanged(bool result)
        {
            SynchronizationContext.Post(post =>
            {
                LongOperationInProgress = result;
            }, null);
        }

        private void OnLongOperationTextChanged(string text)
        {
            SynchronizationContext.Post(post =>
            {
                LongOperationText = text;
            }, null);
        }

        public bool HasAutorizationUser
        {
            get => _hasAutorizationUser;
            set
            {
                _hasAutorizationUser = value;
                OnPropertyChanged();
            }
        }

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand AuthorizationCommand
        {
            get
            {
                return _authorizationCommand ?? (_authorizationCommand = new RelayCommand(
                           param => Authorization())
                {
                    MouseGesture = MouseAction.LeftClick
                });
            }
        }

        public ICommand CreateUserCommand
        {
            get
            {
                return _createUserCommand ?? (_createUserCommand = new RelayCommand(
                           param => CreateUser())
                {
                    MouseGesture = MouseAction.LeftClick
                });
            }
        }

        private async Task CreateUser()
        {
            if (AuthorizationValid())
            {
                var user = await _longOperationService.ExecuteAsync(() => _userService.CreateUser(Login, Password), "Регистрация пользователя");
                if (user != null)
                {
                    _dialogService.Show(DialogIcon.Information, "Авторизация", $"Пользователь {Login} добавлен.");
                }
                else
                {
                    _dialogService.Show(DialogIcon.Error, "Авторизация", $"Не удалось добавить пользователя {Login}.");
                }
            }
        }

        private async Task Authorization()
        {
            if (AuthorizationValid())
            {
                if (AdminValidation())
                {
                    var user = await _longOperationService.ExecuteAsync(() =>
                    _userService.GetUser(Login, Password), "Авторизация");
                    if (user != null)
                    {
                        _userService.CurrentUser = user;
                        HasAutorizationUser = true;
                        Init();
                    }
                }
                else
                {
                    _dialogService.Show(DialogIcon.Warning, "Предупреждение", "Пользователь не admin");
                }
            }
        }

        private bool AdminValidation()
        {
            if (Login.ToLower().Equals("admin") && Password.ToLower().Equals("admin"))
            {
                return true;
            }

            return false;
        }

        private bool AuthorizationValid()
        {
            if (string.IsNullOrEmpty(Login))
            {
                _dialogService.Show(DialogIcon.Warning, "Предупреждение", "Не указан логин");
                return false;
            }

            if (string.IsNullOrEmpty(Password))
            {
                _dialogService.Show(DialogIcon.Warning, "Предупреждение", "Не указан пароль");
                return false;
            }

            return true;
        }
    }
}