using Flurl;

using MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar;

using Microsoft.Exchange.WebServices.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MarkZither.KimaiDotNet.ExcelAddin.Services
{
    public class EwsCalendarService : EwsCalendarServiceBase, ICalendarService
    {
        public EwsCalendarService(string url, string username, string password) : base(url, username, password)
        {
        }

        public Categories GetCategories()
        {
            ExchangeService ewsservice = GetEwsService();

#pragma warning disable S4423 // Weak SSL/TLS protocols should not be used
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; //DevSkim: ignore DS440020,DS440000,DS144436
#pragma warning restore S4423 // Weak SSL/TLS protocols should not be used
            UserConfiguration usrConfig = UserConfiguration.Bind(ewsservice,
                                                                     "CategoryList",
                                                                     WellKnownFolderName.Calendar,
                                                                     UserConfigurationProperties.All);

            string xml = Encoding.UTF8.GetString(usrConfig.XmlData);
            XmlSerializer serializer = new XmlSerializer(typeof(Categories));
            using (StringReader reader = new StringReader(xml))
            {
                var categories = (Categories)serializer.Deserialize(reader);
                return categories;
            }
        }

        private ExchangeService GetEwsService()
        {
            string ewsUrl = RootUrl.AppendPathSegment("EWS/Exchange.asmx");

            var ewsservice = new Microsoft.Exchange.WebServices.Data.ExchangeService
            {
                Credentials = new System.Net.NetworkCredential(Username, Password),
                Url = new Uri(ewsUrl)
            };
            return ewsservice;
        }
    }
}
