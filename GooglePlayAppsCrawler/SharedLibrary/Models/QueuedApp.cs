using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models
{
    public class QueuedApp
    {
        public ObjectId _id { get; set; }
        public String Url   { get; set; }
        public bool IsBusy { get; set; }

    }
}
