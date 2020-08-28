using SLiDS.Storage.Api;
using System;

namespace SLiDS.Samples.ObjectSamples
{
    class MemoryObject : IObject<int>
    {
        public int Id { get; set; }
        public string Data { get; set; }
        public DateTime Created { get; set; }
    }
}
