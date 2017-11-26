using ExtraClub.WorkflowService;
using System;
using System.Linq;
using ExtraClub.Entities;

public partial class FileManager : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void DoSync(object sender, EventArgs e)
    {
        return;
        if (new ExtraEntities().LocalSettings.Any()) return;
        new CloseDivisionUpdater(new ExtraClub.ServerCore.CancelThreadInfo()).RunAsync();
    }
}