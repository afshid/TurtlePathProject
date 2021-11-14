using JanaltaPathProject.Models;
using JanaltaPathProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JanaltaPathProject.Controllers
{
    public class MovementController : Controller
    {
        readonly Point startPoint;
        private Point lastMove;
        private List<Point> pathPointList;
        private Direction currentDirection;
        const int stepLenght = 4;
        private PathInfo pathInfo;
        private int imageHeight;
        private int imageWidth;
        public MovementController()
        {
            startPoint = new Point(0, 0);
            pathPointList = new List<Point>();
            pathPointList.Add(startPoint);
            pathInfo = new PathInfo();
            imageHeight = 500;
            imageWidth = 500;
    }

        [HttpPost]
        public ActionResult ProcessTextFile(HttpPostedFileBase file)
        {
            try
            {
                BinaryReader b = new BinaryReader(file.InputStream);
                byte[] binData = b.ReadBytes(file.ContentLength);
                string fileContent = System.Text.Encoding.UTF8.GetString(binData);
                var PathModel = CreatePathInformation(fileContent);

                return View("Index", PathModel);
            }
            catch
            {
                ViewBag.Message = "File upload failed!!";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            string fileName = "directions-1.txt";

            var fileContent = System.IO.File.ReadAllText(Server.MapPath($"~/App_Data/{fileName}"));

            var PathModel = CreatePathInformation(fileContent);

            return View(PathModel);
        }

        [NonAction]
        private String CreatePathImage()
        {
            Bitmap bmp = new Bitmap(
            imageWidth, imageHeight, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.LightGray);
            var  centerPoint = CoordinateCalculator(pathPointList);
            g.TranslateTransform(imageWidth / 2 - centerPoint.X,  imageHeight / 2 - centerPoint.Y);
            g.DrawLines(Pens.Black, pathPointList.ToArray());
            bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            bmp.Save(memoryStream, ImageFormat.Jpeg);
            g.Dispose();
            bmp.Dispose();
            string imreBase64Data = Convert.ToBase64String(memoryStream.ToArray());
            string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
            return imgDataURL;
        }

        [NonAction]
        private void CreatePathPoints(string pathString)
        {
            List<char> pathcharList = new List<char>(pathString.Trim());
            currentDirection = Direction.Up;
            lastMove = pathPointList.Last<Point>();
            foreach (var move in pathcharList)
            {
                SetCurrentDirection(move);
                lastMove = CreateNewPoint();
                pathPointList.Add(lastMove);
            }
        }

        [NonAction]
        private Point CreateNewPoint()
        {
            switch (currentDirection)
            {
                case Direction.Left:
                    return new Point(lastMove.X - stepLenght, lastMove.Y);
                case Direction.Right:
                    return new Point(lastMove.X + stepLenght, lastMove.Y);
                case Direction.Up:
                    return new Point(lastMove.X, lastMove.Y + stepLenght);
                case Direction.Down:
                    return new Point(lastMove.X, lastMove.Y - stepLenght);
            }
            return new Point(0, 0);
        }

        [NonAction]
        private void SetCurrentDirection(char currentChar)
        {
            if (currentChar == 'L')
                currentDirection = ((int)currentDirection != 4) ? currentDirection + 1 : Direction.Up;
            if (currentChar == 'R')
                currentDirection = ((int)currentDirection != 1) ? currentDirection - 1 : Direction.Right;
        }

        [NonAction]
        private List<PointInfo> GetDuplicatePoint()
        {
            return pathPointList.GroupBy(p => p).Where(g => g.Count() > 1).Select(m => new PointInfo { LocPoint = m.Key, Count = m.Count() }).ToList();
        }

        [NonAction]
        private PathInfo CreatePathInformation(string pathString)
        {

            CreatePathPoints(pathString);
            pathInfo.DuplicatedPoint = GetDuplicatePoint();
            pathInfo.LastPoint = pathPointList.LastOrDefault();
            pathInfo.ImageData = CreatePathImage();
            return pathInfo;
        }

        [NonAction]
        private Point CoordinateCalculator(List<Point> pointList)
        {
            int minX = pathPointList.Min(p => p.X);
            int maxX = pathPointList.Max(p => p.X);
            int minY = pathPointList.Min(p => p.Y);
            int maxY = pathPointList.Max(p => p.Y);
            imageHeight =Math.Abs(maxY - minY)+10;
            imageWidth = Math.Abs(maxX - minX)+10;
            return new Point((minX + maxX) / 2, (minY + maxY) / 2);
        }
    }
}