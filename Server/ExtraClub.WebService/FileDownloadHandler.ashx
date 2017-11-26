<%@ WebHandler Language="C#" Class="FileDownloadHandler" %>

using System;
using System.Web;
using System.Linq;

public class FileDownloadHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        context.Response.Buffer = false;
        context.Response.ContentType = "text/plain";
        ExtraClub.ServiceModel.SshFile sf;
        var fId = Guid.Parse(context.Request.Params[0]);
        using (var context1 = new ExtraClub.Entities.ExtraEntities())
        {
            sf = context1.SshFiles.Single(i => i.Id == fId);
        }
        
        
        string path = @"c:\temp\"+context.Request.Params[0];
        var file = new System.IO.FileInfo(path);
        int len = (int)file.Length, bytes;
        context.Response.AppendHeader("content-length", len.ToString());
        context.Response.AppendHeader("Content-Disposition", "attachment; filename=\"" + sf.Filename + "\"");
        byte[] buffer = new byte[1024];
        var outStream = context.Response.OutputStream;
        using (var stream = System.IO.File.OpenRead(path))
        {
            while (len > 0 && (bytes =
                stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                outStream.Write(buffer, 0, bytes);
                len -= bytes;
            }
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}