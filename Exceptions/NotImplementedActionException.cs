using System;

namespace SLiDS.Storage.Exceptions
{
    public class NotImplementedActionException<TClass> : NotSupportedException
    {
        public NotImplementedActionException() : base($"Запрощенная операция недопускается для {typeof(TClass).Name}.") { }
    }

    public class NotSupportedActionInTransactionException : NotSupportedException
    {
        public NotSupportedActionInTransactionException() : base("Запрощенная операция не поддерживает использование транзакций.") { }
    }
}

