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

      //directory
      string month = (now.Month < 10) ? "0" + now.Month : now.Month.ToString();

      string date          = String.Format("{0}\\{1}\\{2}\\", now.Year.ToString(), month, now.Day.ToString());

      string directoryPath = String.Format("{0}Log\\{1}", AppDomain.CurrentDomain.BaseDirectory, date);

      Directory.CreateDirectory(directoryPath);

      //file
      string fileDate = String.Format("{0}.{1}.{2}", now.Year.ToString(), month, now.Day.ToString());

      string fileName = String.Format("log {0}-{1}.txt", fileDate, 1);

      string[] filePaths = Directory.GetFiles(directoryPath);

      if (filePaths.Length != 0)
        fileName = String.Format("log {0}-{1}.txt", fileDate, GetFileNumber(filePaths));     

      string path = directoryPath + fileName;

      using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))      
        record.WriteDelimitedTo(fs);      
    }

    private long FILE_SIZE = 5242880; // 5Mb
    //private long FILE_SIZE = 100; // for test

    public int GetFileNumber(string[] filePaths)
    {
      string currentFilePath = "";
      int higherfile = 0;

      foreach (string filePath in filePaths)
      {
        char lastChar = filePath[filePath.Length - 5];
        int lastCharInt = (int)Char.GetNumericValue(lastChar);
        int currentfile = Math.Max(higherfile, lastCharInt);

        if (currentfile > higherfile)
        {
          currentFilePath = filePath;
          higherfile = currentfile;
        }          
      }

      FileInfo file = new FileInfo(currentFilePath);

      return (file.Length > FILE_SIZE) ? higherfile + 1 : higherfile;
    }

    public void LogMessage(string message)
    {
      Console.WriteLine(message);
    }
  }
}
