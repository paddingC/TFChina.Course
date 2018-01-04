using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFChina.Course.Data;

namespace TFChina.Course.Service
{
    /// <summary>
    /// export service
    /// </summary>
    public class ExporterService
    {
        private CourseService _courseService;
        private UserService _userService;

        public ExporterService()
        {
            _userService = new UserService();
            _courseService = new CourseService();
        } 

        //得到excel文件流
        public Stream ExportCoursesByUserId(int userId)
        {
            var user = _userService.GetUserById(userId);
            if (user == null) throw new Exception("未查找到用户：" + userId);
            if (user.IsAdmin.Value)
            {
                return ExportCoursesByAdmin();
            }
            else
            {
                return ExportCoursesByFellow(user.UserID);
            }
        }

        private Stream ExportCoursesByFellow(int fellowId)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("Course");
            IRow rowHeader = sheet.CreateRow(0);

            //生成excel标题
            rowHeader.CreateCell(0).SetCellValue("Date");
            rowHeader.CreateCell(1).SetCellValue("StartTime");
            rowHeader.CreateCell(2).SetCellValue("EndTime");
            rowHeader.CreateCell(3).SetCellValue("CourseCode");
            rowHeader.CreateCell(4).SetCellValue("CourseName");
            rowHeader.CreateCell(5).SetCellValue("Lecturer");
            rowHeader.CreateCell(6).SetCellValue("Class");

            var coursesByFellow = _courseService.GetCoursesByUserId(fellowId);
            //生成excel内容
            for (int i = 0; i < coursesByFellow.Count; i++)
            {
                IRow rowtemp = sheet.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(coursesByFellow[i].Date?.ToString("yyyy-MM-dd"));
                rowtemp.CreateCell(1).SetCellValue(coursesByFellow[i].StartTime);
                rowtemp.CreateCell(2).SetCellValue(coursesByFellow[i].EndTime);
                rowtemp.CreateCell(3).SetCellValue(coursesByFellow[i].CourseCode);
                rowtemp.CreateCell(4).SetCellValue(coursesByFellow[i].CourseName);
                rowtemp.CreateCell(5).SetCellValue(coursesByFellow[i].Lecturer);
                rowtemp.CreateCell(6).SetCellValue(coursesByFellow[i].Class);
            }

            for (int i = 0; i <=6 ; i++)
                sheet.AutoSizeColumn(i);

            MemoryStream stream = new MemoryStream();
            hssfworkbook.Write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private Stream ExportCoursesByAdmin()
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            var allCourses = _courseService.GetCourses();
            var allUsers= _userService.GetAll();
            var existedCourseUsers = GetAllCourseUser();

            if (allCourses.Count() <= 0)
            {
                //如果没有课程数据，则只生成表头
                ISheet sheet = hssfworkbook.CreateSheet("Demo");
                IRow courseHeader = sheet.CreateRow(0);

                //生成课程标题
                courseHeader.CreateCell(0).SetCellValue("Date");
                courseHeader.CreateCell(1).SetCellValue("StartTime");
                courseHeader.CreateCell(2).SetCellValue("EndTime");
                courseHeader.CreateCell(3).SetCellValue("CourseCode");
                courseHeader.CreateCell(4).SetCellValue("CourseName");
                courseHeader.CreateCell(5).SetCellValue("CourseDetail");
                courseHeader.CreateCell(6).SetCellValue("TotalStudent");
                courseHeader.CreateCell(7).SetCellValue("Class");
                courseHeader.CreateCell(8).SetCellValue("Region");

                //生成学员列表表头
                IRow fellowListHeader = sheet.CreateRow(3);
                fellowListHeader.CreateCell(0).SetCellValue("学员列表");
            }

