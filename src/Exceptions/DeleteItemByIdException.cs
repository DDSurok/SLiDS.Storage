using System;

namespace SLiDS.Storage.Exceptions
{
    public class DeleteItemByIdException<TClass, TId> : Exception
    {
        public DeleteItemByIdException(TId id) : base($"Ошибка при удалении из БД {typeof(TClass).Name} с ID. [ ID: {id} ]")
        {
        }
        public DeleteItemByIdException(TId id, Exception innerEx) : base($"Ошибка при удалении из БД {typeof(TClass).Name} с ID. [ ID: {id} ]", innerEx)
        {
        }
    }
}
