using nthLink.Header.Interface;
using nthLink.Header.Struct;
using nthLink.SDK.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace nthLink.Wpf.ViewModels
{
    internal class FeedbackPageViewModel : NotifyPropertyChangedBase
    {
        private readonly IMainThreadSyncContext mainThreadSyncContext;
        private readonly IClientInfo clientInfo;
        private readonly IReportService reportService;

        public ComboBoxItem[] IssueCategories { get; private set; }

        private ComboBoxItem? selectedIssueCategories;

        public ComboBoxItem? SelectedIssueCategories
        {
            get { return this.selectedIssueCategories; }
            set { SetProperty(ref this.selectedIssueCategories, value); }
        }

        private string email = string.Empty;

        public string Emlil
        {
            get { return this.email; }
            set { SetProperty(ref this.email, value); }
        }

        private string description = string.Empty;

        public string Description
        {
            get { return this.description; }
            set
            {
                int oriLength = this.description.Length;

                if (SetProperty(ref this.description, value))
                {
                    if ((oriLength == 0 && value.Length > 0) ||
                        (oriLength > 0 && value.Length == 0))
                    {
                        this.mainThreadSyncContext.Post(SendCommand.RaiseCanExecuteChanged);
                    }
                }
            }
        }

        private bool isSended;

        public bool IsSended
        {
            get { return this.isSended; }
            set { SetProperty(ref this.isSended, value); }
        }

        private bool isSendSuccess;

        public bool IsSendSuccess
        {
            get { return this.isSendSuccess; }
            set { SetProperty(ref this.isSendSuccess, value); }
        }


        public IRelayCommand SendCommand { get; }

        public FeedbackPageViewModel(ILanguageService languageService,
            IMainThreadSyncContext mainThreadSyncContext,
            IClientInfo clientInfo,
            IReportService reportService)
        {
            this.mainThreadSyncContext = mainThreadSyncContext;
            this.clientInfo = clientInfo;
            this.reportService = reportService;

            SendCommand = new RelayCommand(OnSendCommandExecute, CanSendCommandExecute);

            IssueCategories = MakeItemsSource(languageService);

            if (IssueCategories.Length > 0)
            {
                SelectedIssueCategories = IssueCategories[0];
            }
        }

        private async void OnSendCommandExecute()
        {
            string feedBackType = string.Empty;

            if (SelectedIssueCategories?.Value is string str)
            {
                feedBackType = str;
            }

            if (string.IsNullOrEmpty(feedBackType))
            {
                feedBackType = "General feedback";
            }

            string result = await Task.Run(() => this.reportService.Feedback(new FeedbackParameter()
            {
                AppVersion = this.clientInfo.AppVersion.ToString(),
                ClientId = this.clientInfo.ClientGuid,
                Os = "Windows",
                Language = CultureInfo.CurrentUICulture.Name,
                UtcSent = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                FeedbackType = feedBackType,
                Description = this.Description,
                Email = this.Emlil,
            }));

            IsSendSuccess = string.IsNullOrEmpty(result);

            IsSended = true;
        }

        private bool CanSendCommandExecute()
        {
            return !string.IsNullOrEmpty(this.description);
        }

        private ComboBoxItem[] MakeItemsSource(ILanguageService languageService)
        {
            //issue_categories
            const int count = 10;

            List<ComboBoxItem> items = new List<ComboBoxItem>(count);

            for (int i = 0; i < count; i++)
            {
                string key = "issue_categories_" + i.ToString();

                string str = languageService.GetString(key);

                if (key == str)
                {
                    break;
                }

                string value = languageService.GetStringWithDefaultCulture(key);

                if (key == value)
                {
                    value = string.Empty;
                }

                items.Add(new ComboBoxItem()
                {
                    Display = str,
                    Value = value,
                });
            }

            return items.ToArray();
        }

        internal void RestoreDefault()
        {
            Emlil = string.Empty;
            Description = string.Empty; if (IssueCategories.Length > 0)
            {
                SelectedIssueCategories = IssueCategories[0];
            }
            IsSended = false;
            IsSendSuccess = false;
        }
    }
}
