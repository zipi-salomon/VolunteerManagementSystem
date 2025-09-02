namespace DalTest;
using DalApi;
using DO;
using System;

public static class Initialization
{
    // שדות פרטיים לאחסון ה-DALים (אובייקטים המתקשרים למאגר הנתונים)

    private static IDal? s_dal;

    private static readonly Random s_rand = new(); // אובייקט שמייצר מספרים אקראיים
    const int MIN_ID = 200000000; // מזהה מינימלי למתנדב/קריאה/הקצאה
    const int MAX_ID = 400000000; // מזהה מקסימלי למתנדב/קריאה/הקצאה
    static List<AddAddress> addresses = AddAddress.GetAddAddresses(); // רשימת כתובות
                                                                      // מתודה ליצירת מתנדבים חדשים עם נתונים אקראיים
    private static void CreateVolunteer()
    {
        // שמות המתנדבים וטלפונים אקראיים
        string[] valunteerNames =
        { "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Shira Israelof",
          "David Gold", "Rachel Green", "Miriam Azulay", "Eyal Shani", "Noa Bar", "Yael Harel",
            "zipi salomon","avigail levy","yaeli shushan"};
        string[] phones = { "0504133382", "0556726282", "0527175821", "0527175820", "0504160838", "0504156891",
        "0503133382", "0556724282", "0527175221", "0528175820", "0504160837", "0504756891",
        "0504113382", "0556723282", "0527575821"};
        int[] ids = { 328462635, 327773271, 327883591, 327887048, 327934857, 328115522,
            328118245, 328119425, 328128061, 328159801, 328177191, 328178371,328183934,328184304,328216437};
        // יצירת מתנדבים ושמירתם ב-DAL
        for (int i = 0; i < valunteerNames.Length; i++)
        {
            AddAddress randAddress = addresses[s_rand.Next(addresses.Count)];
            Volunteer newVolunteer = new ()
            {
                Id = ids[i], // מזהה אקראי
                Name = valunteerNames[i], // שם המתנדב
                Phone = phones[i], // טלפון המתנדב
                Email = valunteerNames[i] + "@gmail.com", // אימייל אקראי
                Role = i % 2 == 0 ? Role.Volunteer : Role.Manager, // תפקיד המתנדב (מתנדב או מנהל)
                Active = i % 2 == 0 ? true : false, // אם המתנדב פעיל או לא
                DistanceType = (DistanceType)s_rand.Next(0, 3), // סוג המרחק (אווירי, הליכה, רכב)
                Latitude = randAddress.Latitude, // קו רוחב של הכתובת
                Longitude = randAddress.Longitude, // קו אורך של הכתובת
                Password = valunteerNames[i] + phones[i], // סיסמה
                Address = randAddress.StringAddress, // כתובת מלאה
                MaxDistanceForCall = s_rand.NextDouble() * 50 // המרחק המרבי לקבלת קריאה
            };
            s_dal!.Volunteer.Create(newVolunteer); // שמירת המתנדב ב-DAL

        }
    }
    // מתודה ליצירת קריאות חדשות עם נתונים אקראיים

