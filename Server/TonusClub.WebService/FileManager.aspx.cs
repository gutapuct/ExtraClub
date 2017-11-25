using Flagmax.WorkflowService;
using System;
using System.Linq;
using TonusClub.Entities;

public partial class FileManager : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void DoSync(object sender, EventArgs e)
    {
        return;
        if (new TonusEntities().LocalSettings.Any()) return;
        new CloseDivisionUpdater(new TonusClub.ServerCore.CancelThreadInfo()).RunAsync();
    }
}