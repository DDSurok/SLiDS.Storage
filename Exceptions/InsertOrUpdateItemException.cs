using System;

namespace SLiDS.Storage.Exceptions
{
    public class InsertOrUpdateItemException<T> : Exception
    {
        public InsertOrUpdateItemException(T item) : base($"Ошибка при добавлении/изменении в БД следующего {typeof(T).Name}: {item}.")
        {
        }
        public InsertOrUpdateItemException(T item, Exception innerEx) : base($"Ошибка при добавлении/изменении в БД следующего {typeof(T).Name}: {item}.", innerEx)
        {
        }
    }
}
