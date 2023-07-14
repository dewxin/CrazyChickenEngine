using EasyPerformanceCounter;
using PerfCounterViewer.User_Controls;
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

namespace PerfCounterViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PerfCounterManager PerfCounterManager { get; set; } = new PerfCounterManager();

        Dictionary<string, CounterSubscriber> checkBox2SubDict = new Dictionary<string, CounterSubscriber>();
        Dictionary<CheckBox, DomainInfoBox> checkBox2DomainBoxDict= new Dictionary<CheckBox, DomainInfoBox>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void domainPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if(PerfCounterManager.UpdateDomain())
            {
                domainPanel.Children.Clear();

                foreach(var subscriber in PerfCounterManager.GetCounterSubscribers())
                {
                    var checkBox = new CheckBox() { Content = subscriber.Domain };

                    checkBox.Checked += OnBoxChecked;
                    checkBox.Unchecked += OnBoxUnChecked;
                    domainPanel.Children.Add(checkBox);

                    checkBox2SubDict.Add(subscriber.Domain, subscriber);
                }
            }
        }

        private void OnBoxChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var str = checkBox.Content.ToString();
            var subscriber = checkBox2SubDict[str];

            var domainInfoBox = new DomainInfoBox();
            domainInfoBox.CounterSubscriber= subscriber;
            container.Children.Add(domainInfoBox);

            checkBox2DomainBoxDict.Add(checkBox, domainInfoBox);
        }

        private void OnBoxUnChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            var infoBox = checkBox2DomainBoxDict[checkBox];

            container.Children.Remove(infoBox);
            checkBox2DomainBoxDict.Remove(checkBox);
        }

    }
}
