using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFChina.Course.Api.Models
{
    /// <summary>
    /// 每天课程
    /// </summary>
    public class DailyCourses
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 本天的所有课程
        /// </summary>
        public IList<Data.Course> Courses { get; set; }
    }
}