using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarketTester.Base
{
    public class BaseCommand : ICommand
    {
        private readonly Action<object> _action;
        private readonly CanExecuteFunc _canExeuteFunc;

        public event EventHandler CanExecuteChanged;

        public delegate bool CanExecuteFunc();

        public BaseCommand(Action<object> action, CanExecuteFunc canExecuteFunc)
        {
            _action = action;
            _canExeuteFunc = canExecuteFunc;
        }

        public void Execute(object parameter)
        {
            _action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExeuteFunc();
        }
    }
}
