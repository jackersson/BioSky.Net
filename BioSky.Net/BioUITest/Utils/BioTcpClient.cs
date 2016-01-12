using BioContracts.Abstract;
using Google.Protobuf;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

using System.Threading.Tasks;

using BioContracts;

namespace BioUITest.Utils
{
  public class BioTcpClient : Threadable
  {

    public BioTcpClient() : base()
    {
      _client = new TcpClient();

     

    }

    public BioTcpClient(string ipAddress, int portNumber) : base()
    {
      _client = new TcpClient();
      Connect(ipAddress, portNumber);
    }

    public void Connect(string ipAddress, int portNumber)
    {
      try
      {
        _client.Connect(IPAddress.Parse(ipAddress), portNumber);

        _networkStream = _client.GetStream();
        _binaryWriter = new BinaryWriter(_networkStream);

        Faces = new AsyncObservableCollection<FaceInformation>();

      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
    }

    public void Close()
    {
      try
      {
        _client.Close();
        _binaryWriter.Close();
        _networkStream.Close();


      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
    }


    public void Write(byte[] bytes, int size)
    {
      if (_binaryWriter == null)
        return;

      try
      {
        _networkStream.WriteAsync(bytes, 0, size);
        //_binaryWriter.(bytes);        
      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
    }
    const int COMMAND_SIZE = 4;
   
    public void Read(int size)
    {
      if (size <= 0)
        return;
    
      try
      {
        byte[] bytes = new byte[size];
        int bytes_read = _networkStream.Read(bytes, 0, size);


        
       int bytes_to_read = BitConverter.ToInt32(bytes, 0);
        Console.WriteLine(bytes_to_read);
             
        if (bytes_read < COMMAND_SIZE)
          return;

        int faceCount = 0;
        int bufferToReadSize = 0;
        CommandInformation ci = CommandInformation.Parser.ParseFrom(bytes);
        if (ci != null)
        {        
          faceCount = ci.PacketLength;
          bufferToReadSize = ci.PacketSize;
          Faces.Clear();
          if (bufferToReadSize > 0)
          {
            byte[] packetBuffer = new byte[bufferToReadSize];
            int packetSizeRead = 0;
            while (packetSizeRead < bufferToReadSize)
            {
              int bytesRead = _networkStream.Read(packetBuffer, packetSizeRead, packetBuffer.Length - packetSizeRead);
              packetSizeRead += bytesRead;
              if (bytesRead == 0)
                break;   // The socket was closed
            }

            if (packetSizeRead >= bufferToReadSize)
            {
              using (MemoryStream ms = new MemoryStream(packetBuffer))
              {
              int parsedBufferSize = 0;
                while (parsedBufferSize < bufferToReadSize)
                {

               // int toRead = packetBuffer.Length - parsedBufferSize;
                  FaceInformation fi = FaceInformation.Parser.ParseDelimitedFrom(ms);
                parsedBufferSize += fi.CalculateSize();
                  if (fi == null)
                    break;
                  Faces.Add(fi);

                  if (Faces.Count > 3)
                    break;
                  Console.WriteLine("read from buffer: \n" + fi + " " + Faces.Count);
                }
              }
            }
          }            
        }
        
        //_client.Close();
      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
    }

    


    public void Serialize(object sender)
    {
      if (sender == null)
        return;

      try
      {
        CommandInformation ci = (CommandInformation)sender;
        ci.WriteTo(_networkStream);
        //Serializer.Serialize(_networkStream, sender);
      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
    }

    private AsyncObservableCollection<FaceInformation> _faces;
    public AsyncObservableCollection<FaceInformation> Faces
    {
      get { return _faces; }
      set
      {
        if (_faces != value)
        {
          _faces = value;         
        }
      }
    }


    public void Deserialize(object sender)
    {

      try
      {

        MethodInfo method = typeof(Serializer).GetMethod("Deserialize");
        MethodInfo generic = method.MakeGenericMethod(sender.GetType());
        sender = generic.Invoke(this, new object[] { _networkStream });
      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
    }


    public void HandleException(Exception ex)
    {
      Console.Write(ex.Message);
    }

    public override void Run()
    {
      Active = true;
      CodedInputStream st = CodedInputStream.CreateWithLimits(_networkStream, 4, 1);
      int i = 0;
      while (Active)
      {
        if (_networkStream != null &&  _networkStream.CanRead)
        {
          Read(_client.Available);
          try
          {
            //int nextSize = st.ReadInt32();

            ////if (nextSize >= 4)
            //{
              //CommandInformation ci = CommandInformation.Parser.ParseFrom(st);
             // Console.WriteLine(ci + " " + _client.Available);
              //i++;
            //}             

            //Console.WriteLine(nextSize);
          }
          catch
          {

          }
          //int bytes_read = _networkStream.Read(bytes, 0, size);
          //CommandInformation ci = CommandInformation.Parser.ParseFrom(st);

        }


        //Thread.Sleep(10);
      }
    }

    NetworkStream _networkStream;
    BinaryWriter _binaryWriter;

    private TcpClient _client;

  }
}
