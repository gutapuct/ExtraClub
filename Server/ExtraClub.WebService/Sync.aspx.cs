using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ExtraClub.ServerCore;

public partial class Sunc : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void DoSync(object sender, EventArgs e)
    {
        SyncCore.Syncronize();
    }
}