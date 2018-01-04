using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using TFChina.Course.Api.Models;
using TFChina.Course.Service;

namespace TFChina.Course.Api.Controllers
{
    /// <summary>
    /// Export controller
    /// </summary>
    public class ExportController : ApiController
    {
        private readonly UserService _userService;
        private readonly CourseService _courseService;
        private readonly ExporterService _exporterService;


        public ExportController()
        {
            _userService = new UserService();
            _courseService = new CourseService();
            _exporterService = new ExporterService();
        }

        [Route("v1/export/users")]
        [HttpGet]
        public HttpResponseMessage ExportUsers()
        {
            try
            {
                var streamData = _exporterService.ExportUsers();
                return CreateExportStream(streamData, "users_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            }
            catch (Exception ex)
            {
                return CreateExceptionResponse(ex);
            }
       }

        [Route("v1/export/courses/region/{region}")]
        [HttpGet]
        public HttpResponseMessage ExportCoursesByRegion(string region)
        {
            try
            {
                var streamData = _exporterService.ExportCoursesByRegion(region);
                return CreateExportStream(streamData, "courses_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            }
            catch (Exception ex)
            {
                return CreateExceptionResponse(ex);
            }
        }

        [Route("v1/export/courses/user/{userId}")]
        [HttpGet]
        public HttpResponseMessage ExportCoursesByUserId(string userId)
        {
            try
            {
                if (!Int32.TryParse(userId, out int nUseId) || nUseId <= 0 )
                {
                    throw new Exception("userId必须为数字且大于0");
                }
                var streamData = _exporterService.ExportCoursesByUserId(nUseId);
                return CreateExportStream(streamData, "courses_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            }
            catch (Exception ex)
            {
                return CreateExceptionResponse(ex);
            }
        }

        private HttpResponseMessage CreateExportStream(Stream stream,string fileName)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = fileName;
            return result;
        }

        private HttpResponseMessage CreateExceptionResponse(Exception ex)
        {
            var tempDto = new DataTransferObject { Status = 0, Message = ex.Message + "\r\n" + ex.StackTrace };
            var messageContent = Newtonsoft.Json.JsonConvert.SerializeObject(tempDto);
            return new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent(messageContent) };
        }
    }
}