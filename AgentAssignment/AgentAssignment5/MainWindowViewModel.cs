using AgentAssignment5;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace AgentAssignment5
{
    public class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<Agent> agents;
        private string filename = "";
        private DispatcherTimer timer = new DispatcherTimer();
        private string filter;


        public MainWindowViewModel()
        {
            agents = new ObservableCollection<Agent>
            {
                #if DEBUG
                new Agent("001", "Nina", "Assasination", "UpperVolta"),
                new Agent("007", "James Bond", "Martinis", "North Korea")
                #endif
            };
            CurrentAgent = null;
            Specialities = new ObservableCollection<string>
            {
                "Assasination",
                "Bombs",
                "Martinis",
                "Low profile",
                "Spy"
            };

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            clock.Update();
        }

        #region Properties

        public ObservableCollection<Agent> Agents
        {
            get { return agents; }
            set { SetProperty(ref agents, value); }
        }

        Agent currentAgent = null;

        public Agent CurrentAgent
        {
            get { return currentAgent; }
            set
            {
                SetProperty(ref currentAgent, value);
            }
        }

        int currentIndex = -1;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                SetProperty(ref currentIndex, value);
            }
        }

        Clock clock = new Clock();
        public Clock Clock { get => clock; set => clock = value; }

        ObservableCollection<string> specialities;
        public ObservableCollection<string> Specialities
        {
            get { return specialities; }
            set
            {
                SetProperty(ref specialities, value);
            }
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


        public IReadOnlyCollection<string> FilterSpecialities
        {
            get
            {
                ObservableCollection<string> result = new ObservableCollection<string>();
                result.Add("All");
                result.AddRange(Specialities);
                return result;
            }
        }

        int currentSpecialityIndex = 0;

        public int CurrentSpecialityIndex
        {
            get { return currentSpecialityIndex; }
            set
            {
                if (currentSpecialityIndex != value)
                {
                    ICollectionView cv = CollectionViewSource.GetDefaultView(Agents);
                    currentSpecialityIndex = value;
                    if (currentSpecialityIndex == 0)
                        cv.Filter = null; // Index 0 is no filter (show all)
                    else
                    {
                        filter = Specialities[(currentSpecialityIndex-1)];
                        cv.Filter = CollectionViewSource_Filter;
                    }
                    RaisePropertyChanged();
                }
            }
        }

        private bool CollectionViewSource_Filter(object ag)
        {
            Agent agent = ag as Agent;
            return (agent.Speciality == filter);
        }
        #endregion

        #region Commands
        ICommand _PreviusCommand;
        public ICommand PreviusCommand
        {
            get
            {
                return _PreviusCommand ??
                (_PreviusCommand = new DelegateCommand(
                 PreviusCommandExecute, PreviusCommandCanExecute).ObservesProperty(() => CurrentIndex));
            }
        }

        private void PreviusCommandExecute()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
        }

        private bool PreviusCommandCanExecute()
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
                    var newAgent = new Agent();
                    Agents.Add(newAgent);
                    CurrentAgent = newAgent;
                    CurrentSpecialityIndex = 0;
                }));
            }
        }

        ICommand _deleteCommand;
        public ICommand DeleteAgentCommand => _deleteCommand ?? (_deleteCommand =
            new DelegateCommand(DeleteAgent, DeleteAgent_CanExecute)
                    .ObservesProperty(() => CurrentIndex));

        private void DeleteAgent()
        {
            Agents.Remove(CurrentAgent);
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

        ICommand _SaveAsCommand;
        public ICommand SaveAsCommand
        {
            get { return _SaveAsCommand ?? (_SaveAsCommand = new DelegateCommand<string>(SaveAsCommand_Execute)); }
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
                filename = argFilename;
                SaveFileCommand_Execute();
            }
        }

        ICommand _SaveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return _SaveCommand ?? (_SaveCommand = new DelegateCommand(SaveFileCommand_Execute, SaveFileCommand_CanExecute)
                  .ObservesProperty(() => Agents.Count));
            }
        }

        private void SaveFileCommand_Execute()
        {
            // Create an instance of the XmlSerializer class and specify the type of object to serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Agent>));
            TextWriter writer = new StreamWriter(filename);
            // Serialize all the agents.
            serializer.Serialize(writer, Agents);
            writer.Close();
        }

        private bool SaveFileCommand_CanExecute()
        {
            return (filename != "") && (Agents.Count > 0);
        }

        ICommand _NewFileCommand;
        public ICommand NewFileCommand
        {
            get { return _NewFileCommand ?? (_NewFileCommand = new DelegateCommand(NewFileCommand_Execute)); }
        }

        private void NewFileCommand_Execute()
        {
            MessageBoxResult res = MessageBox.Show("Any unsaved data will be lost. Are you sure you want to initiate a new file?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (res == MessageBoxResult.Yes)
            {
                Agents.Clear();
                filename = "";
            }
        }


        ICommand _OpenFileCommand;
        public ICommand OpenFileCommand
        {
            get { return _OpenFileCommand ?? (_OpenFileCommand = new DelegateCommand<string>(OpenFileCommand_Execute)); }
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
                filename = argFilename;
                var tempAgents = new ObservableCollection<Agent>();

                // Create an instance of the XmlSerializer class and specify the type of object to deserialize.
                XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Agent>));
                try
                {
                    TextReader reader = new StreamReader(filename);
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

        ICommand _ColorCommand;
        public ICommand ColorCommand
        {
            get
            {
                return _ColorCommand ?? (_ColorCommand = new
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

            Application.Current.Resources["BackgroundBrush"] = newBrush;
        }

        #endregion
    }
}
