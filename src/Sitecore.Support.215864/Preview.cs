using Sitecore.Diagnostics;
using Sitecore.Publishing;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.DeviceSimulation;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using System.Linq;

namespace Sitecore.Support.Shell.Applications.WebEdit.Commands
{
  public class Preview : Sitecore.Shell.Applications.WebEdit.Commands.Preview
  {
    public override void Execute(CommandContext context)
    {
      Assert.ArgumentNotNull(context, "context");
      if (context.Items.Length >= 1)
      {
        Context.ClientPage.Start(this, "Run");
      }
      //base.Execute(context);
    }
    protected override void Run(ClientPipelineArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      if (WebUtil.GetQueryString("mode") == "edit")
      {
        CheckModifiedParameters parameters = new CheckModifiedParameters
        {
          DisableNotifications = true
        };
        if (!SheerResponse.CheckModified(parameters))
        {
          return;
        }
      }
      WebUtil.SetCookieValue("sc_last_page_mode_command", base.Name);

      UrlString url = WebEditCommand.GetUrl();

      Assert.ArgumentNotNull(url, "url");
      PreviewManager.RestoreUser();
      DeviceSimulationUtil.DeactivateSimulators();
      url["sc_mode"] = "preview";
      url["sc_debug"] = "0";
      url["sc_trace"] = "0";
      url["sc_prof"] = "0";
      url["sc_ri"] = "0";
      url["sc_rb"] = "0";
      if (url.Parameters.AllKeys.Contains<string>("sc_version"))
      {
        url.Remove("sc_version");
      }
      WebEditCommand.Reload(url);

      base.Run(args);
    }
  }
}
