using Accessories;
namespace BlTest
{
    internal class Program
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
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
        private static void CreateVolunteer()
        {
            BO.Volunteer volunteer = new()
            {
                Id = ReadHelper.ReadInt("insert id volunteer: "),
                Name = ReadHelper.ReadString("insert full name: "),
                Phone = ReadHelper.ReadString("insert phone number: "),
                Email = ReadHelper.ReadString("insert email: "),
                Role = ReadHelper.ReadEnum<BO.Role>("insert role: "),
                Active = ReadHelper.ReadBool("insert is active: "),
                DistanceType = ReadHelper.ReadEnum<BO.DistanceType>("insert distance type "),
                Address = ReadHelper.ReadString("insert address: "),
                MaxDistanceForCall = ReadHelper.ReadDouble("insert max distance for call: ")
            };
            try
            {
                s_bl.Volunteer.AddVolunteer(volunteer);
            }
            catch (BO.BlAlreadyExistsException ex)
            {
                Console.WriteLine($"BlAlreadyExistsException ,{ex},מידע על חריגה פנימית!!!!");
            }
            catch (BO.BlInvalidValueException ex)
            {
                Console.WriteLine($"BlInvalidValueException ,{ex},מידע על חריגה פנימית!!!!");
            }

        }
        private static void CreateCall()
        {
            BO.Call call = new()
            {
                CallType = ReadHelper.ReadEnum<BO.CallType>("insert call type: "),
                CallAddress = ReadHelper.ReadString("insert call address: "),
                CallDescription = ReadHelper.ReadString("insert call description: "),
                MaxTimeFinishCall = ReadHelper.ReadDate("insert max time finish call:")
            };
            try
            {
                s_bl.Call.AddCall(call);
            }
            catch (BO.BlAlreadyExistsException ex)
            {
                Console.WriteLine($"BlAlreadyExistsException {ex}");
            }
        }
        private static void CreateAssignment()
        {
            int idVolunteer = ReadHelper.ReadInt("insert id Volunteer");
            int idCall = ReadHelper.ReadInt("insert id Call");

            try
            {
                s_bl.Call.ChooseTreatmentCall(idVolunteer, idCall);
            }
            catch (BO.BlInvalidRequestException ex)
            {
                Console.WriteLine($"BlInvalidRequestException,{ex}");
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"BlDoesNotExistException,{ex}");
            }
        }
        /// <summary>
        /// פונקציה הקולטת אי.די. למחיקה ומוחקת מהרשימה המתאימה
        /// </summary>
        /// <param name="entityName">סוג היישות למחיקה</param>
        private static void Delete(string entityName)
        {
            Console.WriteLine("insert id-entity to delete:");
            int idToDelete = ReadHelper.ReadInt("insert id to delete");
            switch (entityName)
            {
                case "Volunteer":
                    {
                        try
                        {
                            s_bl.Volunteer.DeleteVolunteer(idToDelete);
                        }
                        catch (BO.BlCantDeleteException ex)
                        {
                            Console.WriteLine($"BlCantDeleteException{ex.Message}{ex}");
                        }
                        break;
                    }
                case "Call":
                    {
                        try { s_bl.Call.DeleteCall(idToDelete); }
                        catch (BO.BlCantDeleteException ex)
                        {
                            Console.WriteLine($"BlCantDeleteException{ex}");
                        }
                        break;
                    }
                case "Assignment":
                    {
                        try
                        {
                            int idRequest = ReadHelper.ReadInt("insert your id:");
                            s_bl.Call.UpdateCancelTreatmentOnCall(idRequest, idToDelete);
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"BlDoesNotExistException{ex}");
                        }
                        catch (BO.BlCantUpdateException ex)
                        {
                            Console.WriteLine($"BlCantUpdateException{ex}");
                        }
                        catch (BO.BlUnauthorizedException ex)
                        {
                            Console.WriteLine($"BlUnauthorizedException{ex}");
                        }
                        break;
                    }
                default:
                    Console.WriteLine("Unsupported type: " + entityName);
                    break;
            }
        }
        /// <summary>
        /// פונקציה לעדכון קריאה לפי אי.די שקולט המשתמש
        /// </summary>
        private static void UpdateCall()
        {
            int idToUpdate = ReadHelper.ReadInt("insert id of call to update");
            try
            {
                BO.Call? oldCall = s_bl.Call.GetCallDetails(idToUpdate);
                Console.WriteLine("Enter the data to create a new object of type call:");
                Console.WriteLine("Enter the data of: type of call, full address, maximum time of finish call, description");
                BO.Call newCall = new()
                {
                    Id=oldCall.Id,
                    CallType = Enum.TryParse(Console.ReadLine(), out BO.CallType parsedCallType)? parsedCallType: oldCall.CallType,
                    CallAddress = ReadHelper.ReadOrDefault(Console.ReadLine(), oldCall!.CallAddress),
                    OpeningTime = oldCall.OpeningTime,
                    MaxTimeFinishCall = DateTime.TryParse(Console.ReadLine(), out DateTime MaximumTimeFinishCall) ? MaximumTimeFinishCall : oldCall.MaxTimeFinishCall,
                    CallDescription = ReadHelper.ReadOrDefault(Console.ReadLine(), oldCall.CallDescription!),

                };
                s_bl.Call.UpdateCallDetails(newCall);
            }
            catch (BO.BlCantUpdateException ex)
            {
                Console.WriteLine($" BlCantUpdateException:{ex}, {ex.Message}");
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($" BlDoesNotExistException:{ex}, {ex.Message}");
            }

        }

        /// <summary>
        /// פונקציה לעדכון מתנדב לפי אי.די שקולט המשתמש
        /// </summary>
        /// // פונקציה לבדיקת המחרוזת והחזרת ערך ברירת המחדל במקרה שהיא ריקה
        private static void UpdateVolunteer()
        {
            try
            {
                int idToUpdate = ReadHelper.ReadInt("insert id volunteer to update:");
                BO.Volunteer? oldVolunteer = s_bl.Volunteer.GetVolunteerDetails(idToUpdate);
                Console.WriteLine("Enter the data of:  full name, phone, email, role, active, distance type,password, address, max distance for call");
                BO.Volunteer newVolunteer = new ()
                {
                    Id = oldVolunteer!.Id,
                    Name = ReadHelper.ReadOrDefault(Console.ReadLine(), oldVolunteer.Name),
                    Phone = ReadHelper.ReadOrDefault(Console.ReadLine(), oldVolunteer.Phone),
                    Email = ReadHelper.ReadOrDefault(Console.ReadLine(), oldVolunteer.Email),
                    Role = int.TryParse(Console.ReadLine(), out int role) ? (BO.Role)role : oldVolunteer.Role,
                    Active = bool.TryParse(Console.ReadLine(), out bool active) ? active : oldVolunteer.Active,
                    DistanceType = int.TryParse(Console.ReadLine(), out int distanceType) ? (BO.DistanceType)distanceType : oldVolunteer.DistanceType,
                    Address = ReadHelper.ReadOrDefault(Console.ReadLine(), oldVolunteer.Address!),
                    MaxDistanceForCall = double.TryParse(Console.ReadLine(), out double maxDistanceForCall) ? maxDistanceForCall : oldVolunteer.MaxDistanceForCall,
                };

                int idRequester = ReadHelper.ReadInt("insert your id:");
                s_bl.Volunteer.UpdateVolunteerDetails(idRequester, newVolunteer);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"BlDoesNotExistException:{ex} {ex.Message}");
            }
            catch (BO.BlCantUpdateException ex)
            {
                Console.WriteLine($"BlCantUpdateException:{ex} {ex.Message}");
            }
            catch (BO.BlUnauthorizedException ex)
            {
                Console.WriteLine($"BlUnauthorizedException:{ex} {ex.Message}");
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
            try
            {
                BO.Volunteer volunteer = s_bl.Volunteer.GetVolunteerDetails(idToRead);
                Console.WriteLine("Volunteer Details:");
                Console.WriteLine(volunteer);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"BlDoesNotExistException{ex}");
            }
        }
        /// <summary>
        /// פונקצית הדפסת פרטי קריאה מסוימת לפי האי.די שנקלט מהמשתמש
        /// </summary>
        /// <param name="idToRead">אי.די של הקיראה להדפסה</param>
        private static void ReadCall(int idToRead)
        {
            try
            {
                BO.Call call = s_bl.Call.GetCallDetails(idToRead);
                Console.WriteLine("call Details:");
                Console.WriteLine(call);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"BlDoesNotExistException{ex}");
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
                idToRead = ReadHelper.ReadInt("insert id entity to read");
            }
            switch (entityName)
            {
                case "Volunteer":
                    ReadVolunteer(idToRead);
                    break;
                case "Call":
                    ReadCall(idToRead);
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
            foreach (var option in Enum.GetValues<BO.MainMenuOptions>())
            {
                Console.WriteLine($"{(int)option}: {option}");
            }
            Console.Write("Choose an option: ");
        }
        static void AdminMenu()
        {
            Console.WriteLine("\nConfig Menu:");
            foreach (var option in Enum.GetValues<BO.AdminMenuOptions>())
            {
                Console.WriteLine($"{(int)option}: {option}");
            }
            Console.Write("Choose an option: ");
            int numericChoice = int.TryParse(Console.ReadLine(), out int result) ? result : throw new BO.BlInvalidValueException("insert value");
            BO.AdminMenuOptions choice = (BO.AdminMenuOptions)numericChoice;
            switch (choice)
            {

                case BO.AdminMenuOptions.Exit:
                    return;
                case BO.AdminMenuOptions.AdvanceClock:
                    s_bl.Admin.AdvanceClock(BO.TimeUnit.Minute);
                    break;
                case BO.AdminMenuOptions.ViewCurrentClock:
                    Console.WriteLine(s_bl.Admin.GetClock());
                    break;
                case BO.AdminMenuOptions.NewConfigValue:
                    s_bl.Admin.SetRiskRange(TimeSpan.MinValue);
                    break;
                case BO.AdminMenuOptions.ViewConfigValue:
                    Console.WriteLine($"the risk range is: {s_bl.Admin.GetRiskRange()}");
                    break;
                case BO.AdminMenuOptions.InitializeDatabase:
                    s_bl.Admin.InitializeDB();
                    break;
                case BO.AdminMenuOptions.ResetDatabase:
                    s_bl.Admin.ResetDB();
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// פונקציה להדפסת התת תפריט ליישות
        /// </summary>
        static void PrintCrudMenu()
        {
            Console.WriteLine("\nMain Menu:");
            foreach (var option in Enum.GetValues<BO.CrudMenuOptions>())
            {
                Console.WriteLine($"{(int)option}: {option}");
            }
            Console.Write("Choose an option: ");
        }
        //##############################################################################################################################
        //##############################################################################################################################
        //##############################################################################################################################
        //##############################################################################################################################
        static void Main(string[] args)
        {
            while (true)
            {
                //פונקציה שמדפיסה את התפריט הראשי
                PrintMainMenu();
                string? input = Console.ReadLine();
                int numericChoice;

                // בדיקת קלט
                if (!int.TryParse(input, out numericChoice) || !Enum.IsDefined(typeof(BO.MainMenuOptions), numericChoice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }
                // המרה לערך Enum של MainMenuOptions
                BO.MainMenuOptions choice = (BO.MainMenuOptions)numericChoice;

                switch (choice)
                {
                    case BO.MainMenuOptions.Exit:
                        return;
                    case BO.MainMenuOptions.VolunteerMenu:
                        CrudMenu("Volunteer");
                        break;
                    case BO.MainMenuOptions.AssignmentMenu:
                        CrudMenu("Assignment");
                        break;
                    case BO.MainMenuOptions.CallMenu:
                        CrudMenu("Call");
                        break;
                    case BO.MainMenuOptions.AdminMenu:
                        AdminMenu();
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
            if (!int.TryParse(input, out numericCrudChoice) || !Enum.IsDefined(typeof(BO.CrudMenuOptions), numericCrudChoice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                return;
            }
            // המרה לערך Enum של MainMenuOptions
            BO.CrudMenuOptions choice = (BO.CrudMenuOptions)numericCrudChoice;

            switch (choice)
            {
                case BO.CrudMenuOptions.Exit:
                    return;
                case BO.CrudMenuOptions.Create:
                    Create(entityName);
                    break;
                case BO.CrudMenuOptions.Read:
                    Read(entityName, 0);
                    break;
                case BO.CrudMenuOptions.Update:
                    Update(entityName);
                    break;
                case BO.CrudMenuOptions.Delete:
                    Delete(entityName);
                    break;
                case BO.CrudMenuOptions.GetList:
                    GetList(entityName);
                    break;
                default:
                    break;
            }
        }

        private static void GetList(string entityName)
        {
            switch (entityName)
            {
                case "Volunteer":
                    ListVolunteers();
                    break;
                case "Call":
                    CallOperations();
                    break;
            }
        }

        private static void EndTreatmentOnCall()
        {
            Console.WriteLine("insert id volunteer");
            int idVolunteer = int.TryParse(Console.ReadLine(), out int result) ? result : throw new BO.BlInvalidValueException("insert value");
            Console.WriteLine("insert id assignment");
            int idAssignment = int.TryParse(Console.ReadLine(), out int result2) ? result2 : throw new BO.BlInvalidValueException("insert value");
            try
            {
                s_bl.Call.UpdateEndTreatmentOnCall(idVolunteer, idAssignment);
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"BlDoesNotExist {ex}");
            }
            catch (BO.BlCantUpdateException ex)
            {
                Console.WriteLine($"BlCantUpdate {ex}");
            }
            catch (BO.BlUnauthorizedException ex)
            {
                Console.WriteLine($"BlUnauthorized {ex}");
            }
            catch (BO.BlInvalidValueException ex)
            {
                Console.WriteLine($"BlInvalidValue {ex}");
            }
        }
        private static void CallOperations()
        {
            string choose = ReadHelper.ReadString("insert your choise:");
            BO.CallMenu attributeEnum = (BO.CallMenu)Enum.Parse(typeof(BO.CallMenu), choose);
            switch (attributeEnum)
            {
                case BO.CallMenu.GetCallsList:
                    CallsList();
                    break;
                case BO.CallMenu.GetCallQuantitiesByStatus:
                    CallQuantitiesByStatus();
                    break;
                case BO.CallMenu.ClosedCallsListHandledByVolunteer:
                    ClosedCallsListHandled();
                    break;
                case BO.CallMenu.OpenCallsListSelectedByVolunteer:
                    OpenCallsListSelected();
                    break;
                case BO.CallMenu.UpdateEndTreatment:
                    EndTreatmentOnCall();
                    break;
            }
        }

        private static void OpenCallsListSelected()
        {
            int idVolunteer = ReadHelper.ReadInt("insert a idVolunteer:");
            BO.CallType filterByAttribute = ReadHelper.ReadEnum<BO.CallType>("insert filter By Attribute: ");
            BO.OpenCallInListAttributes sortByAttribute = ReadHelper.ReadEnum<BO.OpenCallInListAttributes>("insert sort By Attribute: ");
            try
            {
                var listcalls = s_bl.Call.OpenCallsListSelectedByVolunteer(idVolunteer, filterByAttribute, sortByAttribute);
                foreach (var c in listcalls)
                {
                    Console.WriteLine(c);
                }
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"BlDoesNotExistException{ex}");
            }
        }

        private static void ClosedCallsListHandled()
        {
            int idVolunteer = ReadHelper.ReadInt("insert a idVolunteer:");
            BO.CallType filterByAttribute = ReadHelper.ReadEnum<BO.CallType>("insert filter By Attribute: ");
            BO.ClosedCallInListAttributes sortByAttribute = ReadHelper.ReadEnum<BO.ClosedCallInListAttributes>("insert sort By Attribute: ");
            try
            {
                var listcalls = s_bl.Call.ClosedCallsListHandledByVolunteer(idVolunteer, filterByAttribute, sortByAttribute);
                foreach (var c in listcalls)
                {
                    Console.WriteLine(c);
                }
            }
            catch (BO.BlDoesNotExistException ex)
            { Console.WriteLine($"BlDoesNotExistException{ex.Message}"); }
        }
        private static void CallQuantitiesByStatus()
        {
            int[] callCounts = s_bl.Call.GetCallQuantitiesByStatus();
            var statuses = Enum.GetValues(typeof(BO.StatusCall)).Cast<BO.StatusCall>();

            foreach (var status in statuses)
            {
                Console.WriteLine($"{status}: {callCounts[(int)status]}");
            }
        }

        private static void CallsList()
        {
            int idCall = ReadHelper.ReadInt("insert a idCall:");
            BO.CallInListAttributes filterByAttribute = ReadHelper.ReadEnum<BO.CallInListAttributes>("insert filter By Attribute: ");
            object filterValue = Console.ReadLine();
            BO.CallInListAttributes? sortByAttribute = ReadHelper.ReadEnum<BO.CallInListAttributes>("insert sort By Attribute: ");
            var listcalls = s_bl.Call.GetCallsList(filterByAttribute, filterValue, sortByAttribute);
            foreach (var c in listcalls)
            {
                Console.WriteLine(c);
            }

        }

        private static void ListVolunteers()
        {
            bool? active = ReadHelper.ReadBoolNull("insert if active");
            BO.VolunteerInListAttributes? sortByAttribute = ReadHelper.ReadEnumNull<BO.VolunteerInListAttributes>("insert sort By Attribute: ");
            var listVols = s_bl.Volunteer.GetVolunteersList(BO.VolunteerInListAttributes.Active,active, sortByAttribute);
            foreach (var v in listVols)
            {
                Console.WriteLine(v);
            }
        }
    }
}