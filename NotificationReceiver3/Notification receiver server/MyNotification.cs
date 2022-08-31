using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationReceiver3
{
    public class MyNotification
    {
        public string NotificationSender { get; set; }
        public string NotificationTitle { get; set; }
        public string NotificationContents { get; set; }
        public MyNotification(string s)
        {
            NotificationSender = s;
            NotificationTitle = s;
            NotificationContents = s;
        }
        public MyNotification(string app, string title, string text)
        {
            NotificationSender = app;
            NotificationTitle = title;
            NotificationContents = text;
        }
        public MyNotification(string[] info)
        {
            NotificationSender = info[0];
            NotificationTitle = info[1];
            NotificationContents = info[2];
        }
    }
}
