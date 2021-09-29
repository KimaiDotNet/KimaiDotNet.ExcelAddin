using MvvmHelpers;

using MvvmValidation;

using System;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MarkZither.KimaiDotNet.ExcelAddin.ViewModels
{
    public class ValidatableViewModelBase : BaseViewModel, IValidatable, INotifyDataErrorInfo
    {
        protected ValidationHelper Validator { get; private set; }
        private NotifyDataErrorInfoAdapter NotifyDataErrorInfoAdapter { get; set; }
        protected ValidatableViewModelBase()
        {
            Validator = new ValidationHelper();

            NotifyDataErrorInfoAdapter = new NotifyDataErrorInfoAdapter(Validator);
            NotifyDataErrorInfoAdapter.ErrorsChanged += OnErrorsChanged;
        }
        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            // Notify the UI that the property has changed so that the validation error gets displayed (or removed).
            OnPropertyChanged(e.PropertyName);
        }
        Task<ValidationResult> IValidatable.Validate()
        {
            return Validator.ValidateAllAsync();
        }
        #region Implementation of INotifyDataErrorInfo
        public IEnumerable GetErrors(string propertyName)
        {
            return NotifyDataErrorInfoAdapter.GetErrors(propertyName);
        }

        public bool HasErrors
        {
            get { return NotifyDataErrorInfoAdapter.HasErrors; }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add { NotifyDataErrorInfoAdapter.ErrorsChanged += value; }
            remove { NotifyDataErrorInfoAdapter.ErrorsChanged -= value; }
        }
        #endregion
    }
}
