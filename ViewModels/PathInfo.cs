using JanaltaPathProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web;

namespace JanaltaPathProject.ViewModels
{
    public class PathInfo
    {
        [DisplayName("Last Point")]
        public Point LastPoint { get; set; }
        public List<PointInfo> DuplicatedPoint { get; set; }
        
        [DisplayName("Path Image")]
        public String ImageData { get; set; }
    }
}