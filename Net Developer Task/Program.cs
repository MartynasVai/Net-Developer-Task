using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;



namespace Net_Developer_Task
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Write your name:");
            var UserName = Console.ReadLine();
            Console.WriteLine($"Hello {UserName}");

            while (true)
            {

                var Command = Console.ReadLine();

                if (Command == "exit" || Command == "Exit")//exit arba Exit isjungia programa
                    break;


                switch (Command)//comandos
                {
                    case "New meeting":
                        NewMeeting(UserName);
                        break;
                    case "Delete meeting":
                        DeleteMeeting(UserName);
                        break;
                    case "Add person":
                        AddPerson();
                        break;
                    case "Remove person":
                        RemovePerson();
                        break;
                    case "List meetings":
                        Console.WriteLine("type in mode of filtering Attendees/Description/Responsible person/Caregory/Type/Dates");
                        var Mode = Console.ReadLine();
                        ListMeetings(Mode);
                        break;

                }

            }
        }

        private class Meeting
        {
            public string Name { get; set; }
            public string ResponsiblePerson { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public string Type { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        private class Attendee
        {
            public string Name { get; set; }
            public string MeetingName { get; set; }
            public DateTime JoinDate { get; set; }
            public DateTime ExitDate { get; set; }
        }

        public static void NewMeeting(string UserName)
        {
            List<Meeting> Meetings = new List<Meeting>();
            Meetings = JsonFileReader.Read<Meeting>("MeetingData.json");//is json failo nuskaito i lista

            Console.WriteLine("Write your meeting name:");
            var NewName = Console.ReadLine();
            var item = Meetings.SingleOrDefault(x => x.Name == NewName);
            if (item != null)
            {
                Console.WriteLine("Meeting name taken");
                return;
            }
                Console.WriteLine("Write description");
            var NewDescription = Console.ReadLine();
            Console.WriteLine("Select Category CodeMonkey/Hub/Short/TeamBuilding");
            var NewCategory = Console.ReadLine();
            if (NewCategory != "CodeMonkey"&& NewCategory != "Hub"&& NewCategory != "Short"&& NewCategory != "TeamBuilding")//jeigu netaip parasytas category atsaukia meeting kurima
            {
                Console.WriteLine("Category should be one of the following : CodeMonkey/Hub/Short/TeamBuilding");
                return;
            }
            Console.WriteLine("Select Type Live/InPerson");//jeigu netaip parasytas type atsaukia meeting kurima
            var NewType = Console.ReadLine();
            if (NewType != "Live" && NewType != "InPerson")
            {
                Console.WriteLine("Type should be one of the following : Live/InPerson");
                return;
            }
            Console.WriteLine("Write start date");
            var StartDate = Console.ReadLine();
            Console.WriteLine("Write end date");
            var EndDate = Console.ReadLine();

            try//datos keitimas i datetime
            {
                var Date1 = DateTime.Parse(StartDate);
                var Date2 = DateTime.Parse(EndDate);


                var Meeting = new Meeting
                {
                    Name = NewName,
                    ResponsiblePerson = UserName,
                    Category = NewCategory,
                    Description = NewDescription,
                    Type = NewType,
                    StartDate = Date1,
                    EndDate = Date2,

            };
                
                Meetings.Add(Meeting);//prie list prideda naujai sukurta meet

                File.WriteAllText("MeetingData.json", JsonSerializer.Serialize(Meetings));//iraso i faila


                List<Attendee> Attendees = new List<Attendee>();
                Attendees = JsonFileReader.Read<Attendee>("Attendees.json");//is json failo nuskaito i lista
                var NewAttendee = new Attendee
                {
                    Name = UserName,
                    MeetingName = NewName,
                    JoinDate = Date1,
                    ExitDate = Date2,
                };

                Attendees.Add(NewAttendee);
                File.WriteAllText("Attendees.json", JsonSerializer.Serialize(Attendees));
            }
            catch (FormatException)
            {
                Console.WriteLine("Date entered incorrectly ", StartDate, EndDate);
            }
            //Meeting meeting = new Meeting();
        }
        public static class JsonFileReader
        {
            public static List<T> Read<T>(string filePath)
            {
                string text = File.ReadAllText(filePath);
                var Deserialized = JsonSerializer.Deserialize<List<T>>(text);
                return Deserialized;
            }

        }
        public static void DeleteMeeting(string UserName)
        {
            Console.WriteLine("Write the name of the meeting that you want to delete");
            var MeetingName = Console.ReadLine();

            List<Meeting> Meetings = new List<Meeting>();
            Meetings = JsonFileReader.Read<Meeting>("MeetingData.json");//is json failo nuskaito i lista
            var item = Meetings.SingleOrDefault(x => x.Name == MeetingName && x.ResponsiblePerson == UserName);
            if (item != null) 
            { 
            Meetings.Remove(item);//panaikina meeting

            File.WriteAllText("MeetingData.json", JsonSerializer.Serialize(Meetings));
            List<Attendee> Attendees = new List<Attendee>();
            Attendees = JsonFileReader.Read<Attendee>("Attendees.json");//is json failo nuskaito i lista
            Attendees.RemoveAll(x=>x.MeetingName == MeetingName);//panaikina meeting lankytojus

            File.WriteAllText("Attendees.json", JsonSerializer.Serialize(Attendees));
            }
        }
        public static void AddPerson()
        {
            Console.WriteLine("Type in the name the person:");
            var Name = Console.ReadLine();
            Console.WriteLine("Type in the meeting name:");
            var MeetingName = Console.ReadLine();
            Console.WriteLine("Type in the Date:");
            var MeetingJoinDate = Console.ReadLine();

            List<Meeting> Meetings = new List<Meeting>();
            Meetings = JsonFileReader.Read<Meeting>("MeetingData.json");//is json failo nuskaito i lista

            List<Attendee> Attendees = new List<Attendee>();
            Attendees = JsonFileReader.Read<Attendee>("Attendees.json");//is json failo nuskaito i lista

            var Meeting = Meetings.SingleOrDefault(x => x.Name == MeetingName);
            if (Meeting != null)
            {
                var Duplicate=Attendees.SingleOrDefault(x => x.MeetingName == MeetingName && x.Name == Name);//paziuri ar nera jau pridetas zmogus
                if(Duplicate != null)
                {
                    Console.WriteLine("Person is already in this meeting");
                    return;
                }
                var Intersection = Attendees.SingleOrDefault(x => x.Name == Name && x.JoinDate > Meeting.StartDate && x.JoinDate < Meeting.EndDate);
                if (Intersection != null) 
                { 
                Console.WriteLine("Meeting intersects with another one, confirm y/n?");
                    var yn = Console.ReadLine();
                    if (yn == "n") { return; }//jeigu atsaukia
                }
                var NewAttendee = new Attendee
                {
                    Name = Name,
                    MeetingName = MeetingName,
                    JoinDate = DateTime.Parse(MeetingJoinDate),
                    ExitDate = Meeting.EndDate,
                };

                Attendees.Add(NewAttendee);
                File.WriteAllText("Attendees.json", JsonSerializer.Serialize(Attendees));

            }
        }
        public static void RemovePerson()
        {
            Console.WriteLine("Type in the name the person:");
            var Name = Console.ReadLine();
            Console.WriteLine("Type in the meeting name:");
            var MeetingName = Console.ReadLine();

            List<Meeting> Meetings = new List<Meeting>();
            Meetings = JsonFileReader.Read<Meeting>("MeetingData.json");//is json failo nuskaito i lista

            List<Attendee> Attendees = new List<Attendee>();
            Attendees = JsonFileReader.Read<Attendee>("Attendees.json");//is json failo nuskaito i lista

            var Meeting = Meetings.SingleOrDefault(x => x.Name == MeetingName);
            if (Meeting != null) { 
            if  (Meeting.ResponsiblePerson != Name) { 
                var AttendeeToRemove = Attendees.SingleOrDefault(x=>x.MeetingName == MeetingName&& x.Name == Name);
                    Attendees.Remove(AttendeeToRemove);
                }
            }
        }
        public static void ListMeetings(string Mode) {
            List<Meeting> Meetings = new List<Meeting>();
            Meetings = JsonFileReader.Read<Meeting>("MeetingData.json");//is json failo nuskaito i lista
            List<Meeting> FilteredMeetings = new List<Meeting>();
            switch (Mode) {//pagal mode nusprendzia pagal ka filtruoti 
                case "Description":
                    Console.WriteLine("Type in the search:");
                    var Search= Console.ReadLine();
                    FilteredMeetings=Meetings.Where(x=>x.Description.Contains(Search)).ToList();
                    foreach (var Meeting in FilteredMeetings)
                    {
                        Console.WriteLine(Meeting.Name);
                        Console.WriteLine($"Responsible person: {Meeting.ResponsiblePerson}");
                        Console.WriteLine($"Description: {Meeting.Description}");
                        Console.WriteLine($"Category: {Meeting.Category}");
                        Console.WriteLine($"Type: {Meeting.Type}");
                        Console.WriteLine($"Start date: {Meeting.StartDate}");
                        Console.WriteLine($"Start date: {Meeting.EndDate}");

                    }
                        break;
                case "Responsible person":
                    Console.WriteLine("Type in the search:");
                    Search = Console.ReadLine();
                    FilteredMeetings = Meetings.Where(x => x.ResponsiblePerson.Contains(Search)).ToList();
                    foreach (var Meeting in FilteredMeetings)
                    {
                        Console.WriteLine(Meeting.Name);
                        Console.WriteLine($"Responsible person: {Meeting.ResponsiblePerson}");
                        Console.WriteLine($"Description: {Meeting.Description}");
                        Console.WriteLine($"Category: {Meeting.Category}");
                        Console.WriteLine($"Type: {Meeting.Type}");
                        Console.WriteLine($"Start date: {Meeting.StartDate}");
                        Console.WriteLine($"Start date: {Meeting.EndDate}");

                    }
                    break;
                case "Caregory":
                    Console.WriteLine("Type in the search:");
                    Search = Console.ReadLine();
                    FilteredMeetings = Meetings.Where(x => x.Category.Contains(Search)).ToList();
                    foreach (var Meeting in FilteredMeetings)
                    {
                        Console.WriteLine(Meeting.Name);
                        Console.WriteLine($"Responsible person: {Meeting.ResponsiblePerson}");
                        Console.WriteLine($"Description: {Meeting.Description}");
                        Console.WriteLine($"Category: {Meeting.Category}");
                        Console.WriteLine($"Type: {Meeting.Type}");
                        Console.WriteLine($"Start date: {Meeting.StartDate}");
                        Console.WriteLine($"Start date: {Meeting.EndDate}");

                    }
                    break;
                case "Type":
                    Console.WriteLine("Type in the search:");
                    Search = Console.ReadLine();
                    FilteredMeetings = Meetings.Where(x => x.Type.Contains(Search)).ToList();
                    foreach (var Meeting in FilteredMeetings)
                    {
                        Console.WriteLine(Meeting.Name);
                        Console.WriteLine($"Responsible person: {Meeting.ResponsiblePerson}");
                        Console.WriteLine($"Description: {Meeting.Description}");
                        Console.WriteLine($"Category: {Meeting.Category}");
                        Console.WriteLine($"Type: {Meeting.Type}");
                        Console.WriteLine($"Start date: {Meeting.StartDate}");
                        Console.WriteLine($"Start date: {Meeting.EndDate}");

                    }
                    break;
                case "Dates":
                    Console.WriteLine("from:");
                    var StartDate = Console.ReadLine();
                    Console.WriteLine("to:");
                    var EndDate = Console.ReadLine();


                    


                    try 
                    {


                        var Date1 = DateTime.Parse(StartDate);
                        var Date2 = DateTime.Parse(EndDate);
                        FilteredMeetings = Meetings.Where(x => x.StartDate >= Date1 && x.EndDate<Date2).ToList();
                        foreach(var Meeting in FilteredMeetings)
                        {
                            Console.WriteLine(Meeting.Name);
                            Console.WriteLine($"Responsible person: {Meeting.ResponsiblePerson}");
                            Console.WriteLine($"Description: {Meeting.Description}");
                            Console.WriteLine($"Category: {Meeting.Category}");
                            Console.WriteLine($"Type: {Meeting.Type}");
                            Console.WriteLine($"Start date: {Meeting.StartDate}");
                            Console.WriteLine($"Start date: {Meeting.EndDate}");

                        }
                        break;
                    }
                    catch(FormatException)
                    {
                        Console.WriteLine("Date entered incorrectly ", StartDate, EndDate);
                        break;
                    }
                case "Attendees":
                    Console.WriteLine("Type in the minimum ammount:");
                    
                    var MinAmmount = Console.ReadLine();
                    if (int.TryParse(MinAmmount,out int j)){
                        var IntMinAmmount=int.Parse(MinAmmount);
                    

                    List<Attendee> Attendees = new List<Attendee>();
                    foreach (var Meeting in Meetings)
                    {
                            Attendees = JsonFileReader.Read<Attendee>("Attendees.json");

                            Attendees.RemoveAll(x => x.MeetingName != Meeting.Name);
                        int Ammount = Attendees.Count;//suskaiciuoja kiek sveciu
                            if (IntMinAmmount <= Ammount) { 
                            Console.WriteLine(Meeting.Name);
                            Console.WriteLine($"Attendee ammount {Ammount}");
                            Console.WriteLine($"Responsible person: {Meeting.ResponsiblePerson}");
                            Console.WriteLine($"Description: {Meeting.Description}");
                            Console.WriteLine($"Category: {Meeting.Category}");
                            Console.WriteLine($"Type: {Meeting.Type}");
                            Console.WriteLine($"Start date: {Meeting.StartDate}");
                            Console.WriteLine($"Start date: {Meeting.EndDate}");
                            }
                    }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}