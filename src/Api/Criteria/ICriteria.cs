namespace SLiDS.Storage.Api.Criteria
{
    public interface ICriteria
    {
        FormatedCriteria Format(int paramNumber);

        static ICriteria Empty { get; } = new EmptyCriteria();
    }
}
