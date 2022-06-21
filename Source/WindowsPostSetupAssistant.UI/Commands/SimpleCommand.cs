using System;
using System.Windows.Input;

namespace WindowsPostSetupAssistant.UI.Commands
{
    /// <summary>
    /// Command that does not take any parameter from the UI
    /// </summary>
    public class SimpleCommand : ICommand
    {
        private readonly Action _exec;

        /// <summary>
        /// Constructor in which we set the Action to execute when this command is called
        /// </summary>
        /// <param name="exec">Action to execute when this command is called</param>
        public SimpleCommand(Action exec)
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
        /// Executes this command's action when called
        /// </summary>
        /// <param name="parameter">Required for ICommand, is discarded</param>
        public void Execute(object parameter)
        {
            _exec.Invoke();
        }
        
#pragma warning disable 67
        /// <summary>
        /// Fires when CanExecute changes state
        /// </summary>
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }
}