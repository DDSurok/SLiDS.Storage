using SLiDS.Samples.ObjectSamples;
using SLiDS.Storage;

namespace SLiDS.Samples.RepoSamples
{
    internal class DbRepositorySample : DbContext<DbObject, int>
    {
        public DbRepositorySample(string connectionString) : base(connectionString)
        {
        }
        private DbRepositorySample() : base("")
        {
        }
    }
}
