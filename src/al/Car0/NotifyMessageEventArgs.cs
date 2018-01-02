using System;
using System.Collections.Generic;
using System.Text;


namespace Car0
{
    public delegate void NotifyMessageEventHandler(object sender, NotifyMessageEventArgs e);
    public class NotifyMessageEventArgs : EventArgs
    {
        private int timeout = 1000;
        private string title = String.Empty;
        private string message = String.Empty;
        private System.Windows.Forms.ToolTipIcon icon = System.Windows.Forms.ToolTipIcon.None;

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        public System.Windows.Forms.ToolTipIcon Icon
        {
            get { return icon; }
            set { icon = value; }
        }
    }
}
