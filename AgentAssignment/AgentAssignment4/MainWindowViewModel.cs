using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using Prism.Mvvm;

namespace AgentAssignment4
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<Agent> agents;
        private string _filename = "";
        readonly DispatcherTimer _timer = new DispatcherTimer();

        public MainWindowViewModel()
        {
            agents = new ObservableCollection<Agent>
            {
                #if DEBUG
                new Agent("001", "Nina", "Assassination", "UpperVolta"),
                new Agent("007", "James Bond", "Martinis", "North Korea")
                #endif
            };
            CurrentAgent = null;

            Specialties = new ObservableCollection<string>
            {
                "Assassination",
                "Bombs",
                "Martinis",
                "Low profile",
                "Spy"
            };

            SortCollection = new ObservableCollection<string>
            {
                "None",
                "ID",
                "CodeName",
                "Specialty",
                "Assignment"
            };

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            Clock.Update();
        }

        #region Properties

        public ObservableCollection<Agent> Agents
        {
            get => agents;
            set => SetProperty(ref agents, value);
        }

        

        Agent _currentAgent = null;

        public Agent CurrentAgent
        {
            get => _currentAgent;
            set => SetProperty(ref _currentAgent, value);
        }

        int _currentIndex = -1;
        public int CurrentIndex
        {
            get => _currentIndex;
            set => SetProperty(ref _currentIndex, value);
        }

        public Clock Clock { get; set; } = new Clock();


        private ObservableCollection<string> _specialties;
        public ObservableCollection<string> Specialties
        {
            get => _specialties;
            set => SetProperty(ref _specialties, value);
        }

        private ObservableCollection<string> _sortingCollection;
        public ObservableCollection<string> SortCollection
        {
            get => _sortingCollection;
            set => SetProperty(ref _sortingCollection, value);
        }

        #endregion

        #region Commands

        ICommand _previousCommand;
        public ICommand PreviousCommand
        {
            get
            {
                return _previousCommand ??
                (_previousCommand = new DelegateCommand(
                 PreviousCommandExecute, PreviousCommandCanExecute).ObservesProperty(() => CurrentIndex));
            }
        }

        private void PreviousCommandExecute()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
        }

        private bool PreviousCommandCanExecute()
        {
            if (CurrentIndex > 0)
                return true;
            else
                return false;
        }

        ICommand _nextCommand;
        public ICommand NextCommand
        {
            get
            {
                return _nextCommand ?? (_nextCommand = new DelegateCommand(
                       () => ++CurrentIndex,
                       () => CurrentIndex < (Agents.Count - 1)).ObservesProperty(() => CurrentIndex));
            }
        }

        ICommand _newCommand;
        public ICommand AddNewAgentCommand
        {
            get
            {
                return _newCommand ?? (_newCommand = new DelegateCommand(() =>
                {
                    Agents.Add(new Agent());
                    CurrentIndex = Agents.Count - 1;
                }));
            }
        }

        ICommand _deleteCommand;
        public ICommand DeleteAgentCommand => _deleteCommand ?? (_deleteCommand =
            new DelegateCommand(DeleteAgent, DeleteAgent_CanExecute)
                    .ObservesProperty(() => CurrentIndex));

        private void DeleteAgent()
        {
            Agents.RemoveAt(CurrentIndex);
            // RaisePropertyChanged("Count");
        }

        private bool DeleteAgent_CanExecute()
        {
            if (Agents.Count > 0 && CurrentIndex >= 0)
                return true;
            else
                return false;
        }

        ICommand _closeAppCommand;
        public ICommand CloseAppCommand
        {
            get
            {
                return _closeAppCommand ?? (_closeAppCommand = new DelegateCommand(() =>
                {
                    App.Current.MainWindow.Close();
                }));
            }
        }

        ICommand _saveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return _saveAsCommand ?? (_saveAsCommand = new DelegateCommand<string>(SaveAsCommand_Execute)); }
        }

        private void SaveAsCommand_Execute(string argFilename)
        {
            if (argFilename == "")
            {
                MessageBox.Show("You must enter a file name in the File Name textbox!", "Unable to save file",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                _filename = argFilename;
                SaveFileCommand_Execute();
            }
        }

        ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new DelegateCommand(SaveFileCommand_Execute, SaveFileCommand_CanExecute)
                  .ObservesProperty(() => Agents.Count));
            }
        }

        private void SaveFileCommand_Execute()
        {
            // Create an instance of the XmlSerializer class and specify the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Agent>));
            TextWriter writer = new StreamWriter(_filename);
            // Serialize all the agents.
            serializer.Serialize(writer, Agents);
            writer.Close();
        }

        private bool SaveFileCommand_CanExecute()
        {
            return (_filename != "") && (Agents.Count > 0);
        }

        ICommand _newFileCommand;
        public ICommand NewFileCommand
        {
            get { return _newFileCommand ?? (_newFileCommand = new DelegateCommand(NewFileCommand_Execute)); }
        }

        private void NewFileCommand_Execute()
        {
            MessageBoxResult res = MessageBox.Show("Any unsaved data will be lost. Are you sure you want to initiate a new file?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Agents.Clear();
                _filename = "";
            }
        }


        ICommand _openFileCommand;
        public ICommand OpenFileCommand
        {
            get { return _openFileCommand ?? (_openFileCommand = new DelegateCommand<string>(OpenFileCommand_Execute)); }
        }

        private void OpenFileCommand_Execute(string argFilename)
        {
            if (argFilename == "")
            {

                MessageBox.Show("You must enter a file name in the File Name textbox!", "Unable to save file",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                _filename = argFilename;
                var tempAgents = new ObservableCollection<Agent>();

                // Create an instance of the XmlSerializer class and specify the type of object to deserialize.
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Agent>));
                try
                {
                    TextReader reader = new StreamReader(_filename);
                    // Deserialize all the agents.
                    tempAgents = (ObservableCollection<Agent>)serializer.Deserialize(reader);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to open file", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Agents = tempAgents;
            }
        }

        ICommand _colorCommand;
        public ICommand ColorCommand
        {
            get
            {
                return _colorCommand ?? (_colorCommand = new
                  DelegateCommand<String>(ColorCommand_Execute));
            }
        }

        private void ColorCommand_Execute(String colorStr)
        {
            SolidColorBrush newBrush = SystemColors.WindowBrush; // Default color

            try
            {
                if (colorStr != null)
                {
                    if (colorStr != "Default")
                        newBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorStr));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Unknown color name, default color is used", "Program error!");
            }

            Application.Current.Resources["DynamicBg"] = newBrush;
        }

        Object _selectedSortOrder = "None";
        public Object SelectedSortOrder
        {
            get { return _selectedSortOrder; }
            set
            {
                SetProperty(ref _selectedSortOrder, value);
                ICollectionView cv = CollectionViewSource.GetDefaultView(Agents);
                var newSortOrder = value as ComboBoxItem;
                var sortDesc = new SortDescription(newSortOrder.Tag.ToString(), ListSortDirection.Ascending);
                if (cv != null)
                {
                    cv.SortDescriptions.Clear();
                    if (newSortOrder.Tag.ToString() != "None")
                        cv.SortDescriptions.Add(sortDesc);
                }
            }
        }

        #endregion
    }
}
