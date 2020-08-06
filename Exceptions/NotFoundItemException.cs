using System;

namespace SLiDS.Storage.Exceptions
{
    public class NotFoundItemByIdException<TClass, TId> : Exception
    {
        public NotFoundItemByIdException(TId id) : base($"В базе данных отсутствует {typeof(TClass).Name} с запрашиваемым ID. [ ID: {id} ]")
        {
        }
    }
}

