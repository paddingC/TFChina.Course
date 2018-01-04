using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TFChina.Course.Api.Models
{
    public class ResetPasswordModel
    {
        /// <summary>
        /// Account
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// confirmed password
        /// </summary>
        public string ConfirmedPassword { get; set; }
    }
}