            allCourses.ToList().ForEach(course => {
                ISheet sheet = hssfworkbook.CreateSheet(course.Lecturer + "-" + course.CourseID);
                IRow courseHeader = sheet.CreateRow(0);

                //生成课程标题
                courseHeader.CreateCell(0).SetCellValue("Date");
                courseHeader.CreateCell(1).SetCellValue("StartTime");
                courseHeader.CreateCell(2).SetCellValue("EndTime");
                courseHeader.CreateCell(3).SetCellValue("CourseCode");
                courseHeader.CreateCell(4).SetCellValue("CourseName");
                courseHeader.CreateCell(5).SetCellValue("CourseDetail");
                courseHeader.CreateCell(6).SetCellValue("TotalStudent");
                courseHeader.CreateCell(7).SetCellValue("Class");
                courseHeader.CreateCell(8).SetCellValue("Region");


                var userIds = existedCourseUsers.Where(p => p.CourseID == course.CourseID).Select(p => p.UserID).Distinct().ToList();
                //生成课程内容
                IRow courseRow = sheet.CreateRow(1);
                courseRow.CreateCell(0).SetCellValue(course.Date?.ToString("YYYY-MM-DD"));
                courseRow.CreateCell(1).SetCellValue(course.StartTime);
                courseRow.CreateCell(2).SetCellValue(course.EndTime);
                courseRow.CreateCell(3).SetCellValue(course.CourseCode);
                courseRow.CreateCell(4).SetCellValue(course.CourseName);
                courseRow.CreateCell(5).SetCellValue(course.CourseDetail);
                courseRow.CreateCell(6).SetCellValue(userIds.Count());
                courseRow.CreateCell(7).SetCellValue(course.Class);
                courseRow.CreateCell(8).SetCellValue(_courseService.GetRegionDescription(course.Region));

                //生成学员列表表头
                IRow fellowListHeader = sheet.CreateRow(3);
                fellowListHeader.CreateCell(0).SetCellValue("学员列表");

                //生成学员列表
                var rowIndex = 3;
                userIds.ForEach(userId => {
                    var user = allUsers.FirstOrDefault(tempUser => tempUser.UserID == userId);
                    if (user == null) return;
                    rowIndex++;
                    IRow rowTemp = sheet.CreateRow(rowIndex);
                    rowTemp.CreateCell(0).SetCellValue(user.Name);
                });

                for (int i = 0; i <= 8; i++)
                    sheet.AutoSizeColumn(i);
            });

            MemoryStream stream = new MemoryStream();
            hssfworkbook.Write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private IList<CourseUser> GetAllCourseUser()
        {
            using (var dbContext = new CourseDbContext())
            {
                return dbContext.CourseUsers.ToList();
            }
        }

        public Stream ExportCoursesByRegion(string region)
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            var courses = _courseService.GetCoursesByRegion(region);
            var allUsers = _userService.GetAll();
            var existedCourseUsers = GetAllCourseUser();

            if (courses.Count() <= 0)
            {
                //如果没有课程数据，则只生成表头
                ISheet sheet = hssfworkbook.CreateSheet("Demo");
                IRow courseHeader = sheet.CreateRow(0);

                //生成课程标题
                courseHeader.CreateCell(0).SetCellValue("Date");
                courseHeader.CreateCell(1).SetCellValue("StartTime");
                courseHeader.CreateCell(2).SetCellValue("EndTime");
                courseHeader.CreateCell(3).SetCellValue("CourseCode");
                courseHeader.CreateCell(4).SetCellValue("CourseName");
                courseHeader.CreateCell(5).SetCellValue("CourseDetail");
                courseHeader.CreateCell(6).SetCellValue("TotalStudent");
                courseHeader.CreateCell(7).SetCellValue("Class");
                courseHeader.CreateCell(8).SetCellValue("Region");

                //生成学员列表表头
                IRow fellowListHeader = sheet.CreateRow(3);
                fellowListHeader.CreateCell(0).SetCellValue("学员列表");
            }

