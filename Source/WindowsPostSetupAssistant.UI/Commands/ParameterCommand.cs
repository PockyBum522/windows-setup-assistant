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
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the current command
        /// </summary>
        /// <param name="parameter">Passed parameter from UI</param>
        public void Execute(object? parameter)
        {
            _exec.Invoke(parameter!);
        }
        
        /// <summary>
        /// Fires when CanExecute changes state, which never happens since we always have it set to true
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add { throw new NotSupportedException(); }
            remove { }
        }
    }
}