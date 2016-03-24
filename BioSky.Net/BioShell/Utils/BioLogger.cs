using BioContracts;
using BioService;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Google.Protobuf;
using System.IO;
using System.Collections.ObjectModel;
using BioModule.ViewModels;

namespace BioShell.Utils
{
  public class BioLogger : ILogger
  {
    public void LogException(_Exception exception)
    {
      if (exception == null)
        return;

      LogMessage(exception.Message);

      LogRecord record = new LogRecord();

      var ex = exception.GetBaseException();

      StackTrace st    = new StackTrace(ex, true);
      StackFrame frame = st.GetFrame(0);

      string exFilename = frame.GetFileName();
      var className = exFilename.Substring(exFilename.LastIndexOf('\\') + 1);

      record.FunctionName     = frame.GetMethod().Name;
      record.LineNumber       = frame.GetFileLineNumber();
      record.MessageType      = MessageType.Error;
      record.ClassName        = className;
      record.ExceptionMessage = exception.Message;
      record.DetectedTime     = DateTime.Now.Ticks;
      
      GetLogFile(record);
    }

    private void GetLogFile(LogRecord record)
    {
      DateTime now = DateTime.Now;

      string month = now.Month.ToString();

      if (now.Month < 10)
        month = "0" + now.Month;

      string date = now.Day.ToString() + "." + month + "." + now.Year.ToString();
      string fileName = "log " + date + ".txt";
      string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "Log\\";
      string path = directoryPath + fileName;

      Directory.CreateDirectory(directoryPath);

      using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))      
        record.WriteDelimitedTo(fs);      
    }

    public void LogMessage(string message)
    {
      Console.WriteLine(message);
    }
  }
}
