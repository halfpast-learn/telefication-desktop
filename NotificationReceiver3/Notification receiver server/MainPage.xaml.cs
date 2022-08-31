using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Notifications;
using System.Diagnostics;
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NotificationReceiver3
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<MyNotification> NotificationContainer { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            NotificationContainer = new ObservableCollection<MyNotification>();
            for (int i = 0; i < 100; i++)
                NotificationContainer.Add(new MyNotification("Lorem"));

            Thread ServerThread = new Thread(new ThreadStart(
            MyServer.RunServer));
            ServerThread.Start();

            Thread UpdateThread = new Thread(new ThreadStart(
                CheckUpdatesAsync));
            UpdateThread.Start();

        }
        private void ClickClear(object sender, RoutedEventArgs e)
        {
            NotificationContainer.Clear();

            ToastNotificationManager.History.Clear();
        }
        public void SendToastNotification(string app, string title, string text)
        {
            var template = ToastTemplateType.ToastText02;
            var toastXml = ToastNotificationManager.GetTemplateContent(template);

            var toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(app));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(title + ": " + text));
            Debug.WriteLine(toastXml);

            var toast = new ToastNotification(toastXml);
            toast.Tag = "ligmas";

            // And create the toast notification
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public async void CheckUpdatesAsync()
        {
            while (true)
            {
                if (MyServer.StatusChanged)
                {
                    List<string> notifications = MyServer.GetNotificationsList();
                    foreach (string info in notifications)
                    {
                        string[] notificationInfo = info.Split("\n");
                        if (notificationInfo[1] == "null" || notificationInfo[2] == "null")
                            continue;
                        MyNotification notification = new MyNotification(notificationInfo);
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            NotificationContainer.Add(notification);
                            SendToastNotification(notificationInfo[0], notificationInfo[1], notificationInfo[2]);
                        });
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

    }
}
