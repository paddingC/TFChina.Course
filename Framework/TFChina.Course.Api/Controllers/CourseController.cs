using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TFChina.Course.Api.Models;
using TFChina.Course.Service;

namespace TFChina.Course.Api.Controllers
{
    public class CourseController : ApiController
    {
        private readonly CourseService _courseService;
        public CourseController()
        {
            _courseService = new CourseService();
        }

        [Route("v1/courses")]
        public DataTransferObject GetCourses()
        {
            try
            {
                var dailyCourseList = new List<DailyCourses>();
                var result = _courseService.GetCourses();
                result.ToList().ForEach(p=> {
                    var tempDailyCourses = dailyCourseList.FirstOrDefault(p1=>p1.Date==p.Date);
                    if (tempDailyCourses == null)
                    {
                        tempDailyCourses = new DailyCourses { Date = p.Date.Value, Courses = new List<Data.Course>() };
                        dailyCourseList.Add(tempDailyCourses);
                    }
                    tempDailyCourses.Courses.Add(p);
                });

                //按天排序
                dailyCourseList = dailyCourseList.OrderBy(p => p.Date).ToList();
                //每天内的课程按照开始时间排序
                dailyCourseList.ForEach(p => {
                    p.Courses = p.Courses.OrderBy(p1=>p1.StartTime).ToList();
                });

                return new DataTransferObject{ Status = 1,Result = dailyCourseList };
            }
            catch (Exception ex)
            {
                return new DataTransferObject{ Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace};
            }
        }

        [Route("v1/courses/region/{region}")]
        public DataTransferObject GetCoursesByRegion(string region)
        {
            try
            {
                var dailyCourseList = new List<DailyCourses>();
                var result = _courseService.GetCoursesByRegion(region);
                result.ToList().ForEach(p => {
                    var tempDailyCourses = dailyCourseList.FirstOrDefault(p1 => p1.Date == p.Date);
                    if (tempDailyCourses == null)
                    {
                        tempDailyCourses = new DailyCourses { Date = p.Date.Value, Courses = new List<Data.Course>() };
                        dailyCourseList.Add(tempDailyCourses);
                    }
                    tempDailyCourses.Courses.Add(p);
                });

                //按天排序
                dailyCourseList = dailyCourseList.OrderBy(p => p.Date).ToList();
                //每天内的课程按照开始时间排序
                dailyCourseList.ForEach(p => {
                    p.Courses = p.Courses.OrderBy(p1 => p1.StartTime).ToList();
                });

                return new DataTransferObject { Status = 1, Result = dailyCourseList.OrderBy(p => p.Date).ToList() };
            }
            catch (Exception ex)
            {
                return new DataTransferObject { Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace };
            }
        }

        [Route("v1/courses/user/{userId}")]
        public DataTransferObject GetCoursesByUserId(string userId)
        {
            try
            {
                if (!Int32.TryParse(userId, out int nUserId)||nUserId<=0)
                {
                    return new DataTransferObject { Status = 0, Message="UserId必须为数字且大于0" };
                }

                var result = _courseService.GetCoursesByUserId(nUserId).OrderBy(p=>p.Date).ThenBy(p=>p.StartTime).ToList();
                return new DataTransferObject { Status = 1, Result = result };
            }
            catch (Exception ex)
            {
                return new DataTransferObject { Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace };
            } 
        }

        [Route("v1/courses/user")]
        public DataTransferObject Post([FromBody]SelectedCourseModel model)
        {
            try
            {
                _courseService.AddCourses(model.CourseIds, model.UserId);
                return new DataTransferObject { Status = 1 };
            }
            catch (Exception ex)
            {
                return new DataTransferObject { Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace };
            }  
        }
    }
}