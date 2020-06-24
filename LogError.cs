using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using DataHelpers;
using BusinessObjects;
namespace DevTrackerLogging
{
    /// <summary>
    /// Log Errors and other info to DevTrkr Database
    /// </summary>
    public class LogError
    {
        const string eol = "\r\n";
        public LogError(Exception ex, bool showMsgBox, string moduleAndMethod)
        {
            try
            {
                string msg = $"Error Message from {moduleAndMethod}: " +
                            ex.Message + eol +
                           "Error Type: " + ex.GetType().ToString() + eol +
                           "Error Details: " + eol + ex.ToString();
                if (showMsgBox)
                    MessageBox.Show(msg, "Program Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    StackTrace st = new StackTrace(true);
                    msg += "Stack Trace: " + eol + st.ToString();
                }
                catch (Exception)
                {
                }

                WriteErrorLogToDB(new StringBuilder(msg), moduleAndMethod);
            }
            catch (Exception ex2)
            {
            }
        }

        public LogError(string err, bool showMsgBox, string moduleAndMethod)
        {
            string msg = $"Error Message from {moduleAndMethod}: " + err + eol;
            if (showMsgBox)
                MessageBox.Show(msg, "Program Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            WriteErrorLogToDB(new StringBuilder(msg), moduleAndMethod);
        }

        private Tuple<string, string> SplitModuleFromMethod(string moduleAndMethod)
        {
            string[] s = moduleAndMethod.Split('.');
            return Tuple.Create(s[0], s.Length > 1 ? s[1] : string.Empty);
        }

        private void WriteErrorLogToDB(StringBuilder sb, string moduleAndMethod)
        {
            Tuple<string, string> mm = SplitModuleFromMethod(moduleAndMethod);
            _ = new DHMisc().WriteErrorLog(new ErrorLog
            {
                Module = mm.Item1,
                Method = mm.Item2,
                Message = sb.ToString(),
                DateCreated = DateTime.Now,
                Machine = Environment.MachineName,
                Username = Environment.UserName
            });
        }
    }
}
