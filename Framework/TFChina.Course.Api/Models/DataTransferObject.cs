using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFChina.Course.Api.Models
{
    /// <summary>
    /// Data Transfer Object
    /// </summary>
    public class DataTransferObject
    {
        /// <summary>
        /// 状态：1：成功；0:失败
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 服务端提供的数据
        /// </summary>
        public dynamic Result { get; set; }

        /// <summary>
        /// 消息：例如：服务端发生的异常
        /// </summary>
        public string Message { get; set; }
    }
}