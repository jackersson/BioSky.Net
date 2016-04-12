using BioContracts;
using BioService;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Google.Protobuf;
using System.IO;

namespace BioShell.Utils
{
  public class BioLogger : ILogger
  {
    public void LogException(_Exception exception)
    {
      if (exception == null)
        return;

      LogMessage(exception.Message);      

      SaveLogFile(GetLogRecordFromException(exception));
    }

    private LogRecord GetLogRecordFromException(_Exception exception)
    {
      LogRecord record = new LogRecord();

      var ex = exception.GetBaseException();

      StackTrace st    = new StackTrace(ex, true);
      StackFrame frame = st.GetFrame(0);

      string exFilename = frame.GetFileName();
      string className = "";
      if (exFilename != null)
        className = exFilename.Substring(exFilename.LastIndexOf('\\') + 1);
      else
        className = ex.Source;

      record.FunctionName     = frame.GetMethod().Name;
      record.LineNumber       = frame.GetFileLineNumber();
      record.MessageType      = MessageType.Error;
      record.ClassName        = className;
      record.ExceptionMessage = exception.Message;
      record.DetectedTime     = DateTime.Now.Ticks;

      return record;
    }
    
    private void SaveLogFile(LogRecord record)
    {
      try
      {
        string filePath = GetFilePath();

        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
          record.WriteDelimitedTo(fs);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
    }

    private long FILE_SIZE = 5242880; // 5Mb
    //private long FILE_SIZE = 100; // for test

    public string GetFilePath()
    {
      string currentFilePath = "";
      DateTime higherfile = new DateTime();
      string fileFormat = ".txt";
      DateTime now = DateTime.Now;

      string month = (now.Month < 10) ? "0" + now.Month : now.Month.ToString();

      string directoryPath = string.Format("{0}Log\\{1}\\{2}\\{3}\\"
                                          , AppDomain.CurrentDomain.BaseDirectory
                                          , now.Year.ToString()
                                          , month
                                          , now.Day.ToString());

      Directory.CreateDirectory(directoryPath);

      string[] filePaths = Directory.GetFiles(directoryPath);

      if (filePaths.Length <= 0)
        return directoryPath + string.Format("{0}{1}", DateTime.Now.Ticks, fileFormat);


      foreach (string filePath in filePaths)
      {
        DateTime creationTime = File.GetCreationTime(filePath);

        if (creationTime > higherfile)
        {
          currentFilePath = filePath;
          higherfile = creationTime;
        }
      }

      FileInfo file = new FileInfo(currentFilePath);

      if (file.Length > FILE_SIZE)
        return directoryPath + string.Format("{0}{1}", DateTime.Now.Ticks, fileFormat);
      else
        return currentFilePath;
    }    

    public void LogMessage(string message)
    {
      Console.WriteLine(message);
    }
  }
}
