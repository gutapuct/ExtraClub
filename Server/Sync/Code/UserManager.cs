using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TonusClub.Entities;
using System.Web.Mvc;

namespace Sync.Code
{
    public class UserManager
    {
        internal static Guid GetCurrentUserId(Controller controller)
        {
            using (var context = new TonusEntities())
            {
                var user = context.Users.Single(u => u.UserName == controller.User.Identity.Name.ToLower());

                return user.UserId;
            }
        }
    }
}