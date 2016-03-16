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
    static readonly Marshaller<global::BioService.Person> __Marshaller_Person = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.Person.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.Card> __Marshaller_Card = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.Card.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.RawIndexes> __Marshaller_RawIndexes = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.RawIndexes.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.QueryPhoto> __Marshaller_QueryPhoto = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.QueryPhoto.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.PhotoList> __Marshaller_PhotoList = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.PhotoList.Parser.ParseFrom);
    static readonly Marshaller<global::BioService.Photo> __Marshaller_Photo = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioService.Photo.Parser.ParseFrom);

    static readonly Method<global::BioService.QueryPersons, global::BioService.PersonList> __Method_PersonSelect = new Method<global::BioService.QueryPersons, global::BioService.PersonList>(
        MethodType.Unary,
        __ServiceName,
        "PersonSelect",
        __Marshaller_QueryPersons,
        __Marshaller_PersonList);

    static readonly Method<global::BioService.Person, global::BioService.Person> __Method_AddPerson = new Method<global::BioService.Person, global::BioService.Person>(
        MethodType.Unary,
        __ServiceName,
        "AddPerson",
        __Marshaller_Person,
        __Marshaller_Person);

    static readonly Method<global::BioService.Person, global::BioService.Person> __Method_UpdatePerson = new Method<global::BioService.Person, global::BioService.Person>(
        MethodType.Unary,
        __ServiceName,
        "UpdatePerson",
        __Marshaller_Person,
        __Marshaller_Person);

    static readonly Method<global::BioService.Person, global::BioService.Person> __Method_RemovePerson = new Method<global::BioService.Person, global::BioService.Person>(
        MethodType.Unary,
        __ServiceName,
        "RemovePerson",
        __Marshaller_Person,
        __Marshaller_Person);

    static readonly Method<global::BioService.Card, global::BioService.Card> __Method_AddCard = new Method<global::BioService.Card, global::BioService.Card>(
        MethodType.Unary,
        __ServiceName,
        "AddCard",
        __Marshaller_Card,
        __Marshaller_Card);

    static readonly Method<global::BioService.RawIndexes, global::BioService.RawIndexes> __Method_RemoveCards = new Method<global::BioService.RawIndexes, global::BioService.RawIndexes>(
        MethodType.Unary,
        __ServiceName,
        "RemoveCards",
        __Marshaller_RawIndexes,
        __Marshaller_RawIndexes);

    static readonly Method<global::BioService.QueryPhoto, global::BioService.PhotoList> __Method_SelectPhotos = new Method<global::BioService.QueryPhoto, global::BioService.PhotoList>(
        MethodType.Unary,
        __ServiceName,
        "SelectPhotos",
        __Marshaller_QueryPhoto,
        __Marshaller_PhotoList);

    static readonly Method<global::BioService.Photo, global::BioService.Photo> __Method_AddPhoto = new Method<global::BioService.Photo, global::BioService.Photo>(
        MethodType.Unary,
        __ServiceName,
        "AddPhoto",
        __Marshaller_Photo,
        __Marshaller_Photo);

    static readonly Method<global::BioService.Photo, global::BioService.Photo> __Method_SetThumbnail = new Method<global::BioService.Photo, global::BioService.Photo>(
        MethodType.Unary,
        __ServiceName,
        "SetThumbnail",
        __Marshaller_Photo,
        __Marshaller_Photo);

    static readonly Method<global::BioService.RawIndexes, global::BioService.RawIndexes> __Method_RemovePhotos = new Method<global::BioService.RawIndexes, global::BioService.RawIndexes>(
        MethodType.Unary,
        __ServiceName,
        "RemovePhotos",
        __Marshaller_RawIndexes,
        __Marshaller_RawIndexes);

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
      global::BioService.Person AddPerson(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.Person AddPerson(global::BioService.Person request, CallOptions options);
      AsyncUnaryCall<global::BioService.Person> AddPersonAsync(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.Person> AddPersonAsync(global::BioService.Person request, CallOptions options);
      global::BioService.Person UpdatePerson(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.Person UpdatePerson(global::BioService.Person request, CallOptions options);
      AsyncUnaryCall<global::BioService.Person> UpdatePersonAsync(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.Person> UpdatePersonAsync(global::BioService.Person request, CallOptions options);
      global::BioService.Person RemovePerson(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.Person RemovePerson(global::BioService.Person request, CallOptions options);
      AsyncUnaryCall<global::BioService.Person> RemovePersonAsync(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.Person> RemovePersonAsync(global::BioService.Person request, CallOptions options);
      global::BioService.Card AddCard(global::BioService.Card request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.Card AddCard(global::BioService.Card request, CallOptions options);
      AsyncUnaryCall<global::BioService.Card> AddCardAsync(global::BioService.Card request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.Card> AddCardAsync(global::BioService.Card request, CallOptions options);
      global::BioService.RawIndexes RemoveCards(global::BioService.RawIndexes request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.RawIndexes RemoveCards(global::BioService.RawIndexes request, CallOptions options);
      AsyncUnaryCall<global::BioService.RawIndexes> RemoveCardsAsync(global::BioService.RawIndexes request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.RawIndexes> RemoveCardsAsync(global::BioService.RawIndexes request, CallOptions options);
      global::BioService.PhotoList SelectPhotos(global::BioService.QueryPhoto request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.PhotoList SelectPhotos(global::BioService.QueryPhoto request, CallOptions options);
      AsyncUnaryCall<global::BioService.PhotoList> SelectPhotosAsync(global::BioService.QueryPhoto request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.PhotoList> SelectPhotosAsync(global::BioService.QueryPhoto request, CallOptions options);
      global::BioService.Photo AddPhoto(global::BioService.Photo request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.Photo AddPhoto(global::BioService.Photo request, CallOptions options);
      AsyncUnaryCall<global::BioService.Photo> AddPhotoAsync(global::BioService.Photo request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.Photo> AddPhotoAsync(global::BioService.Photo request, CallOptions options);
      global::BioService.Photo SetThumbnail(global::BioService.Photo request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.Photo SetThumbnail(global::BioService.Photo request, CallOptions options);
      AsyncUnaryCall<global::BioService.Photo> SetThumbnailAsync(global::BioService.Photo request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.Photo> SetThumbnailAsync(global::BioService.Photo request, CallOptions options);
      global::BioService.RawIndexes RemovePhotos(global::BioService.RawIndexes request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      global::BioService.RawIndexes RemovePhotos(global::BioService.RawIndexes request, CallOptions options);
      AsyncUnaryCall<global::BioService.RawIndexes> RemovePhotosAsync(global::BioService.RawIndexes request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncUnaryCall<global::BioService.RawIndexes> RemovePhotosAsync(global::BioService.RawIndexes request, CallOptions options);
    }

    // server-side interface
    public interface IBiometricDatabaseSevice
    {
      Task<global::BioService.PersonList> PersonSelect(global::BioService.QueryPersons request, ServerCallContext context);
      Task<global::BioService.Person> AddPerson(global::BioService.Person request, ServerCallContext context);
      Task<global::BioService.Person> UpdatePerson(global::BioService.Person request, ServerCallContext context);
      Task<global::BioService.Person> RemovePerson(global::BioService.Person request, ServerCallContext context);
      Task<global::BioService.Card> AddCard(global::BioService.Card request, ServerCallContext context);
      Task<global::BioService.RawIndexes> RemoveCards(global::BioService.RawIndexes request, ServerCallContext context);
      Task<global::BioService.PhotoList> SelectPhotos(global::BioService.QueryPhoto request, ServerCallContext context);
      Task<global::BioService.Photo> AddPhoto(global::BioService.Photo request, ServerCallContext context);
      Task<global::BioService.Photo> SetThumbnail(global::BioService.Photo request, ServerCallContext context);
      Task<global::BioService.RawIndexes> RemovePhotos(global::BioService.RawIndexes request, ServerCallContext context);
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
      public global::BioService.Person AddPerson(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_AddPerson, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.Person AddPerson(global::BioService.Person request, CallOptions options)
      {
        var call = CreateCall(__Method_AddPerson, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Person> AddPersonAsync(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_AddPerson, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Person> AddPersonAsync(global::BioService.Person request, CallOptions options)
      {
        var call = CreateCall(__Method_AddPerson, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public global::BioService.Person UpdatePerson(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_UpdatePerson, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.Person UpdatePerson(global::BioService.Person request, CallOptions options)
      {
        var call = CreateCall(__Method_UpdatePerson, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Person> UpdatePersonAsync(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_UpdatePerson, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Person> UpdatePersonAsync(global::BioService.Person request, CallOptions options)
      {
        var call = CreateCall(__Method_UpdatePerson, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public global::BioService.Person RemovePerson(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_RemovePerson, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.Person RemovePerson(global::BioService.Person request, CallOptions options)
      {
        var call = CreateCall(__Method_RemovePerson, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Person> RemovePersonAsync(global::BioService.Person request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_RemovePerson, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Person> RemovePersonAsync(global::BioService.Person request, CallOptions options)
      {
        var call = CreateCall(__Method_RemovePerson, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public global::BioService.Card AddCard(global::BioService.Card request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_AddCard, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.Card AddCard(global::BioService.Card request, CallOptions options)
      {
        var call = CreateCall(__Method_AddCard, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Card> AddCardAsync(global::BioService.Card request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_AddCard, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Card> AddCardAsync(global::BioService.Card request, CallOptions options)
      {
        var call = CreateCall(__Method_AddCard, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public global::BioService.RawIndexes RemoveCards(global::BioService.RawIndexes request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_RemoveCards, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.RawIndexes RemoveCards(global::BioService.RawIndexes request, CallOptions options)
      {
        var call = CreateCall(__Method_RemoveCards, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.RawIndexes> RemoveCardsAsync(global::BioService.RawIndexes request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_RemoveCards, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.RawIndexes> RemoveCardsAsync(global::BioService.RawIndexes request, CallOptions options)
      {
        var call = CreateCall(__Method_RemoveCards, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public global::BioService.PhotoList SelectPhotos(global::BioService.QueryPhoto request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_SelectPhotos, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.PhotoList SelectPhotos(global::BioService.QueryPhoto request, CallOptions options)
      {
        var call = CreateCall(__Method_SelectPhotos, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.PhotoList> SelectPhotosAsync(global::BioService.QueryPhoto request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_SelectPhotos, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.PhotoList> SelectPhotosAsync(global::BioService.QueryPhoto request, CallOptions options)
      {
        var call = CreateCall(__Method_SelectPhotos, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public global::BioService.Photo AddPhoto(global::BioService.Photo request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_AddPhoto, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.Photo AddPhoto(global::BioService.Photo request, CallOptions options)
      {
        var call = CreateCall(__Method_AddPhoto, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Photo> AddPhotoAsync(global::BioService.Photo request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_AddPhoto, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Photo> AddPhotoAsync(global::BioService.Photo request, CallOptions options)
      {
        var call = CreateCall(__Method_AddPhoto, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public global::BioService.Photo SetThumbnail(global::BioService.Photo request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_SetThumbnail, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.Photo SetThumbnail(global::BioService.Photo request, CallOptions options)
      {
        var call = CreateCall(__Method_SetThumbnail, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Photo> SetThumbnailAsync(global::BioService.Photo request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_SetThumbnail, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.Photo> SetThumbnailAsync(global::BioService.Photo request, CallOptions options)
      {
        var call = CreateCall(__Method_SetThumbnail, options);
        return Calls.AsyncUnaryCall(call, request);
      }
      public global::BioService.RawIndexes RemovePhotos(global::BioService.RawIndexes request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_RemovePhotos, new CallOptions(headers, deadline, cancellationToken));
        return Calls.BlockingUnaryCall(call, request);
      }
      public global::BioService.RawIndexes RemovePhotos(global::BioService.RawIndexes request, CallOptions options)
      {
        var call = CreateCall(__Method_RemovePhotos, options);
        return Calls.BlockingUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.RawIndexes> RemovePhotosAsync(global::BioService.RawIndexes request, Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_RemovePhotos, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncUnaryCall(call, request);
      }
      public AsyncUnaryCall<global::BioService.RawIndexes> RemovePhotosAsync(global::BioService.RawIndexes request, CallOptions options)
      {
        var call = CreateCall(__Method_RemovePhotos, options);
        return Calls.AsyncUnaryCall(call, request);
      }
    }

    // creates service definition that can be registered with a server
    public static ServerServiceDefinition BindService(IBiometricDatabaseSevice serviceImpl)
    {
      return ServerServiceDefinition.CreateBuilder(__ServiceName)
          .AddMethod(__Method_PersonSelect, serviceImpl.PersonSelect)
          .AddMethod(__Method_AddPerson, serviceImpl.AddPerson)
          .AddMethod(__Method_UpdatePerson, serviceImpl.UpdatePerson)
          .AddMethod(__Method_RemovePerson, serviceImpl.RemovePerson)
          .AddMethod(__Method_AddCard, serviceImpl.AddCard)
          .AddMethod(__Method_RemoveCards, serviceImpl.RemoveCards)
          .AddMethod(__Method_SelectPhotos, serviceImpl.SelectPhotos)
          .AddMethod(__Method_AddPhoto, serviceImpl.AddPhoto)
          .AddMethod(__Method_SetThumbnail, serviceImpl.SetThumbnail)
          .AddMethod(__Method_RemovePhotos, serviceImpl.RemovePhotos).Build();
    }

    // creates a new client
    public static BiometricDatabaseSeviceClient NewClient(Channel channel)
    {
      return new BiometricDatabaseSeviceClient(channel);
    }

  }
}
#endregion
