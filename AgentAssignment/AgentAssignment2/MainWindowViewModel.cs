using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;

namespace AgentAssignment2
{
    class MainWindowViewModel : BindableBase
    {
        public ObservableCollection<Agent> Agents { get; }
        public MainWindowViewModel()
        {
            Agents = new ObservableCollection<Agent>
            {
                new Agent("28", "Mellisa","Being a super duper cringe virgin", "Nak nogle tards"),
                new Agent("34", "James Bond","Being a super duper cringe virgin", "Nak nogle tards"),
                new Agent("38", "Mellisa","Being a super duper cringe virgin", "Nak nogle tards")
            };
        }

        private Agent _currentAgent = null;

        public Agent CurrentAgent
        {
            get => _currentAgent;
            set => _currentAgent = value;
        }
        int currentIndex = -1;

        public int CurrentIndex
        {
            get => currentIndex;
            set => currentIndex = value;
        }

        void AgentCommandHandler()
        {
            //CurrentAgent
            MessageBox.Show(
                string.Format($"LOL, {CurrentAgent.CodeName}, ID {CurrentAgent.ID}!"),
                "Crazy agent");
        }

        ICommand previousCommand;
        public ICommand PreviousCommand
        {
            get
            {
                return previousCommand ??
                       (previousCommand = new DelegateCommand(
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
                    Agents.Add(new Agent("21", "New Agent", "N/A", "N/A"));
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
    }
}
