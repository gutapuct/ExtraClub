using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TonusClub.Entities;
using TonusClub.ServiceModel;

namespace TonusClub.ServerCore
{
    public static class FileCore
    {
        public static void PostFilePart(Guid divisionId, Guid fileId, string fileName, byte[] part, int bytes, int? category, Guid parameter)
        {
            using (var context = new TonusEntities())
            {
                var file = context.Files.SingleOrDefault(i => i.Id == fileId);
                if (file == null)
                {
                    var division = context.Divisions.Single(i => i.Id == divisionId);
                    file = new File
                    {
                        CompanyId = division.Id,
                        CreatedOn = DateTime.Now,
                        DivisionId = divisionId,
                        Filename = fileName,
                        Id = fileId,
                        Category = category
                    };
                    if (category == 2)
                    {
                        file.Parameter = parameter;
                        file.CompanyId = null;
                        file.DivisionId = null;
                    }
                    context.Files.AddObject(file);
                }
                if (file.Data == null)
                {
                    file.Data = part;
                }
                else
                {
                    file.Data = file.Data.Concat(part.Take(bytes)).ToArray();
                }
                context.SaveChanges();
            }
        }

        public static List<File> GetDivisionFiles(Guid divisionId)
        {
            using (var context = new TonusEntities())
            {
                var res = context.Files.Where(i => i.DivisionId == divisionId || !i.DivisionId.HasValue).OrderBy(i => i.Filename).ToList();
                res.ForEach(i => i.Data = null);
                return res;
            }
        }

        public static byte[] GetFilePart(Guid fileId, int blockNumber)
        {
            using (var context = new TonusEntities())
            {
                var file = context.Files.SingleOrDefault(i => i.Id == fileId);
                if (file == null) return new byte[0];
                var pos = blockNumber * 32768;
                if (pos > file.Data.Length) return new byte[0];

                var len = Math.Min(32768, file.Data.Length - pos);

                var res = new byte[len];
                Array.Copy(file.Data, pos, res, 0, len);
                return res;
            }
        }
    }
}
