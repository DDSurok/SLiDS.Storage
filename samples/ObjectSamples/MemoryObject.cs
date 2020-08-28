using SLiDS.Storage.Api;
using System;

namespace SLiDS.Samples.ObjectSamples
{
    internal class MemoryObject : IObject<Guid>
    {
        public Guid Id { get; set; }
        public string Data { get; set; }
        public DateTime Created { get; set; }
    }
}
