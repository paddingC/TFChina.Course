using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TFChina.Course.Data;

namespace TFChina.Course.Api.Models
{
    /// <summary>
    /// 用户输出
    /// </summary>
    public class UsersOutput
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int Count { get; set; }

        public IList<User> Users { get; set; }
    }
}