﻿using DO;
namespace DalApi;

public interface ICrud<T> where T:class
{
    void Create(T item); //Creates new entity object in DAL
    //T? Read(int id); //Reads entity object by its ID 
    T? Read(Func<T, bool> filter); // stage 2
    T? Read(int id); 
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // stage 2,Reads all entity objects
    void Update(T item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects
}