    private static void CreateCall()
    {
        for (int i = 0; i < 50; i++)
        {
            int rand_day = s_rand.Next(3, 14);
            AddAddress randAddress = addresses[s_rand.Next(addresses.Count)];
            Call newCall = new ()
            {
                CallType = (DO.CallType)s_rand.Next(0, 6),
                CallAddress = randAddress.StringAddress!,
                Latitude = randAddress.Latitude,
                Longitude = randAddress.Longitude,
                OpeningTime = s_dal!.Config.Clock.AddDays(-rand_day),
                CallDescription = randAddress.StringAddress + " " + randAddress.Longitude.ToString() + " " + randAddress.Latitude.ToString() + " call",
                MaxTimeFinishCall = i < 5 ? s_dal.Config.Clock.AddDays(-rand_day + 1) : s_dal.Config.Clock.AddDays(rand_day),
            };
            s_dal.Call!.Create(newCall);
        }
    }
    // מתודה ליצירת הקצאות חדשות עם נתונים אקראיים
    private static void CreateAssignment()
    {
        // שאיבת נתוני מתנדבים וקריאות מתוך ה-DAL
        List<Volunteer> volunteers = s_dal!.Volunteer.ReadAll().ToList();
        List<Call> calls = s_dal.Call.ReadAll().ToList();
        // בדיקה אם רשימות המתנדבים או הקריאות ריקות
        if (!volunteers.Any() || !calls.Any())
            throw new InvalidOperationException("Volunteers or Calls data is empty!");
        // יצירת הקצאות עבור קריאות
        foreach (var call in calls)
        {
            // אחוז קטן מהקריאות (20%) לא יקבלו הקצאה
            if (s_rand.NextDouble() < 0.2)
                continue;

            DateTime now = s_dal!.Config.Clock;

            if (call.MaxTimeFinishCall < now)
            {
                // הקריאה פגה – נסמן כהקצאה שפג תוקפה
                Assignment expiredAssignment = new()
                {
                    CallId = call.Id,
                    VolunteerId = 0,
                    EntryTimeForTreatment = call.OpeningTime,
                    EndOfTreatmentTime = call.MaxTimeFinishCall,
                    TypeOfTreatmentTermination = TypeOfTreatmentTermination.CancellationExpired
                };
                s_dal.Assignment.Create(expiredAssignment);
                continue;
            }

            // בחירת מתנדב אקראי מתוך הרשימה
            var volunteer = volunteers.Skip(4).ElementAt(s_rand.Next(volunteers.Count - 4));
            // חישוב זמן תחילת טיפול (בין 30 דקות ל-24 שעות לאחר פתיחת הקריאה)
            DateTime entryTimeOfTreatment = call.OpeningTime.AddMinutes(s_rand.Next(30, 1440));
            // משתנה לסיום טיפול (אופציונל)י
            DateTime? endTimeOfTreatment = null;
            // קביעה אם הקריאה טופלה
            bool isCompleted = s_rand.NextDouble() < 0.7; // 70% סיכוי שהקריאה טופלה
            if (isCompleted)
            {
                // זמן סיום טיפול (בין 30 דקות ל-48 שעות לאחר תחילת הטיפול)
                endTimeOfTreatment = entryTimeOfTreatment.AddMinutes(s_rand.Next(30, 2880));
                // אם זמן סיום הטיפול חורג מזמן הסיום המקסימלי, מוסיפים חריגה קטנה
                //endTimeOfTreatment = endTimeOfTreatment ?? call.MaxTimeFinishCall;
                //endTimeOfTreatment = endTimeOfTreatment!.Value.AddMinutes(s_rand.Next(30, 1440));
            }

            // קביעה של סוג סיום הטיפול
            TypeOfTreatmentTermination? typeOfEndTreatment;
            if(endTimeOfTreatment==null)
                typeOfEndTreatment = null;
            else 
            if (!isCompleted)
            {
                // אם הקריאה לא טופלה, סוג הסיום יהיה "ביטול מנהל"
                typeOfEndTreatment = TypeOfTreatmentTermination.CancelAdministrator;
            }
            else
            {
                // אם הקריאה טופלה, סוג הסיום ייבחר באופן אקראי
                //typeOfEndTreatment = (TypeOfTreatmentTermination)s_rand.Next(0, Enum.GetValues(typeof(TypeOfTreatmentTermination)).Length);
                typeOfEndTreatment= TypeOfTreatmentTermination.Handled;
            }

            // יצירת אובייקט הקצאה חדש
            Assignment newAssignment = new ()
            {
                CallId = call.Id,                        // מזהה הקריאה
                VolunteerId = volunteer.Id,              // מזהה המתנדב
                EntryTimeForTreatment = entryTimeOfTreatment, // זמן תחילת הטיפול
                TypeOfTreatmentTermination = typeOfEndTreatment,    // סוג סיום הטיפול
                EndOfTreatmentTime = endTimeOfTreatment     // זמן סיום הטיפול (אם קיים)
            };

            // שמירת ההקצאה ב-DAL
            s_dal.Assignment.Create(newAssignment);
        }

        // סימון חלק מהמתנדבים ככאלו שלא טיפלו כלל בקריאות (20%)
        var idleVolunteers = volunteers.Take(volunteers.Count / 5).ToList(); // 20% מהמתנדבים
    }
    //מתודה לאתחול הנתונים
    //public static void Do(IDal dal) //stage 2
    public static void Do() { //stage 4
      //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); //stage 2
        s_dal = DalApi.Factory.Get; //stage 4
        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();//stage 2
        CreateCall();
        CreateVolunteer();
        CreateAssignment();
    }


}

