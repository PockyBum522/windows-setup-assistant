using System;
using System.Windows.Input;

namespace WindowsPostSetupAssistant.UI.Commands
{
    /// <summary>
    /// Command for MVVM that takes in a parameter from the UI
    /// </summary>
    public class ParameterCommand : ICommand
    {
        private readonly Action<object> _exec;

        /// <summary>
        /// Constructor where we set the Action to execute when the command is called
        /// </summary>
        /// <param name="exec">Action to execute when the command is called</param>
        public ParameterCommand(Action<object> exec)
        {
            _exec = exec;
        }
        
        /// <summary>
        /// Returns true if the command can execute currently, false otherwise 
        /// </summary>
        /// <param name="parameter">Required for ICommand, is discarded</param>
        /// <returns>Always true</returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the current command
        /// </summary>
        /// <param name="parameter">Passed parameter from UI</param>
        public void Execute(object parameter)
        {
            _exec.Invoke(parameter);
        }
        
#pragma warning disable 67
        /// <summary>
        /// Fires when CanExecute changes state
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }
}