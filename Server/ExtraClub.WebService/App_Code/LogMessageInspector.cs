using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;

public class LogMessageInspector : IDispatchMessageInspector
{
    public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, IClientChannel channel, InstanceContext instanceContext)
    {
        var req = request.Headers.FirstOrDefault(i => i.Name == "Action");

        return new _Helper { Stopwatch = Stopwatch.StartNew(), Method = req.GetType().GetProperty("Action").GetValue(req, null).ToString() };
    }

    public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
    {
        using (var w = File.AppendText("c:\\temp\\statlog.txt"))
        {
            var h = correlationState as _Helper;
            w.WriteLine("('" + h.Method + "'," + h.Stopwatch.ElapsedMilliseconds + "),");
        }
    }
}

public class _Helper {
    public Stopwatch Stopwatch { get; set; }
    public string Method { get; set; }
}

