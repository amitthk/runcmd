using RunCmd.Common.Messaging;
using RunCmd.ViewModels;
using RunCmd.Views;
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

namespace RunCmd
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IMessageBus _messageBus;

        public MainWindow()
        {
            _messageBus = App.messageBus;
            InitializeComponent();
            _MainFrame.NavigationService.Navigate(new HomeView());
            _messageBus.Subscribe<NavigationMessage>(NavigateToPage);
        }

        private void NavigateToPage(NavigationMessage message)
        {
            ////GetQueryString isn't shown, but is simply a helper method for formatting the query string from the dictionary
            string queryStringParams = message.QueryStringParams == null ? "" : GetQueryString(message);

            string uri = string.Format("/Views/{0}.xaml{1}", message.PageName, queryStringParams);
            _MainFrame.NavigationService.Navigate(new Uri(uri, UriKind.Relative));
            //_MainFrame.NavigationService.Navigate(new Uri(message.PageName));
        }

        private string GetQueryString(NavigationMessage message)
        {
            string qstr = null;
            if (message.QueryStringParams != null)
            {
                qstr = string.Concat(message.QueryStringParams.Select(x => x.Key + "=" + x.Value).ToList<string>().ToArray());
                qstr = "?" + qstr;
            }
            return (qstr);
        }
    }
}
