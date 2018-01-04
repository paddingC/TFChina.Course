using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFChina.Course.Api.Models
{
    /// <summary>
    /// 被选中的课程
    /// </summary>
    public class SelectedCourseModel
    {
        public int UserId { get; set; }

        public IList<int> CourseIds { get; set; }
    }
}