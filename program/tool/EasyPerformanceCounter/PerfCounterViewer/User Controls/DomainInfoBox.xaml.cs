using EasyPerformanceCounter;
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
using System.Windows.Threading;

namespace PerfCounterViewer.User_Controls
{
    /// <summary>
    /// Interaction logic for DomainCounter.xaml
    /// </summary>
    public partial class DomainInfoBox : UserControl
    {
        public CounterSubscriber CounterSubscriber { get; set; }

        public DomainInfoBox()
        {
            InitializeComponent();
        }

        private void counterPanel_Loaded(object sender, RoutedEventArgs e)
        {
            domainName.Content = CounterSubscriber.Domain;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick+= UpdateInfo;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void UpdateInfo(object sender, EventArgs s)
        {
            CounterSubscriber.FetchAllCounter(out var mappedFileCounters);
            {
                counterPanel.Children.Clear();

                foreach (var counter in mappedFileCounters)
                {
                    counterPanel.Children.Add(new Label() { Content = $"{counter.Name}={counter.Value}" });
                }
            }
        }
    }
}
