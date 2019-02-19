using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AgentAssignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Agent> _agents = new List<Agent>();
        public MainWindow()
        {
            InitializeComponent();
            _agents.Add(new Agent() { ID = "001", CodeName = "Nina", Specialty = "Assassination", Assignment = "UpperVolta" });
            _agents.Add(new Agent("007", "James Bond", "Martinis", "North Korea"));
            AgentGrid.DataContext = _agents;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
