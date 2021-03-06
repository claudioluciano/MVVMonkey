﻿using System;
using System.Windows.Input;

namespace MVVMonkey.Core.ViewModel
{
    public sealed class ViewModelCommand<T> : ViewModelCommand
    {
        public ViewModelCommand(ViewModelBase baseViewModel, Action<T> execute) : base(baseViewModel, o => execute((T)o))
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
        }

        public ViewModelCommand(ViewModelBase baseViewModel, Action<T> execute, Func<T, bool> canExecute) : base(baseViewModel, o => execute((T)o), o => canExecute((T)o))
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));
        }
    }

    public class ViewModelCommand : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _execute;
        private readonly ViewModelBase _baseViewModel;

        public event EventHandler CanExecuteChanged;

        public ViewModelCommand(ViewModelBase baseViewModel, Action<object> execute)
        {
            if (baseViewModel == null) throw new ArgumentNullException(nameof(baseViewModel));
            if (execute == null) throw new ArgumentNullException(nameof(execute));

            _baseViewModel = baseViewModel;
            _execute = execute;
        }

        public ViewModelCommand(ViewModelBase baseViewModel, Action execute) : this(baseViewModel, o => execute())
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
        }

        public ViewModelCommand(ViewModelBase baseViewModel, Action<object> execute, Func<object, bool> canExecute) : this(baseViewModel, execute)
        {
            if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));

            _canExecute = canExecute;
        }

        public ViewModelCommand(ViewModelBase baseViewModel, Action execute, Func<bool> canExecute) : this(baseViewModel, o => execute(), o => canExecute())
        {
            if (execute == null) throw new ArgumentNullException(nameof(execute));
            if (canExecute == null) throw new ArgumentNullException(nameof(canExecute));
        }

        public bool CanExecute(object parameter)
        {
            if (_baseViewModel.IsBusy)
                return false;

            if (_canExecute != null)
                return _canExecute(parameter);

            return true;
        }
        
        public void Execute(object parameter)
        {
            try
            {
                _baseViewModel.IsBusy = true;
                _execute(parameter);
            }
            finally
            {
                _baseViewModel.IsBusy = false;
            }
        }

        public void ChangeCanExecute()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}