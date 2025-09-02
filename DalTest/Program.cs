using Dal;
using DalApi;
using DO;
using Accessories;
using System.Runtime.InteropServices;
namespace DalTest
{
    internal class Program
    {

        //private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); //stage 1
        //private static ICall? s_dalCall = new CallImplementation(); //stage 1
        //private static IAssignment? s_dalAssignment = new AssignmentImplementation(); //stage 1
        //private static IConfig? s_dalConfig = new ConfigImplementation(); 
        //static readonly IDal s_dal = new DalList(); //stage 2
        //static readonly IDal s_dal = new DalList(); //stage 2
        //static readonly IDal s_dal = new DalXml(); //stage 3
        static readonly IDal s_dal = Factory.Get; //stage 4

        /// <summary>
        /// פונקציה המזמנת את הפונקציה ליצירת האוביקט מסוג הפרמטר שהתקבל
        /// </summary>
        /// <param name="entityName">סוג ישות</param>
        private static void Create(string entityName)
        {
            switch (entityName)
            {
                case "Volunteer":
                    CreateVolunteer();
                    break;

                case "Call":
                    CreateCall();
                    break;
                case "Assignment":
                    CreateAssignment();
                    break;
            }
        }
        /// <summary>
        /// פונקציה ליצירת מתנדב
        /// </summary>
        private static void CreateVolunteer()
        {
            Volunteer volunteer = new()
            {
                Id = ReadHelper.ReadInt("insert id volunteer: "),
                Name = ReadHelper.ReadString("insert full name: "),
                Phone = ReadHelper.ReadString("insert phone number: "),
                Email = ReadHelper.ReadString("insert email: "),
                Role = ReadHelper.ReadEnum<Role>("insert role: "),
                Active = ReadHelper.ReadBool("insert is active: "),
                DistanceType = ReadHelper.ReadEnum<DistanceType>("insert distance type "),
                Latitude = ReadHelper.ReadDouble("insert latitude: "),
                Longitude = ReadHelper.ReadDouble("insert longitude: "),
                Password = ReadHelper.ReadString("insert password: "),
                Address = ReadHelper.ReadString("insert address: "),
                MaxDistanceForCall = ReadHelper.ReadDouble("insert max distance for call: ")
            };
            s_dal.Volunteer.Create(volunteer);
        }
        /// <summary>
        /// פונקציה ליצירת קריאה חדשה
        /// </summary>
        private static void CreateCall()
        {
            Call call = new ()
            {
                CallType = ReadHelper.ReadEnum<CallType>("insert call type: "),
                CallAddress = ReadHelper.ReadString("insert call address: "),
                Latitude = ReadHelper.ReadDouble("insert a latitude:"),
                Longitude = ReadHelper.ReadDouble("insert a longitude:"),
                OpeningTime = ReadHelper.ReadDate("insert opening time for call: "),
                CallDescription = ReadHelper.ReadString("insert call description: "),
                MaxTimeFinishCall = ReadHelper.ReadDate("insert max time finish call:")
            };
            s_dal.Call.Create(call);
        }
        /// <summary>
        /// פונקציה ליצירת הקצאה
        /// </summary>
        private static void CreateAssignment()
        {
            Assignment assignment = new()
            {
                CallId = ReadHelper.ReadInt("insert id of call: "),
                VolunteerId = ReadHelper.ReadInt("insert id of volunteer: "),
                EntryTimeForTreatment = ReadHelper.ReadDate("insert entry time for treatment: "),
                TypeOfTreatmentTermination = ReadHelper.ReadEnum<TypeOfTreatmentTermination>("insert type of treatment termination"),
                EndOfTreatmentTime = ReadHelper.ReadDate("insert end of treatment time")
            };
            s_dal.Assignment.Create(assignment);
        }
        /// <summary>
        /// פונקציה הקולטת אי.די. למחיקה ומוחקת מהרשימה המתאימה
        /// </summary>
        /// <param name="entityName">סוג היישות למחיקה</param>
        private static void Delete(string entityName)
        {
            int idToDelete;
            Console.WriteLine("insert id-entity to delete:");
            idToDelete = int.TryParse(Console.ReadLine(), out int result) ? result : throw new DalMustValueExeption("insert value");
            try
            {
                switch (entityName)
                {
                    case "Volunteer":
                        s_dal.Volunteer.Delete(idToDelete);
                        break;

                    case "Call":
                        s_dal.Call.Delete(idToDelete);
                        break;
                    case "Assignment":
                        s_dal.Assignment.Delete(idToDelete);
                        break;
                    default:
                        Console.WriteLine("Unsupported type: " + entityName);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        /// <summary>
        /// פונקציה לעדכון קריאה לפי אי.די שקולט המשתמש
        /// </summary>
        private static void UpdateCall()
        {
            Console.WriteLine("insert id-entity to update:");
            int idToUpdate = int.TryParse(Console.ReadLine(), out int result) ? result : throw new DalMustValueExeption("insert value");
            try
            {
                Call? oldCall = s_dal.Call.Read(idToUpdate);
                Console.WriteLine("Enter the data to create a new object of type call:");
                Console.WriteLine("Enter the data of: type of call, full address, latitude, longitude, opening time, maximum time of finish call, description");
                Call newCall = new Call()
                {
                    CallType = int.TryParse(Console.ReadLine(), out int typeOfCall) ? (CallType)typeOfCall : oldCall!.CallType,
                    CallAddress = ReadHelper.ReadOrDefault(Console.ReadLine(), oldCall!.CallAddress),
                    Latitude = double.TryParse(Console.ReadLine(), out double latitude) ? latitude : oldCall.Latitude,
                    Longitude = double.TryParse(Console.ReadLine(), out double Longitude) ? Longitude : oldCall.Longitude,
                    OpeningTime = DateTime.TryParse(Console.ReadLine(), out DateTime OpeningTime) ? OpeningTime : oldCall.OpeningTime,
                    MaxTimeFinishCall = DateTime.TryParse(Console.ReadLine(), out DateTime MaximumTimeFinishCall) ? MaximumTimeFinishCall : oldCall.MaxTimeFinishCall,
                    CallDescription = ReadHelper.ReadOrDefault(Console.ReadLine(), oldCall.CallDescription!),

                };
                s_dal.Call.Update(newCall);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        /// <summary>
        /// פונקציה לעדכון מתנדב לפי אי.די שקולט המשתמש
        /// </summary>
        /// // פונקציה לבדיקת המחרוזת והחזרת ערך ברירת המחדל במקרה שהיא ריקה
        private static void UpdateVolunteer()
        {
            Console.WriteLine("insert id-entity to update:");
           int idToUpdate = int.TryParse(Console.ReadLine(), out int result) ? result : throw new DalMustValueExeption("insert value");
            try
            {
                Volunteer? oldVolunteer = s_dal.Volunteer.Read(idToUpdate);
                Console.WriteLine("Enter the data of:  full name, phone, email, role, active, distance type,latitude,longitude,password, address, max distance for call");
                Volunteer newVolunteer = new Volunteer()
                {
                    Id = oldVolunteer!.Id,
                    Name = ReadHelper.ReadOrDefault(Console.ReadLine(), oldVolunteer.Name),
                    Phone = ReadHelper.ReadOrDefault(Console.ReadLine(), oldVolunteer.Phone),
                    Email = ReadHelper.ReadOrDefault(Console.ReadLine(), oldVolunteer.Email),
                    Role = int.TryParse(Console.ReadLine(), out int role) ? (Role)role : oldVolunteer.Role,
                    Active = bool.TryParse(Console.ReadLine(), out bool active) ? active : oldVolunteer.Active,
                    DistanceType = int.TryParse(Console.ReadLine(), out int distanceType) ? (DistanceType)distanceType : oldVolunteer.DistanceType,
                    Latitude = double.TryParse(Console.ReadLine(), out double latitude) ? latitude : oldVolunteer.Latitude,
                    Longitude = double.TryParse(Console.ReadLine(), out double longitude) ? longitude : oldVolunteer.Longitude,
                    Password = ReadHelper.ReadOrDefault(Console.ReadLine(), oldVolunteer.Password!),
                    Address = ReadHelper.ReadOrDefault(Console.ReadLine(), oldVolunteer.Address!),
                    MaxDistanceForCall = double.TryParse(Console.ReadLine(), out double maxDistanceForCall) ? maxDistanceForCall : oldVolunteer.MaxDistanceForCall,
                };
                s_dal.Volunteer.Update(newVolunteer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        /// <summary>
        /// פונקציה לעדכון הקצאה לפי אי.די שקולט המשתמש
        /// </summary>
        private static void UpdateAssignment()
        {
            Console.WriteLine("insert id-entity to update:");
            int idToUpdate = int.TryParse(Console.ReadLine(), out int result) ? result : throw new DalMustValueExeption("insert value");
            try
            {
                Assignment? oldAssignment = s_dal.Assignment.Read(idToUpdate);
                Console.WriteLine("Enter the data to create a new object of type assignment:");
                Console.WriteLine("insert  call id, volunteer id, entry time for treatment, type of treatment termination,end of treatment time");
                Assignment newAssignment = new Assignment()
                {
                    CallId = int.TryParse(Console.ReadLine(), out int callId) ? callId : oldAssignment!.CallId,
                    VolunteerId = int.TryParse(Console.ReadLine(), out int volunteerId) ? volunteerId : oldAssignment!.VolunteerId,
                    EntryTimeForTreatment = DateTime.TryParse(Console.ReadLine(), out DateTime entryTimeForTreatment) ? entryTimeForTreatment : oldAssignment!.EntryTimeForTreatment,
                    TypeOfTreatmentTermination = int.TryParse(Console.ReadLine(), out int typeOfTreatmentTermination) ? (TypeOfTreatmentTermination)typeOfTreatmentTermination : oldAssignment!.TypeOfTreatmentTermination,
                    EndOfTreatmentTime = DateTime.TryParse(Console.ReadLine(), out DateTime endOfTreatmentTime) ? endOfTreatmentTime : oldAssignment!.EndOfTreatmentTime,
                };
                s_dal.Assignment.Update(newAssignment);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        /// <summary>
        /// פונקציה לעדכון אוביקט בהתאם לסוג היישות שמתקבלת
        /// </summary>
        /// <param name="entityName">סוג היישות של האוביקט לעדכון</param>
        private static void Update(string entityName)
        {
            try
            {
                switch (entityName)
                {
                    case "Volunteer":
                        UpdateVolunteer();
                        break;
                    case "Call":
                        UpdateCall();
                        break;
                    case "Assignment":
                        UpdateAssignment();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
        /// <summary>
        /// פונקצית הדפסת פרטי מתנדב מסוים לפי האי.די שנקלט מהמשתמש
        /// </summary>
        /// <param name="idToRead">אי.די של המתנדב להדפסה</param>
        private static void ReadVolunteer(int idToRead)
        {
            if (s_dal.Volunteer.Read(idToRead) != null)
            {
                Volunteer volunteer = s_dal.Volunteer.Read(idToRead) ?? throw new DalDoesNotExistException("the volunteer does not exist");
                Console.WriteLine("Volunteer Details:");
                Console.WriteLine($"Volunteer ID: {volunteer.Id}");
                Console.WriteLine($"Volunteer name: {volunteer.Name}");
                Console.WriteLine($"Volunteer phone: {volunteer.Phone}");
                Console.WriteLine($"Volunteer email: {volunteer.Email}");
                Console.WriteLine($"Volunteer role: {volunteer.Role}");
                Console.WriteLine($"Active: {volunteer.Active}");
                Console.WriteLine($"Distance type: {volunteer.DistanceType}");
                Console.WriteLine($"Latitude: {volunteer.Latitude?.ToString() ?? "there isn't Latitude"}");
                Console.WriteLine($"Longitude: {volunteer.Longitude?.ToString() ?? "there isn't Longitude"}");
                Console.WriteLine($"Password: {volunteer.Password?.ToString() ?? "there isn't Password"}");
                Console.WriteLine($"Address: {volunteer.Address?.ToString() ?? "there isn't Address"}");
                Console.WriteLine($"Max distance for call: {volunteer.MaxDistanceForCall?.ToString() ?? "there isn't max distance for call"}");
            }
        }
        /// <summary>
        /// פונקצית הדפסת פרטי קריאה מסוימת לפי האי.די שנקלט מהמשתמש
        /// </summary>
        /// <param name="idToRead">אי.די של הקיראה להדפסה</param>
        private static void ReadCall(int idToRead)
        {
            if (s_dal.Call.Read(idToRead) != null)
            {
                Call call = s_dal.Call.Read(idToRead) ?? throw new DalDoesNotExistException("the call does not exist");
                // הצגת המידע של המשימה
                Console.WriteLine("Call Details:");
                Console.WriteLine($"Call ID: {call.Id}");
                Console.WriteLine($"Call type: {call.CallType}");
                Console.WriteLine($"Call address: {call.CallAddress}");
                Console.WriteLine($"Latitude: {call.Latitude}");
                Console.WriteLine($"Longitude: {call.Longitude}");
                Console.WriteLine($"Opening time: {call.OpeningTime.ToString()}");
                Console.WriteLine($"Call description: {call.CallDescription?.ToString() ?? "there isn't call description"}");
                Console.WriteLine($"Max time finish call: {call.MaxTimeFinishCall?.ToString() ?? "there isn't max time finish to call"}");
            }
        }
        /// <summary>
        /// פונקצית הדפסת פרטי הקצאה מסוימת לפי האי.די שנקלט מהמשתמש
        /// </summary>
        /// <param name="idToRead">אי.די של ההקצאה להדפסה</param>
        private static void ReadAssignment(int idToRead)
        {
            if (s_dal.Assignment.Read(idToRead) != null)
            {
                Assignment assignment = s_dal.Assignment.Read(idToRead) ?? throw new DalDoesNotExistException("the assignment does not exist");
                // הצגת המידע של המשימה
                Console.WriteLine("Assignment Details:");
                Console.WriteLine($"Assignment ID: {assignment.Id}");
                Console.WriteLine($"Call ID: {assignment.CallId}");
                Console.WriteLine($"Volunteer ID: {assignment.VolunteerId}");
                Console.WriteLine($"Entry Time of Treatment: {assignment.EntryTimeForTreatment}");
                Console.WriteLine($"End Time of Treatment: {assignment.EndOfTreatmentTime?.ToString() ?? "N/A"}");
                Console.WriteLine($"Type of End Treatment: {assignment.TypeOfTreatmentTermination?.ToString() ?? "N/A"}");
            }
        }
        /// <summary>
        /// פונקציה להדפסת פרטי אוביקט
        /// </summary>
        /// <param name="entityName">סוג יישות האוביקט</param>
        /// <param name="idToRead">אי.די של האוביקט להדפסה</param>
        private static void Read(string entityName, int idToRead)
        {
            if (idToRead == 0)
            {
                Console.WriteLine("insert id-entity to read:");
                idToRead = int.TryParse(Console.ReadLine(), out int result) ? result : throw new DalMustValueExeption("insert value");
            }
            switch (entityName)
            {
                case "Volunteer":
                    ReadVolunteer(idToRead);
                    break;
                case "Call":
                    ReadCall(idToRead);
                    break;

                case "Assignment":
                    ReadAssignment(idToRead);
                    break;

                default:
                    Console.WriteLine("Unsupported type: " + entityName);
                    break;
            }

        }
        /// <summary>
        /// פונקציה להדפסת כל האוביקטים מסוג מסויים
        /// </summary>
        /// <param name="entityName">סוג היישות להדפסה</param>
        private static void ReadAll(string entityName)
        {
            switch (entityName)
            {
                case "Volunteer":
                    foreach (var volunteer in s_dal.Volunteer.ReadAll())
                        Read("Volunteer", volunteer.Id);
                    break;
                case "Call":
                    foreach (var call in s_dal.Call.ReadAll())
                        Read("Call", call.Id);
                    break;
                case "Assignment":
                    foreach (var assignment in s_dal.Assignment.ReadAll())
                        Read("Assignment", assignment.Id);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// פונקציה למחיקת כל האוביקטים מסוג מסוים
        /// </summary>
        /// <param name="entityName">סוג היישות למחיקה</param>
        private static void DeleteAll(string entityName)
        {
            switch (entityName)
            {
                case "Volunteer":
                    s_dal.Volunteer.DeleteAll();
                    break;
                case "Call":
                    s_dal.Call.DeleteAll();
                    break;
                case "Assignment":
                    s_dal.Assignment.DeleteAll();
                    break;
                default:
                    Console.WriteLine("Unsupported type: " + entityName);
                    break;
            }
        }
        /// <summary>
        /// פונקציה להדפסת התפריט הראשי
        /// </summary>
        static void PrintMainMenu()
        {
            Console.WriteLine("\nMain Menu:");
            foreach (var option in Enum.GetValues<MainMenuOptions>())
            {
                Console.WriteLine($"{(int)option}: {option}");
            }
            Console.Write("Choose an option: ");
        }
        static void MenuConfig()
        {
            Console.WriteLine("\nConfig Menu:");
            foreach (var option in Enum.GetValues<ConfigMenuOptions>())
            {
                Console.WriteLine($"{(int)option}: {option}");
            }
            Console.Write("Choose an option: ");
            int numericChoice = int.TryParse(Console.ReadLine(), out int result) ? result : throw new DalMustValueExeption("insert value");
            ConfigMenuOptions choice = (ConfigMenuOptions)numericChoice;
            switch (choice)
            {

                case ConfigMenuOptions.Exit:
                    return;
                case ConfigMenuOptions.AdvanceClockMinute:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                    break;
                case ConfigMenuOptions.AdvanceClockHour:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddHours(1);
                    break;
                case ConfigMenuOptions.ViewCurrentClock:
                    Console.WriteLine(s_dal.Config.Clock);
                    break;
                case ConfigMenuOptions.NewConfigValue:
                    s_dal.Config.Clock = s_dal.Config.Clock.AddMinutes(3);
                    break;
                case ConfigMenuOptions.ViewConfigValue:
                    Console.WriteLine($"the risk range is: {s_dal.Config.RiskRange.ToString()}");
                    break;
                case ConfigMenuOptions.ResetConfigValues:
                    s_dal.Config.Reset();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// פונקציה להדפסת כל האוביקטים
        /// </summary>
        static void ShowAll()
        {
            ReadAll("Volunteer");
            ReadAll("Call");
            ReadAll("Assignment");
        }
        /// <summary>
        /// פונקציה להדפסת התת תפריט ליישות
        /// </summary>
        static void PrintCrudMenu()
        {
            Console.WriteLine("\nMain Menu:");
            foreach (var option in Enum.GetValues<CrudMenuOptions>())
            {
                Console.WriteLine($"{(int)option}: {option}");
            }
            Console.Write("Choose an option: ");
        }
        static void Main(string[] args)
        {
            while (true)
            {
                //פונקציה שמדפיסה את התפריט הראשי
                PrintMainMenu();
                string? input = Console.ReadLine();
                int numericChoice;

                // בדיקת קלט
                if (!int.TryParse(input, out numericChoice) || !Enum.IsDefined(typeof(MainMenuOptions), numericChoice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }
                // המרה לערך Enum של MainMenuOptions
                MainMenuOptions choice = (MainMenuOptions)numericChoice;

                switch (choice)
                {
                    case MainMenuOptions.Exit:
                        return;
                    case MainMenuOptions.VolunteerMenu:
                        CrudMenu("Volunteer");
                        break;
                    case MainMenuOptions.AssignmentMenu:
                        CrudMenu("Assignment");
                        break;
                    case MainMenuOptions.CallMenu:
                        CrudMenu("Call");
                        break;
                    case MainMenuOptions.ConfigMenu:
                        MenuConfig();
                        break;
                    case MainMenuOptions.ShowAll:
                        ShowAll();
                        break;
                    case MainMenuOptions.InitializeDatabase:
                        try
                        {
                          //Initialization.Do(s_dal); //stage 2
                            Initialization.Do(); //stage 4
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        break;
                    case MainMenuOptions.ResetDatabase:
                        s_dal.Config.Reset();
                        break;
                }
            }
        }
        /// <summary>
        /// פונקציה לזימון הפעולה הרצויה מהתפריט משנה
        /// </summary>
        /// <param name="entityName">סוג היישות</param>
        static void CrudMenu(string entityName)
        {
            PrintCrudMenu();
            string? input = Console.ReadLine();
            int numericCrudChoice;

            // בדיקת קלט
            if (!int.TryParse(input, out numericCrudChoice) || !Enum.IsDefined(typeof(CrudMenuOptions), numericCrudChoice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                return;
            }
            // המרה לערך Enum של MainMenuOptions
            CrudMenuOptions choice = (CrudMenuOptions)numericCrudChoice;

            switch (choice)
            {

                case CrudMenuOptions.Exit:
                    return;

                case CrudMenuOptions.Create:
                    Create(entityName);
                    break;
                case CrudMenuOptions.Read:
                    Read(entityName, 0);
                    break;
                case CrudMenuOptions.ReadAll:
                    ReadAll(entityName);
                    break;
                case CrudMenuOptions.Update:
                    Update(entityName);
                    break;
                case CrudMenuOptions.Delete:
                    Delete(entityName);
                    break;
                case CrudMenuOptions.DeleteAll:
                    DeleteAll(entityName);
                    break;
                default:
                    break;
            }
        }
    }
}
