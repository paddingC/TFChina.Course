using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFChina.Course.Service
{
    /// <summary>
    /// 课程解析
    /// </summary>
    public class CourseParser
    {
        private IList<Duration> _schedule;

        public CourseParser()
        {
            _schedule = new List<Duration>
            {
               new Duration { Index = 0, Start = "8:30", End = "9:15" },
               new Duration { Index = 1, Start = "9:25", End = "10:10"  },
               new Duration { Index = 2, Start = "10:20", End = "11:05" },
               new Duration { Index = 3, Start = "11:15", End = "12:00" },
               new Duration { Index = 4, Start = "14:00", End = "14:45" },
               new Duration { Index = 5, Start = "14:55", End = "15:40" },
               new Duration { Index = 6, Start = "15:50", End = "16:35" },
               new Duration { Index = 7, Start = "16:45", End = "17:30" }
            };
        }

        /// <summary>
        /// 从Excel解析课程数据
        /// </summary>
        /// <param name="excelStream"></param>
        /// <returns></returns>
        public IList<Data.Course> ParseFromExcel(Stream excelStream)
        {
            XSSFWorkbook workbook = new XSSFWorkbook(excelStream);
            //获取excel的第一个sheet  
            ISheet sheet = workbook.GetSheetAt(0);
            IList<Data.Course> courseList = new List<Data.Course>();

            IEnumerator rows = sheet.GetRowEnumerator();
            while (rows.MoveNext())
            {
                IRow row = (IRow)rows.Current;
                if (row.RowNum == 0) continue;//第一行表头，跳过
                if (row == null) break;//读到
                var rowIndex = sheet.PhysicalNumberOfRows;

                var tempCourse = new Data.Course();
                tempCourse.Category = row.GetCell(0)?.StringCellValue;
                tempCourse.Region = row.GetCell(1)?.StringCellValue;
                tempCourse.Group = row.GetCell(2)?.StringCellValue;
                tempCourse.CourseName = row.GetCell(3)?.StringCellValue;
                tempCourse.CourseDetail = row.GetCell(4)?.StringCellValue;
                tempCourse.Lecturer = row.GetCell(5)?.StringCellValue;
                tempCourse.LecturerDetail = row.GetCell(6)?.StringCellValue;
                tempCourse.Date = row.GetCell(7)?.DateCellValue;
                tempCourse.StartTime = row.GetCell(8)?.DateCellValue.ToShortTimeString();
                tempCourse.EndTime = row.GetCell(9)?.DateCellValue.ToShortTimeString();
                tempCourse.ClassHour = (int)row.GetCell(10)?.NumericCellValue;
                tempCourse.MaxNumber = (int)row.GetCell(11)?.NumericCellValue;
                tempCourse.Class = row.GetCell(12)?.StringCellValue;
                if (row.GetCell(13).NumericCellValue.ToString() == "1")
                {
                    tempCourse.Sector = "17-19";
                }

                if (row.GetCell(14).NumericCellValue.ToString() == "1")
                {
                    tempCourse.Career = "17-19";
                }

                if (row.GetCell(15).NumericCellValue.ToString() == "1")
                {
                    tempCourse.Career = "16-18";
                }

                if (row.GetCell(16).NumericCellValue.ToString() == "1")
                {
                    tempCourse.MainTeacher = true;
                }

                courseList.Add(tempCourse);
            }


            return courseList;
        }

        /// <summary>
        /// 拆分课程：如果课时大于1，则拆分为多个单课时课程
        /// </summary>
        /// <returns></returns>
        public IList<Data.Course> SplitCourses(IList<Data.Course> courseList)
        {
            var result = new List<Data.Course>();
            courseList.ToList().ForEach(course => {
                if (course.ClassHour == 1)
                {
                    result.Add(course);
                    return;
                }
                if (course.ClassHour > 1) {
                    result.AddRange(SplitCourses(course));
                }
            });
            return result;
        }

        /// <summary>
        /// 拆分课时
        /// </summary>
        /// <param name="course"></param>
        /// <returns></returns>
        private IList<Data.Course> SplitCourses(Data.Course course)
        {
            var result = new List<Data.Course>();
            if (course.ClassHour == 1)
            {
                result.Add(course);
                return result;
            }

            if (course.ClassHour > 1)
            {
                //开始时间匹配
                var duration = _schedule.FirstOrDefault(p=>p.Start==course.StartTime);
                if (duration == null) throw new Exception("未找到对应时段");

                for (var index = duration.Index; index < duration.Index + course.ClassHour; index++ )
                {
                    var tempDuration = _schedule.FirstOrDefault(p => p.Index == index);
                    if (duration == null) throw new Exception("未找到对应时段");
                    var tempCourse = new Data.Course();
                    tempCourse.Category = course.Category;
                    tempCourse.Region = course.Region;
                    tempCourse.Group = course.Group;
                    tempCourse.CourseName = course.CourseName;
                    tempCourse.CourseDetail = course.CourseDetail;
                    tempCourse.Lecturer = course.Lecturer;
                    tempCourse.LecturerDetail = course.LecturerDetail;
                    tempCourse.Date = course.Date;
                    tempCourse.StartTime = tempDuration.Start;
                    tempCourse.EndTime = tempDuration.End;
                    tempCourse.ClassHour = 1;
                    tempCourse.MaxNumber = course.MaxNumber;
                    tempCourse.Class = course.Class;
                    tempCourse.Sector = course.Sector;
                    tempCourse.Career = course.Career;
                    result.Add(tempCourse);
                }
            }
            return result;
        }
    }
}
