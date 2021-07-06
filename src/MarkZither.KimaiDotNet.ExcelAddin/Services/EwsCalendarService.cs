using Flurl;

using MarkZither.KimaiDotNet.ExcelAddin.Models.Calendar;

using Microsoft.Exchange.WebServices.Data;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MarkZither.KimaiDotNet.ExcelAddin.Services
{
    public class EwsCalendarService : CalendarServiceBase, ICalendarService
    {
        public EwsCalendarService(string url, string username, string password) : base(url, username, password)
        {
        }

        public IList<Appointment> GetAppointments()
        {
            ExchangeService ewsservice = GetEwsService();
            // Initialize values for the start and end times, and the number of appointments to retrieve.
            DateTime startDate = DateTime.Now;
            DateTime endDate = startDate.AddDays(30);
            const int NUM_APPTS = 25;
            // Initialize the calendar folder object with only the folder ID. 
            CalendarFolder calendar = CalendarFolder.Bind(ewsservice, WellKnownFolderName.Calendar, new PropertySet());
            // Set the start and end time and number of appointments to retrieve.
            CalendarView cView = new CalendarView(startDate, endDate, NUM_APPTS);
            // Limit the properties returned to the appointment's subject, start time, and end time.
            cView.PropertySet = new Microsoft.Exchange.WebServices.Data.PropertySet(AppointmentSchema.Subject, AppointmentSchema.Start, AppointmentSchema.End, AppointmentSchema.Categories);
            // Retrieve a collection of appointments by using the calendar view.
            FindItemsResults<Appointment> appointments = calendar.FindAppointments(cView);

            return appointments.Items.ToImmutableList();
        }
        public Categories GetCategories()
        {
            ExchangeService ewsservice = GetEwsService();
            FindRecurringCalendarItems(ewsservice, DateTime.Now.AddMonths(-6).AddDays(14), DateTime.Now.AddDays(7));
#pragma warning disable S4423 // Weak SSL/TLS protocols should not be used
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; //DevSkim: ignore DS440020,DS440000,DS144436
#pragma warning restore S4423 // Weak SSL/TLS protocols should not be used
            UserConfiguration usrConfig = UserConfiguration.Bind(ewsservice,
                                                                     "CategoryList",
                                                                     WellKnownFolderName.Calendar,
                                                                     UserConfigurationProperties.All);
            byte[] xmlData = usrConfig.XmlData;
            //found some nasty leading chars in XmlData which was breaking deserialization
            //it looked something like ï»À<?xml version... o worse an unprintable char
            if (usrConfig.XmlData[0].Equals(239) && usrConfig.XmlData[1].Equals(187) && usrConfig.XmlData[2].Equals(191))
            {
                xmlData = usrConfig.XmlData.Skip(3).ToArray();
            }
            string xml = Encoding.UTF8.GetString(xmlData);
            XmlSerializer serializer = new XmlSerializer(typeof(Categories));
            using (StringReader reader = new StringReader(xml))
            {
                var categories = (Categories)serializer.Deserialize(reader);
                return categories;
            }
        }

        public Collection<Appointment> FindRecurringCalendarItems(ExchangeService service,
                                                            DateTime startSearchDate,
                                                            DateTime endSearchDate)
        {
            // Instantiate a collection to hold occurrences and exception calendar items.
            Collection<Appointment> foundAppointments = new Collection<Appointment>();
            // Create a calendar view to search the calendar folder and specify the properties to return.
            CalendarView calView = new CalendarView(startSearchDate, endSearchDate, 50);
            calView.PropertySet = new PropertySet(BasePropertySet.IdOnly,
                                                    AppointmentSchema.Subject,
                                                    AppointmentSchema.Start,
                                                    AppointmentSchema.IsRecurring,
                                                    AppointmentSchema.AppointmentType);
            // Retrieve a collection of calendar items.
            FindItemsResults<Appointment> findResults = service.FindAppointments(WellKnownFolderName.Calendar, calView);
            // Add all calendar items in your view that are occurrences or exceptions to your collection.
            foreach (Appointment appt in findResults.Items)
            {
                if (appt.AppointmentType == AppointmentType.Occurrence || appt.AppointmentType == AppointmentType.Exception)
                {
                    foundAppointments.Add(appt);
                }
                else
                {
                    Console.WriteLine("Discarding calendar item of type {0}.", appt.AppointmentType);
                }
            }
            return foundAppointments;
        }
        private ExchangeService GetEwsService()
        {
#pragma warning disable S4423 // Weak SSL/TLS protocols should not be used
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; //DevSkim: ignore DS440020,DS440000,DS144436
#pragma warning restore S4423 // Weak SSL/TLS protocols should not be used
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
