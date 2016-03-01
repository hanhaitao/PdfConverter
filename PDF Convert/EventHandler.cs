using System;
using System.Collections.Generic;
using System.Text;

namespace PDF_Convert
{
    public class ErrorMessageArgs : EventArgs
    {
        public ErrorMessageArgs(string message)
        {
            this.message = message;
        }
        public string message;
    }

    public delegate void ErrorMessageHandler(object sender, ErrorMessageArgs args);
}
