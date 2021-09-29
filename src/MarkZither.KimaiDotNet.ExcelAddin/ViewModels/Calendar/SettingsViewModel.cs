
using MarkZither.KimaiDotNet.ExcelAddin.Services;

using MvvmHelpers.Commands;

using MvvmValidation;

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarkZither.KimaiDotNet.ExcelAddin.ViewModels.Calendar
{
    public class SettingsViewModel : ValidatableViewModelBase
    {
        // https://stackoverflow.com/questions/16172462/close-window-from-viewmodel
        // http://jkshay.com/closing-a-wpf-window-using-mvvm-and-minimal-code-behind/
        public ICommand SaveCommand { get; }
        public Action CloseAction { get; set; }
        public Action Changed { get; set; }
        private bool? isValid;
        private string validationErrorsString;

        string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                SetProperty(ref userName, value, onChanged: Changed);
                Validator.ValidateAsync(nameof(UserName));
            }
        }

        string password;
        public string Password
        {
            get { return password; }
            set
            {
                SetProperty(ref password, value, onChanged: Changed);
                Validator.ValidateAsync(nameof(Password));
            }
        }

        string oWAUrl;
        public string OWAUrl
        {
            get { return oWAUrl; }
            set
            {
                SetProperty(ref oWAUrl, value, onChanged: Changed);
                Validator.ValidateAsync(nameof(OWAUrl));
            }
        }
        public SettingsViewModel()
        {
            StartDate = Globals.ThisAddIn.CalSyncStartDate;
            EndDate = Globals.ThisAddIn.CalSyncEndDate;
            OWAUrl = Globals.ThisAddIn.OWAUrl;
            UserName = Globals.ThisAddIn.OWAUsername;
            Password = Globals.ThisAddIn.OWAPassword;
            ValidateCommand = new AsyncCommand(ValidateAsync);
            ConfigureValidationRules();
            Validator.ResultChanged += OnValidationResultChanged;
            SaveCommand = new AsyncCommand(SaveOWACredntialsAsync);
        }
        public ICommand ValidateCommand { get; private set; }

        async Task SaveOWACredntialsAsync()
        {
            var validationResult = await ValidateAsync().ConfigureAwait(false);
            if (validationResult.IsValid)
            {
                Globals.ThisAddIn.OWAUrl = OWAUrl;
                Globals.ThisAddIn.OWAUsername = userName;
                Globals.ThisAddIn.OWAPassword = password;
                _ = await Task.Run(() => { return ""; }).ConfigureAwait(false);
                ICalendarService calendarService = new EwsCalendarService(Globals.ThisAddIn.OWAUrl, Globals.ThisAddIn.OWAUsername, Globals.ThisAddIn.OWAPassword);
                var categories = calendarService.GetCategories();
                Globals.ThisAddIn.Categories = categories.Category;
                Sheets.CalendarCategoryWorksheet.Instance.CreateOrUpdateCalendarCategoriesOnSheet(categories);

                CloseAction(); // Invoke the Action previously defined by the View
                               // do stuff async
            }
        }
        
        private async Task<ValidationResult> ValidateAsync()
        {
            var result = await Validator.ValidateAllAsync().ConfigureAwait(true);

            UpdateValidationSummary(result);

            return result;
        }
        public string ValidationErrorsString
        {
            get { return validationErrorsString; }
            private set
            {
                validationErrorsString = value;
                bool success = SetProperty(ref validationErrorsString, value);
                if (!success)
                {
                    OnPropertyChanged();
                }
            }
        }

        public bool? IsValid
        {
            get { return isValid; }
            private set
            {
                SetProperty(ref isValid, value);
            }
        }
        private void ConfigureValidationRules()
        {
            Validator.AddRequiredRule(() => OWAUrl, "Url is required");
            Validator.AddRequiredRule(() => UserName, "User Name is required");
            Validator.AddRequiredRule(() => Password, "Password is required");
        }

        private void OnValidationResultChanged(object sender, ValidationResultChangedEventArgs e)
        {
            if (!IsValid.GetValueOrDefault(true))
            {
                ValidationResult validationResult = Validator.GetResult();

                UpdateValidationSummary(validationResult);
            }
        }
        private void UpdateValidationSummary(ValidationResult validationResult)
        {
            IsValid = validationResult.IsValid;
            ValidationErrorsString = validationResult.ToString();
        }

        private DateTime? startDate;
        public DateTime? StartDate { 
            get => startDate; 
            set => SetProperty(ref startDate, value, onChanged: SetCalendarImportStartDate); 
        }
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get => endDate;
            set => SetProperty(ref endDate, value, onChanged: SetCalendarImportEndDate);
        }
        void SetCalendarImportStartDate()
        {
            Globals.ThisAddIn.CalSyncStartDate = StartDate.Value;
            Globals.Ribbons.KimaiRibbon.lblStartDate.Label = StartDate.Value.ToShortDateString();
        }
        void SetCalendarImportEndDate()
        {
            Globals.ThisAddIn.CalSyncEndDate = EndDate.Value;
            Globals.Ribbons.KimaiRibbon.lblEndDate.Label = EndDate.Value.ToShortDateString();
        }
    }
}
