using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Serialization;
using AgentAssignment;

namespace AgentAssignment3
{
    class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<Agent> _agents;
        private string _filename = "";
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindowViewModel()
        {
            _agents = new ObservableCollection<Agent>
            {
                //#if DEBUG
                new Agent("001", "Nina", "Assassination", "UpperVolta"),
                new Agent("007", "James Bond", "Martinis", "North Korea")
                //#endif
            };
            CurrentAgent = null;
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Start();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            clock.Update();
        }

        Clock clock = new Clock();
        public Clock Clock { get => clock; set => clock = value; }

        public ObservableCollection<Agent> Agents
        {
            get => _agents;
            set => SetProperty(ref _agents, value);
        }

        private Agent _currentAgent = null;

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

        private ICommand exitButtonCommand;

        public ICommand ExitButtonCommand
        {
            get
            {
                return exitButtonCommand ?? (exitButtonCommand = new DelegateCommand(() =>
                {
                    if (Application.Current.MainWindow != null) Application.Current.MainWindow.Close();
                }));
            }
        }

        ICommand previousCommand;
        public ICommand PreviousCommand
        {
            get
            {
                return previousCommand ?? (previousCommand = new DelegateCommand(
                               PreviousCommandExecute, 
                           PreviousCommandCanExecute)
                           .ObservesProperty(() => CurrentIndex));
            }
        }
        private void PreviousCommandExecute()
        {
            if (CurrentIndex > 0)
                --CurrentIndex;
        }
        private bool PreviousCommandCanExecute()
        {
            return CurrentIndex > 0;
        }

        ICommand _nextCommand;
        public ICommand NextCommand => _nextCommand ?? (_nextCommand = new DelegateCommand(
                                           () => ++CurrentIndex,
                                           () => CurrentIndex < (Agents.Count - 1))
                                           .ObservesProperty(() => CurrentIndex));

        ICommand _newCommand;
        public ICommand NewCommand
        {
            get
            {
                return _newCommand ?? (_newCommand = new DelegateCommand(() =>
                {
                    Agents.Add(new Agent("xx", "New Agent", "N/A", "N/A"));
                }));
            }
        }

        ICommand _delCommand;
        public ICommand DeleteCommand
        {
            get
            {
                return _delCommand ?? (_delCommand = new DelegateCommand(() =>
                {
                    Agents.Remove(CurrentAgent);
                    CurrentIndex = Agents.Count - 1;
                }));
            }
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
                _filename = argFilename;
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
            TextWriter writer = new StreamWriter(_filename);
            // Serialize all the agents.
            serializer.Serialize(writer, Agents);
            writer.Close();
        }

        private bool SaveFileCommand_CanExecute()
        {
            return (_filename != "") && (Agents.Count > 0);
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
            if (res != MessageBoxResult.Yes) return;
            Agents.Clear();
            _filename = "";
        }


        ICommand _OpenFileCommand;
        public ICommand OpenFileCommand => _OpenFileCommand ?? (_OpenFileCommand = new DelegateCommand<string>(OpenFileCommand_Execute));

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

                // We have to insert the agents in the existing collection. 
                //Agents.Clear();
                //foreach (var agent in tempAgents)
                //    Add(agent);
            }
        }

        private ICommand _changeColorCommand;

        public ICommand ChangeColorCommand =>
            _changeColorCommand ?? (
                _changeColorCommand = new DelegateCommand<string>((ColorCommandExecute)));

        private static void ColorCommandExecute(string color)
        {
            Debug.WriteLine("TEST");
            SolidColorBrush _color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            Application.Current.Resources["DynamicBg"] = _color;
        }
    }
}
