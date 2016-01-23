// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: bioface_service.proto
#region Designer generated code

using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;

namespace BioFaceService {
  public static class BioFaceDetector
  {
    static readonly string __ServiceName = "BioFaceService.BioFaceDetector";

    static readonly Marshaller<global::BioFaceService.BioImage> __Marshaller_BioImage = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioFaceService.BioImage.Parser.ParseFrom);
    static readonly Marshaller<global::BioFaceService.DetectedObjectsInfo> __Marshaller_DetectedObjectsInfo = Marshallers.Create((arg) => global::Google.Protobuf.MessageExtensions.ToByteArray(arg), global::BioFaceService.DetectedObjectsInfo.Parser.ParseFrom);

    static readonly Method<global::BioFaceService.BioImage, global::BioFaceService.DetectedObjectsInfo> __Method_DetectFace = new Method<global::BioFaceService.BioImage, global::BioFaceService.DetectedObjectsInfo>(
        MethodType.DuplexStreaming,
        __ServiceName,
        "DetectFace",
        __Marshaller_BioImage,
        __Marshaller_DetectedObjectsInfo);

    // service descriptor
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::BioFaceService.BiofaceService.Descriptor.Services[0]; }
    }

    // client interface
    public interface IBioFaceDetectorClient
    {
      AsyncDuplexStreamingCall<global::BioFaceService.BioImage, global::BioFaceService.DetectedObjectsInfo> DetectFace(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken));
      AsyncDuplexStreamingCall<global::BioFaceService.BioImage, global::BioFaceService.DetectedObjectsInfo> DetectFace(CallOptions options);
    }

    // server-side interface
    public interface IBioFaceDetector
    {
      Task DetectFace(IAsyncStreamReader<global::BioFaceService.BioImage> requestStream, IServerStreamWriter<global::BioFaceService.DetectedObjectsInfo> responseStream, ServerCallContext context);
    }

    // client stub
    public class BioFaceDetectorClient : ClientBase, IBioFaceDetectorClient
    {
      public BioFaceDetectorClient(Channel channel) : base(channel)
      {
      }
      public AsyncDuplexStreamingCall<global::BioFaceService.BioImage, global::BioFaceService.DetectedObjectsInfo> DetectFace(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
      {
        var call = CreateCall(__Method_DetectFace, new CallOptions(headers, deadline, cancellationToken));
        return Calls.AsyncDuplexStreamingCall(call);
      }
      public AsyncDuplexStreamingCall<global::BioFaceService.BioImage, global::BioFaceService.DetectedObjectsInfo> DetectFace(CallOptions options)
      {
        var call = CreateCall(__Method_DetectFace, options);
        return Calls.AsyncDuplexStreamingCall(call);
      }
    }

    // creates service definition that can be registered with a server
    public static ServerServiceDefinition BindService(IBioFaceDetector serviceImpl)
    {
      return ServerServiceDefinition.CreateBuilder(__ServiceName)
          .AddMethod(__Method_DetectFace, serviceImpl.DetectFace).Build();
    }

    // creates a new client
    public static BioFaceDetectorClient NewClient(Channel channel)
    {
      return new BioFaceDetectorClient(channel);
    }

  }
}
#endregion
