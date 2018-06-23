using System;
namespace TFABot
{
    public interface ISpreadsheet<T> where T : class
    {
      void Update(T item); 
      void PostPopulate();
    }
}
