using BioContracts.Abstract;
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

namespace BioSocket
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
        _binaryWriter  = new BinaryWriter(_networkStream);
       
      }
      catch ( Exception ex )
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

    int i = 1;
    public async void Read(int size)
    {
      if (size <= 0)
        return;

      try
      {
        byte[] bytes = new byte[size];
        await _networkStream.ReadAsync(bytes, 0, size);

        //CommandInformation

        string result = System.Text.Encoding.UTF8.GetString(bytes);
        Console.WriteLine(i + " " + result);
        i++;
      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
    }

    public void Serialize( object sender )
    {
      if (sender == null)
        return;

      try
      {       
        Serializer.Serialize(_networkStream, sender);
      }
      catch (Exception ex)
      {
        HandleException(ex);
      }
    }


    public void Deserialize(object sender)
    {

      try
      {  

        MethodInfo method = typeof(Serializer).GetMethod("Deserialize");
        MethodInfo generic = method.MakeGenericMethod(sender.GetType());
        sender = generic.Invoke(this, new object[] { _networkStream  });        
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

      while (Active)
      {
        if (_networkStream.CanRead)
        {
          Read(_client.Available);
        }


        Thread.Sleep(10);
      }
    }

    NetworkStream _networkStream;
    BinaryWriter  _binaryWriter ;

    private TcpClient _client;

  }
}
