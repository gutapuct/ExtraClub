using System;
using System.IO;
using TonusClub.UIControls;

namespace TonusClub.ClientDal
{
    public static class FileUploader
    {
        static FileUploader()
        {
            var di = new DirectoryInfo("temp");
            if (!di.Exists) di.Create();
        }

        public static Guid UploadFile(ClientContext context, string fileName, int? category, Guid parameter)
        {
            FileInfo file = new FileInfo(fileName);
            var fs = file.OpenRead();
            var buff = new byte[32768];
            var res = Guid.NewGuid();
            int i;
            while (0 != (i = fs.Read(buff, 0,buff.Length)))
            {
                context.PostFilePart(res, file.Name, buff, i, category, parameter);
            }
            return res;
        }

        public static string DownloadFile(ClientContext context, ServiceModel.File file)
        {
            FileInfo fileInfo = new FileInfo("temp\\" + file.Filename);
            if (fileInfo.Exists)
            {
                fileInfo = new FileInfo("temp\\" + Guid.NewGuid() + file.Filename);
            }
            var fs = fileInfo.OpenWrite();
            int i =0;
            while (true)
            {
                var bts = context.GetFilePart(file.Id, i++);
                if (bts.Length > 0)
                {
                    fs.Write(bts, 0, bts.Length);
                }
                else
                {
                    break;
                }
            }
            fs.Close();

            return fileInfo.FullName;
        }
    }
}
