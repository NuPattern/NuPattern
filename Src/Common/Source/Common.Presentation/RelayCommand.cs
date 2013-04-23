using System;
using System.Diagnostics;
using System.Windows.Input;

namespace NuPattern.Presentation
{
    /// <summary>
    /// Relays commands from view to viewmodel
    /// </summary>
    [DebuggerStepThrough]
    public class RelayCommand : ICommand
    {
        private Action execute;
        private Func<bool> canExecute;

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        public RelayCommand(Action execute)
            : this(execute, () => true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            Guard.NotNull(() => execute, execute);
            Guard.NotNull(() => canExecute, canExecute);

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Raises an event when execution of the command could change.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Determines if the command can be executed.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return this.canExecute();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.execute();
            }
        }
    }

    /// <summary>
    /// Relays commands from view to viewmodel
    /// </summary>
    [DebuggerStepThrough]
    public class RelayCommand<T> : ICommand
    {
        private Action<T> execute;
        private Predicate<T> canExecute;

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        public RelayCommand(Action<T> execute)
            : this(execute, o => true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            Guard.NotNull(() => execute, execute);
            Guard.NotNull(() => canExecute, canExecute);

            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Raises an event when execution of the command could change.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Determines if the command can be executed.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return this.canExecute((T)parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        public void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                this.execute((T)parameter);
            }
        }
    }
}