using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AspNet.SignalR.Client.Silverlight.Sample.ViewModels;

namespace Microsoft.AspNet.SignalR.Client.Silverlight.Sample
{
    public partial class MainPage : UserControl
    {
        Connection connection;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;

            connection = new Connection("http://localhost:40476/raw-connection");

            connection.Received += data =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModel.Items.Add(data);
                });
            };

            connection.Error += ex =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    var aggEx = (AggregateException)ex;
                    App.ViewModel.Items.Add(aggEx.InnerException.ToString());
                });
            };

            connection.Reconnected += () =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    App.ViewModel.Items.Add("Connection restored");
                });
            };

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            connection.Start(new Microsoft.AspNet.SignalR.Client.Transports.LongPollingTransport()).ContinueWith(task =>
            {
                var ex = task.Exception.InnerExceptions[0];
                App.ViewModel.Items.Add(ex.Message);
            },
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            scheduler);
        }

        private void btSend_Click(object sender, RoutedEventArgs e)
        {
            connection.Send("This is a simple test");
        }
    }
}
