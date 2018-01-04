using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TFChina.Course.Api.Models;
using TFChina.Course.Service;

namespace TFChina.Course.Api.Controllers
{
    /// <summary>
    /// Import controller 
    /// </summary>
    public class ImportController : ApiController
    {

        private CourseParser _courseParser;
        private CourseService _courseService;
        public ImportController()
        {
            _courseParser = new CourseParser();
            _courseService = new CourseService();
        }

        // POST api/<controller>
        [Route("v1/import/courses")]
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            try
            {
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
              
                // Read the form data.
                await Request.Content.ReadAsMultipartAsync(provider);
                // This illustrates how to get the file names.
                foreach (MultipartFileData file in provider.FileData)
                {
                    //Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                    //Trace.WriteLine("Server file path: " + file.LocalFileName);
                    using (var fileStream = new FileStream(file.LocalFileName, FileMode.Open))
                    {
                        //1.读取Excel文件

                        //2.解析Excel文件中的课程信息，组织成Course对象
                        //2.1根据课时，拆分成课时为1的单课时课程
                        var originalCourseList = _courseParser.ParseFromExcel(fileStream);
                        var finalCourseList = _courseParser.SplitCourses(originalCourseList);
                        //3.保存到数据库
                        _courseService.AddCourses(finalCourseList);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}