            courses.ToList().ForEach(course => {
                ISheet sheet = hssfworkbook.CreateSheet(course.Lecturer + "-" + course.CourseID);
                IRow courseHeader = sheet.CreateRow(0);

                //生成课程标题
                courseHeader.CreateCell(0).SetCellValue("Date");
                courseHeader.CreateCell(1).SetCellValue("StartTime");
                courseHeader.CreateCell(2).SetCellValue("EndTime");
                courseHeader.CreateCell(3).SetCellValue("CourseCode");
                courseHeader.CreateCell(4).SetCellValue("CourseName");
                courseHeader.CreateCell(5).SetCellValue("CourseDetail");
                courseHeader.CreateCell(6).SetCellValue("TotalStudent");
                courseHeader.CreateCell(7).SetCellValue("Class");
                courseHeader.CreateCell(8).SetCellValue("Region");

                var userIds = existedCourseUsers.Where(p => p.CourseID == course.CourseID).Select(p => p.UserID).Distinct().ToList();
                //生成课程内容
                IRow courseRow = sheet.CreateRow(1);
                courseRow.CreateCell(0).SetCellValue(course.Date?.ToString("YYYY-MM-DD"));
                courseRow.CreateCell(1).SetCellValue(course.StartTime);
                courseRow.CreateCell(2).SetCellValue(course.EndTime);
                courseRow.CreateCell(3).SetCellValue(course.CourseCode);
                courseRow.CreateCell(4).SetCellValue(course.CourseName);
                courseRow.CreateCell(5).SetCellValue(course.CourseDetail);
                courseRow.CreateCell(6).SetCellValue(userIds.Count());
                courseRow.CreateCell(7).SetCellValue(course.Class);
                courseRow.CreateCell(8).SetCellValue(_courseService.GetRegionDescription(course.Region));

                //生成学员列表表头
                IRow fellowListHeader = sheet.CreateRow(3);
                fellowListHeader.CreateCell(0).SetCellValue("学员列表");

                //生成学员列表
                var rowIndex = 3;
                userIds.ForEach(userId => {
                    var user = allUsers.FirstOrDefault(tempUser => tempUser.UserID == userId);
                    if (user == null) return;
                    rowIndex++;
                    IRow rowTemp = sheet.CreateRow(rowIndex);
                    rowTemp.CreateCell(0).SetCellValue(user.Name);
                });

                for (int i = 0; i <= 8; i++)
                    sheet.AutoSizeColumn(i);
            });

            MemoryStream stream = new MemoryStream();
            hssfworkbook.Write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public Stream ExportUsers()
        {
            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("Users");
            IRow rowHeader = sheet.CreateRow(0);

            //生成excel标题
            rowHeader.CreateCell(0).SetCellValue("UserID");
            rowHeader.CreateCell(1).SetCellValue("Account");
            rowHeader.CreateCell(2).SetCellValue("Name");
            rowHeader.CreateCell(3).SetCellValue("Password");
            rowHeader.CreateCell(4).SetCellValue("Region");
            rowHeader.CreateCell(5).SetCellValue("Sector");
            rowHeader.CreateCell(6).SetCellValue("IsMainTeacher");
            rowHeader.CreateCell(7).SetCellValue("IsAdmin");

            var allUsers = _userService.GetAll();
            //生成excel内容
            for (int i = 0; i < allUsers.Count; i++)
            {
                IRow rowtemp = sheet.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(allUsers[i].UserID);
                rowtemp.CreateCell(1).SetCellValue(allUsers[i].Account);
                rowtemp.CreateCell(2).SetCellValue(allUsers[i].Name);
                rowtemp.CreateCell(3).SetCellValue(allUsers[i].Password);
                rowtemp.CreateCell(4).SetCellValue(_courseService.GetRegionDescription(allUsers[i].Region));
                rowtemp.CreateCell(5).SetCellValue(allUsers[i].Sector);
                rowtemp.CreateCell(6).SetCellValue(allUsers[i].IsMainTeacher.HasValue && allUsers[i].IsMainTeacher.Value ? 1 : 0);
                rowtemp.CreateCell(7).SetCellValue(allUsers[i].IsAdmin.HasValue && allUsers[i].IsAdmin.Value ? 1 : 0);
            }

            for (int i = 0; i <= 7; i++)
                sheet.AutoSizeColumn(i);

            MemoryStream stream = new MemoryStream();
            hssfworkbook.Write(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
