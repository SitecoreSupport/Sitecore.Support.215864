using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.ExperienceEditor.Utils;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Sitecore.Shell.DeviceSimulation;
using Sitecore.Text;
using Sitecore.Web;
using System.Linq;
using System.Web;

namespace Sitecore.Support.ExperienceEditor.Speak.Ribbon.Requests.SelectMode
{
  public class SelectModeRequest : PipelineProcessorRequest<ValueItemContext>
  {
    public override PipelineProcessorResponseValue ProcessRequest()
    {
      Assert.IsNotNullOrEmpty(base.RequestContext.Value, "Could not get string value for requestArgs:{0}", new object[] { base.Args.Data });
      string[] strArray = base.RequestContext.Value.Split(new char[] { '|' });
      if (strArray.Length != 2)
      {
        return new PipelineProcessorResponseValue { AbortMessage = "Missing item id or current url" };
      }
      string cookieValue = string.Empty;
      using (new SecurityDisabler())
      {
        Item item = Client.CoreDatabase.GetItem(new ID(HttpUtility.UrlDecode(strArray[0])));
        if (item != null)
        {
          cookieValue = item["Message"];
        }
      }
      UrlString url = new UrlString(strArray[1]);
      switch (cookieValue)
      {
        case "webedit:preview":
          WebUtil.SetCookieValue("sc_last_page_mode_command", cookieValue);
          ActivatePreview(url);
          return new PipelineProcessorResponseValue { Value = url.ToString() };

        case "webedit:debug":
          WebUtil.SetCookieValue("sc_last_page_mode_command", cookieValue);
          WebEditUtility.ActivateDebug(url);
          return new PipelineProcessorResponseValue { Value = url.ToString() };
      }
      return new PipelineProcessorResponseValue { Value = url.ToString() };
    }
    public static void ActivatePreview(UrlString url)
    {
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
    }



  }
}
