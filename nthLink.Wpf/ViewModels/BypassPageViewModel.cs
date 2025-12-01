using nthLink.Header.Interface;
using nthLink.SDK.Model;
using nthLink.Wpf.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace nthLink.Wpf.ViewModels
{
    internal class BypassPageViewModel : NotifyPropertyChangedBase
    {
        private const string BYPASS_SELECTED_SET = "bypassselectedset";

        private string processName = string.Empty;

        public string ProcessName
        {
            get { return this.processName; }
            set { SetProperty(ref this.processName, value); }
        }

        private ProcessViewModel[] itemsSource;

        public ProcessViewModel[] ItemsSource
        {
            get { return this.itemsSource; }
            set { SetProperty(ref this.itemsSource, value); }
        }

        private bool isSelectAll;

        public bool IsSelectAll
        {
            get { return this.isSelectAll; }
            set
            {
                if (SetProperty(ref this.isSelectAll, value))
                {
                    SelectAll(value);
                }
            }
        }

        public IRelayCommand AddProcessNameCommand { get; }
        public IRelayCommand UpdateProcessListCommand { get; }

        private readonly HashSet<string> selectedItems;

        private readonly IDataPersistence dataPersistence;
        private readonly IJsonConverter jsonConverter;
        private readonly BypassSetProvider? bypassSetProvider;

        public BypassPageViewModel(IDataPersistence dataPersistence, IJsonConverter jsonConverter,
            IBypassSetProvider bypassSetProvider)
        {
            AddProcessNameCommand = new RelayCommand(OnAddProcessNameCommandExecute);
            UpdateProcessListCommand = new RelayCommand(OnUpdateProcessListCommandExecute);
            this.dataPersistence = dataPersistence;
            this.jsonConverter = jsonConverter;
            this.bypassSetProvider = bypassSetProvider as BypassSetProvider;

            string save = this.dataPersistence.Load(BYPASS_SELECTED_SET);

            if (!string.IsNullOrEmpty(save))
            {
                HashSet<string>? temp = this.jsonConverter.Deserialize<HashSet<string>>(save);

                if (temp != null)
                {
                    this.selectedItems = temp;
                }
            }

            if (this.selectedItems == null)
            {
                this.selectedItems = new HashSet<string>();
            }

            if (this.bypassSetProvider != null)
            {
                this.bypassSetProvider.SetBypassNames(this.selectedItems);
            }

            this.itemsSource = MakeItemsSource();
        }

        private void SelectAll(bool value)
        {
            if (this.itemsSource != null)
            {
                foreach (var item in this.itemsSource)
                {
                    item.PropertyChanged -= ProcessViewModel_PropertyChanged;
                }

                ProcessViewModel[] currentItemsSource = this.itemsSource;

                if (value)
                {
                    foreach (var item in currentItemsSource)
                    {
                        this.selectedItems.Add(item.Name);
                    }
                }
                else
                {
                    this.selectedItems.Clear();
                }

                SelectedItemsChanged();

                ItemsSource = new ProcessViewModel[0];

                foreach (var item in currentItemsSource)
                {
                    item.IsSelected = value;
                }

                ItemsSource = currentItemsSource;

                foreach (var item in currentItemsSource)
                {
                    item.PropertyChanged += ProcessViewModel_PropertyChanged;
                }
            }
        }

        private void OnUpdateProcessListCommandExecute()
        {
            ItemsSource = MakeItemsSource();
        }

        private void OnAddProcessNameCommandExecute()
        {
            if (!string.IsNullOrEmpty(ProcessName) &&
                !this.selectedItems.Contains(ProcessName))
            {
                ProcessViewModel processViewModel = new ProcessViewModel()
                {
                    Name = ProcessName,
                    IsSelected = true,
                };

                processViewModel.PropertyChanged += ProcessViewModel_PropertyChanged;

                this.selectedItems.Add(ProcessName);

                List<ProcessViewModel> finalItemsSource = new List<ProcessViewModel>();

                if (this.itemsSource != null)
                {
                    finalItemsSource.AddRange(this.itemsSource);
                }

                finalItemsSource.Add(processViewModel);

                ItemsSource = finalItemsSource.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToArray();

                ProcessName = string.Empty;
            }
        }

        private void ProcessViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProcessViewModel.IsSelected) &&
                sender is ProcessViewModel processViewModel)
            {
                if (processViewModel.IsSelected)
                {
                    this.selectedItems.Add(processViewModel.Name);
                }
                else
                {
                    this.selectedItems.Remove(processViewModel.Name);
                }
                SelectedItemsChanged();
            }
        }

        private ProcessViewModel[] MakeItemsSource()
        {
            HashSet<string> allProcess = GetAllProcess();

            Dictionary<string, ProcessViewModel> itemPair = new Dictionary<string, ProcessViewModel>();

            foreach (var item in allProcess)
            {
                ProcessViewModel processViewModel = new ProcessViewModel()
                {
                    Name = item,
                    IsSelected = false,
                };

                itemPair.Add(item, processViewModel);
            }

            if (this.selectedItems.Count != 0)
            {
                foreach (var item in this.selectedItems)
                {
                    if (itemPair.ContainsKey(item))
                    {
                        itemPair[item].IsSelected = true;
                    }
                    else
                    {
                        itemPair.Add(item, new ProcessViewModel()
                        {
                            Name = item,
                            IsSelected = true,
                        });
                    }
                }
            }

            ProcessViewModel[] finalItemsSource = itemPair.Values.ToArray();

            foreach (var item in finalItemsSource)
            {
                item.PropertyChanged += ProcessViewModel_PropertyChanged;
            }

            if (this.itemsSource != null)
            {
                foreach (var item in this.itemsSource)
                {
                    item.PropertyChanged -= ProcessViewModel_PropertyChanged;
                }
            }

            return finalItemsSource.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        }

        private HashSet<string> GetAllProcess()
        {
            var processNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var process in Process.GetProcesses())
            {
                try
                {
                    processNames.Add(process.ProcessName);
                    process.Dispose();
                }
                catch
                {

                }
            }

            return processNames;
        }

        private void SelectedItemsChanged()
        {
            this.dataPersistence.Cache(BYPASS_SELECTED_SET, this.jsonConverter.Serialize(this.selectedItems));
        }
    }

    internal class ProcessViewModel : NotifyPropertyChangedBase
    {
        private string name = string.Empty;

        public string Name
        {
            get { return this.name; }
            set { SetProperty(ref this.name, value); }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get { return this.isSelected; }
            set { SetProperty(ref this.isSelected, value); }
        }
    }
}
