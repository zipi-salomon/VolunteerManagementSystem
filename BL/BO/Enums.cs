namespace BO;
public enum Role { Manager, Volunteer }
public enum DistanceType { Aerial, Walking, TraveledByCar }
public enum CallType { HouseCleaning, CookingFood, BabysitterServices, Transportation, Ironing, Shopping,None }
public enum StatusCallInProgress { InTreatment, InRiskTreatment }
public enum TypeOfTreatmentTermination { Handled, SelfCancellation, CancelAdministrator, CancellationExpired }
public enum StatusCall{Open ,InTreatment ,Closed ,Expired ,OpenAtRisk ,InTreatmentAtRisk ,None}
public enum VolunteerInListAttributes { Id, Name, Active, TotalCallsHandledByVolunteer, TotalCallsCanceledByVolunteer, TotalExpiredCallingsByVolunteer, IDCallInHisCare, CallType }
public enum CallInListAttributes { Id, CallId, CallType, OpeningTime, TotalTimeRemainingFinishCalling, LastVolunteerName, TotalTimeCompleteTreatment, StatusCall, TotalAssignments }
public enum ClosedCallInListAttributes { Id, CallType, CallAddress, OpeningTime, EntryTimeForTreatment, EndOfTreatmentTime, TypeOfTreatmentTermination }
public enum OpenCallInListAttributes { Id, CallType, CallDescription, CallAddress, OpeningTime, MaxTimeFinishCall, CallingDistanceFromTreatingVolunteer }
public enum TimeUnit{Minute, Hour, Day,Month,Year };
public enum MainMenuOptions { Exit, VolunteerMenu, AssignmentMenu, CallMenu, AdminMenu }
public enum CrudMenuOptions { Exit, Create, Read, Update, Delete,GetList }
public enum AdminMenuOptions { Exit, AdvanceClock, ViewCurrentClock,ViewConfigValue,NewConfigValue ,InitializeDatabase, ResetDatabase  }
public enum CallMenu { ClosedCallsListHandledByVolunteer, GetCallQuantitiesByStatus, GetCallsList, OpenCallsListSelectedByVolunteer, ChooseTreatmentCall,UpdateEndTreatment }
