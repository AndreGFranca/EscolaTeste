using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EscolaTeste.Domain.Commom
{
    public class RedisKeys
    {
        public static string ClassGroup(string hashQuery, long version)
            => $"classgroup:{version}:{hashQuery}";

        public static string Students(string hashQuery, long version)
            => $"student:{version}:{hashQuery}";

        public static string StudentById(int id, long version)
            => $"student:{version}:{id}";
        public static string ReportStudentPerClassGroup(long version)
            => $"report:{version}";

        public static string Version(string prefix)
            => $"{prefix}:version";
    }
}