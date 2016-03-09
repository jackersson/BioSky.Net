// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: bio_service.proto
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace BioService {
  public static class BiometricFacialSevice
  {
    static readonly string __ServiceName = "BioService.BiometricFacialSevice";

    static readonly Marshaller<global::BioService.SocketConfiguration> __Marshaller_SocketConfiguration = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.SocketConfiguration.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.Response> __Marshaller_Response = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.Response.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.EnrollmentData> __Marshaller_EnrollmentData = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.EnrollmentData.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.EnrollmentFeedback> __Marshaller_EnrollmentFeedback = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.EnrollmentFeedback.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.VerificationData> __Marshaller_VerificationData = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.VerificationData.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.VerificationFeedback> __Marshaller_VerificationFeedback = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.VerificationFeedback.Parser.ParseFrom);

    static readonly Method<global::BioService.SocketConfiguration, global::BioService.Response> __Method_AddSocket = new Method<global::BioService.SocketConfiguration, global::BioService.Response>(
        MethodType.Unary,
        __ServiceName,
        "AddSocket",
        __Marshaller_SocketConfiguration,
        __Marshaller_Response);

    static readonly Method<global::BioService.EnrollmentData, global::BioService.EnrollmentFeedback> __Method_EnrollFace = new Method<global::BioService.EnrollmentData, global::BioService.EnrollmentFeedback>(
        MethodType.ServerStreaming,
        __ServiceName,
        "EnrollFace",
        __Marshaller_EnrollmentData,
        __Marshaller_EnrollmentFeedback);

    static readonly Method<global::BioService.VerificationData, global::BioService.VerificationFeedback> __Method_VerifyFace = new Method<global::BioService.VerificationData, global::BioService.VerificationFeedback>(
        MethodType.ServerStreaming,
        __ServiceName,
        "VerifyFace",
        __Marshaller_VerificationData,
        __Marshaller_VerificationFeedback);

    // service descriptor
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::BioService.BioService.Descriptor.Services[0]; }
    }

    // client interface
    public interface IBiometricFacialSeviceClient
    {
      global::BioService.Response AddSocket(global::BioService.SocketConfiguration request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.Response AddSocket(global::BioService.SocketConfiguration request, CallOptions options);
      AsyncUnaryCall<global::BioService.Response> AddSocketAsync(global::BioService.SocketConfiguration request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.Response> AddSocketAsync(global::BioService.SocketConfiguration request, CallOptions options);
      AsyncServerStreamingCall<global::BioService.EnrollmentFeedback> EnrollFace(global::BioService.EnrollmentData request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncServerStreamingCall<global::BioService.EnrollmentFeedback> EnrollFace(global::BioService.EnrollmentData request, CallOptions options);
      AsyncServerStreamingCall<global::BioService.VerificationFeedback> VerifyFace(global::BioService.VerificationData request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncServerStreamingCall<global::BioService.VerificationFeedback> VerifyFace(global::BioService.VerificationData request, CallOptions options);
    }

    // server-side interface
    public interface IBiometricFacialSevice
    {
      Task<global::BioService.Response> AddSocket(global::BioService.SocketConfiguration request, ServerCallContext context);
      Task EnrollFace(global::BioService.EnrollmentData request, IServerStreamWriter<global::BioService.EnrollmentFeedback> responseStream, ServerCallContext context);
      Task VerifyFace(global::BioService.VerificationData request, IServerStreamWriter<global::BioService.VerificationFeedback> responseStream, ServerCallContext context);
    }

    // client stub
    public class BiometricFacialSeviceClient : ClientBase, IBiometricFacialSeviceClient
    {
      public BiometricFacialSeviceClient(Channel channel) : base(channel)
      {
      }
      public global::BioService.Response AddSocket(global::BioService.SocketConfiguration request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_AddSocket, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.Response AddSocket(global::BioService.SocketConfiguration request, CallOptions options)
      {
        var call = CreateCall(__Method_AddSocket, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Response> AddSocketAsync(global::BioService.SocketConfiguration request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_AddSocket, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Response> AddSocketAsync(global::BioService.SocketConfiguration request, CallOptions options)
      {
        var call = CreateCall(__Method_AddSocket, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncServerStreamingCall<global::BioService.EnrollmentFeedback> EnrollFace(global::BioService.EnrollmentData request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_EnrollFace, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncServerStreamingCall(call, request);
      }
      public AsyncServerStreamingCall<global::BioService.EnrollmentFeedback> EnrollFace(global::BioService.EnrollmentData request, CallOptions options)
      {
        var call = CreateCall(__Method_EnrollFace, options);
        return Calls.AsyncServerStreamingCall(call, request);
      }
      public AsyncServerStreamingCall<global::BioService.VerificationFeedback> VerifyFace(global::BioService.VerificationData request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_VerifyFace, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncServerStreamingCall(call, request);
      }
      public AsyncServerStreamingCall<global::BioService.VerificationFeedback> VerifyFace(global::BioService.VerificationData request, CallOptions options)
      {
        var call = CreateCall(__Method_VerifyFace, options);
        return Calls.AsyncServerStreamingCall(call, request);
      }
    }

    // creates service definition that can be registered with a server
    public static ServerServiceDefinition BindService(IBiometricFacialSevice serviceImpl)
    {
      return ServerServiceDefinition.CreateBuilder(__ServiceName)
          .AddMethod(__Method_AddSocket, serviceImpl.AddSocket)
          .AddMethod(__Method_EnrollFace, serviceImpl.EnrollFace)
          .AddMethod(__Method_VerifyFace, serviceImpl.VerifyFace).Build();
    }

    // creates a new client
    public static BiometricFacialSeviceClient NewClient(Channel channel)
    {
      return new BiometricFacialSeviceClient(channel);
    }

  }
  public static class BiometricDatabaseSevice
  {
    static readonly string __ServiceName = "BioService.BiometricDatabaseSevice";

    static readonly Marshaller<global::BioService.QueryPersons> __Marshaller_QueryPersons = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.QueryPersons.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.PersonList> __Marshaller_PersonList = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.PersonList.Parser.ParseFrom);

    static readonly Method<global::BioService.QueryPersons, global::BioService.PersonList> __Method_PersonSelect = new Method<global::BioService.QueryPersons, global::BioService.PersonList>(
        MethodType.Unary,
        __ServiceName,
        "PersonSelect",
        __Marshaller_QueryPersons,
        __Marshaller_PersonList);

    static readonly Method<global::BioService.PersonList, global::BioService.PersonList> __Method_PersonUpdate = new Method<global::BioService.PersonList, global::BioService.PersonList>(
        MethodType.Unary,
        __ServiceName,
        "PersonUpdate",
        __Marshaller_PersonList,
        __Marshaller_PersonList);

    // service descriptor
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::BioService.BioService.Descriptor.Services[1]; }
    }

    // client interface
    public interface IBiometricDatabaseSeviceClient
    {
      global::BioService.PersonList PersonSelect(global::BioService.QueryPersons request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.PersonList PersonSelect(global::BioService.QueryPersons request, CallOptions options);
      AsyncUnaryCall<global::BioService.PersonList> PersonSelectAsync(global::BioService.QueryPersons request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.PersonList> PersonSelectAsync(global::BioService.QueryPersons request, CallOptions options);
      global::BioService.PersonList PersonUpdate(global::BioService.PersonList request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.PersonList PersonUpdate(global::BioService.PersonList request, CallOptions options);
      AsyncUnaryCall<global::BioService.PersonList> PersonUpdateAsync(global::BioService.PersonList request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.PersonList> PersonUpdateAsync(global::BioService.PersonList request, CallOptions options);
    }

    // server-side interface
    public interface IBiometricDatabaseSevice
    {
      Task<global::BioService.PersonList> PersonSelect(global::BioService.QueryPersons request, ServerCallContext context);
      Task<global::BioService.PersonList> PersonUpdate(global::BioService.PersonList request, ServerCallContext context);
    }

    // client stub
    public class BiometricDatabaseSeviceClient : ClientBase, IBiometricDatabaseSeviceClient
    {
      public BiometricDatabaseSeviceClient(Channel channel) : base(channel)
      {
      }
      public global::BioService.PersonList PersonSelect(global::BioService.QueryPersons request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_PersonSelect, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.PersonList PersonSelect(global::BioService.QueryPersons request, CallOptions options)
      {
        var call = CreateCall(__Method_PersonSelect, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.PersonList> PersonSelectAsync(global::BioService.QueryPersons request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_PersonSelect, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.PersonList> PersonSelectAsync(global::BioService.QueryPersons request, CallOptions options)
      {
        var call = CreateCall(__Method_PersonSelect, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public global::BioService.PersonList PersonUpdate(global::BioService.PersonList request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_PersonUpdate, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.PersonList PersonUpdate(global::BioService.PersonList request, CallOptions options)
      {
        var call = CreateCall(__Method_PersonUpdate, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.PersonList> PersonUpdateAsync(global::BioService.PersonList request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_PersonUpdate, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.PersonList> PersonUpdateAsync(global::BioService.PersonList request, CallOptions options)
      {
        var call = CreateCall(__Method_PersonUpdate, options);
        return Calls.AsyncUnaryCall(call, request);
      }
    }

    // creates service definition that can be registered with a server
    public static ServerServiceDefinition BindService(IBiometricDatabaseSevice serviceImpl)
    {
      return ServerServiceDefinition.CreateBuilder(__ServiceName)
          .AddMethod(__Method_PersonSelect, serviceImpl.PersonSelect)
          .AddMethod(__Method_PersonUpdate, serviceImpl.PersonUpdate).Build();
    }

    // creates a new client
    public static BiometricDatabaseSeviceClient NewClient(Channel channel)
    {
      return new BiometricDatabaseSeviceClient(channel);
    }

  }
}
#endregion
