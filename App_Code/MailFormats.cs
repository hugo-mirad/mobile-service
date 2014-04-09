using System;
using System.Data;
using System.Configuration;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Mail;
using System.Text;
using System.IO;

/// <summary>
/// Summary description for MailFormats
/// </summary>
public class MailFormats
{
    public string SendNewRegistrationRequestDetails(string Username, string Phone, string Email, ref string strMailFormat)
    {
        
        try
        {
            string OpenPath;
            StringBuilder sbQuery;
            string line;
            string SalesMailFile = System.Web.Hosting.HostingEnvironment.MapPath("~/MailTemplate/MobileRegisterCarRequest.txt");
            StreamReader objStreamReader;
            objStreamReader = File.OpenText(SalesMailFile);
            sbQuery = new StringBuilder();
            while ((line = objStreamReader.ReadLine()) != null)
            {
                string strMail = string.Empty;

                strMail = line + "<br />";

                if (line.Contains("###Cusname###"))
                {
                    strMail = line.Replace("###Cusname###", Username) + "<br />";
                }
                else if (line.Contains("###Phone###"))
                {
                    strMail = line.Replace("###Phone###", Phone) + "<br />";
                }
                else if (line.Contains("###Email###"))
                {
                    strMail = line.Replace("###Email###", Email) + "<br />";
                }
                strMailFormat = strMailFormat + strMail;
            }
        }
        catch (Exception ex)
        {
        }
        
        return strMailFormat;
    }
}
