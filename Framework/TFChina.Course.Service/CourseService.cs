using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFChina.Course.Data;

namespace TFChina.Course.Service
{
    public class CourseService
    {
        /// <summary>
        /// Adds courses
        /// </summary>
        /// <param name="courseIDs"></param>
        /// <param name="account"></param>
        public void AddCourses(IList<int> courseIDs,int userId)
        {
            using (var dbContext = new CourseDbContext())
            {
                var user = dbContext.Users.FirstOrDefault(p => p.UserID == userId);
                if (user == null) throw new Exception("未查找到用户：" + userId);

                var courses = dbContext.Courses.Where(p=>courseIDs.Contains(p.CourseID)).Distinct();
                courses.ToList().ForEach(p=> {
                    //DO:检测每节课的最大上课人数是否达到学员上限
                    var exsitedCourseUsers = dbContext.CourseUsers.Where(p1=>p1.CourseID==p.CourseID).Select(p1=>new { p1.UserID,p1.CourseID}).Distinct();
                    if (exsitedCourseUsers.Count() >= p.MaxNumber)
                    {
                        //已达课程最大上限
                        throw new Exception("课程["+p.CourseID+":"+p.CourseName+"]"+"已经达到上限");
                    }

                    dbContext.CourseUsers.Add(new CourseUser {UserID=user.UserID,CourseID=p.CourseID});
                });
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Adds courses
        /// </summary>
        /// <param name="courseIDs"></param>
        /// <param name="account"></param>
        public void AddCourses(IList<Data.Course> courseList)
        {
            if (courseList == null) throw new ArgumentNullException("courseList");

            using (var dbContext = new CourseDbContext())
            {
                //courseList.ToList().ForEach(p=>dbContext.Courses.Add(p));
                dbContext.Courses.AddRange(courseList);
                dbContext.SaveChanges();
            }
        }


        /// <summary>
        /// gets courses
        /// </summary>
        public IList<Data.Course> GetCourses()
        {
            using (var dbContext = new CourseDbContext())
            {
                var result = from oCourse in dbContext.Courses 
                             select oCourse;
                return result.ToList();
            }
        }


        /// <summary>
        /// gets courses by region
        /// </summary>
        public IList<Data.Course> GetCoursesByRegion(string region)
        {
            using (var dbContext = new CourseDbContext())
            {
                var result = from oCourse in dbContext.Courses.Where(p=>p.Region == region)
                             select oCourse;
                return result.ToList();
            }
        }


        /// <summary>
        /// Gets courses by user
        /// </summary>
        public IList<Data.Course> GetCoursesByUserId(int userId)
        {
            using (var dbContext = new CourseDbContext())
            {
                var result = from oUser in dbContext.Users.Where(p=>p.UserID==userId)
                             join oUserCourse in dbContext.CourseUsers on oUser.UserID equals oUserCourse.UserID
                             join oCourse in dbContext.Courses on oUserCourse.CourseID equals oCourse.CourseID
                             select oCourse;
                return result.ToList();
            }
        }

        public string GetRegionDescription(string regionCode)
        {
            var result = string.Empty;
            switch (regionCode)
            {
                case "YM":
                    result = "粤闽";
                    break;
                case "GG":
                    result = "广甘";
                    break;
                case "YN":
                    result = "云南";
                    break;
                default:
                    result = string.Empty;
                    break;
            }
            return result;
        }
    }
}
