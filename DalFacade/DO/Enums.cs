namespace DO;
public enum Role{Manager,Volunteer}
public enum DistanceType { Aerial, walking, TraveledByCar }
public enum CallType { HouseCleaning, CookingFood, BabysitterServices, Transportation, Ironing, Shopping }
public enum TypeOfTreatmentTermination {Handled, SelfCancellation, CancelAdministrator, CancellationExpired }
public enum MainMenuOptions { Exit, VolunteerMenu, AssignmentMenu, CallMenu,ConfigMenu,ShowAll,InitializeDatabase, ResetDatabase }
public enum CrudMenuOptions { Exit, Create, Read, ReadAll, Update, Delete, DeleteAll }
public enum ConfigMenuOptions { Exit,AdvanceClockMinute, AdvanceClockHour, ViewCurrentClock,NewConfigValue,ViewConfigValue,ResetConfigValues }
