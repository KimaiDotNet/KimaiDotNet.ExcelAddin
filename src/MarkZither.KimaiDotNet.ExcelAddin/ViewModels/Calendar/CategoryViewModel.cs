using MvvmHelpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers.Commands;
using System.Windows.Input;

namespace MarkZither.KimaiDotNet.ExcelAddin.ViewModels.Calendar
{
    public class CategoryViewModel : BaseViewModel
    {
        // https://stackoverflow.com/questions/16172462/close-window-from-viewmodel
        // http://jkshay.com/closing-a-wpf-window-using-mvvm-and-minimal-code-behind/
        public ICommand SaveCommand { get; }
        public Action Changed { get; set; }
        private bool ValidateUserName(string currentUserName, string newUserName)
        {
            if (newUserName?.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                SetProperty(ref userName, value, onChanged: Changed, validateValue: ValidateUserName);
            }
        }
        public CategoryViewModel()
        {
            SaveCommand = new AsyncCommand(SaveOWACredntialsAsync);
        }

        async Task SaveOWACredntialsAsync()
        {
            _ = await Task.Run(() => { return ""; }).ConfigureAwait(false);
            // do stuff async
        }
    }
}
