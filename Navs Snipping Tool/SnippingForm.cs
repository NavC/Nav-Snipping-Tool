using Navs_Snipping_Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Navs_Snipping_Tool
{
    public partial class SnippingForm : Form
    {
        public SnippingForm()
        {
            InitializeComponent();
        }


        public Pen selectPen;

        private int displayWidth;
        private int displayHeight;



        //Draw rectangle
        Point startPos;      // mouse-down position
        Point currentPos;    // current mouse position
        bool drawing;        // busy drawing
        List<Rectangle> rectangles = new List<Rectangle>();  // previous rectangles

        //This variable control when you start the right click

        private void SnippingForm_Load(object sender, EventArgs e)
        {
            GetScreenSize();
            SnipImage();

        }

        private void GetScreenSize()
        {

            foreach (var screen in Screen.AllScreens)
            {
                // For each screen, add the screen properties to a console.
                Console.WriteLine("");
                Console.WriteLine("Device Name: " + screen.DeviceName);
                Console.WriteLine("Type: " + screen.GetType().ToString());
                Console.WriteLine("Working Area: " + screen.WorkingArea.ToString());
                Console.WriteLine("Primary Screen: " + screen.Primary.ToString());
                Console.WriteLine("Bounds: " + screen.Bounds.ToString());
                displayWidth += screen.Bounds.Width; //adds all the screens width together
                displayHeight += screen.Bounds.Height; //adds all the screens height together
            }

            this.Location = new Point(-6, 0);
            this.Width = displayWidth + 6;
            this.Height = displayHeight;

            pictureBox1.Size = new Size(displayWidth, displayHeight);
        }




        void SnipImage()
        {
            //Hide the Form
            this.Hide();
            //Create the Bitmap
            Bitmap printscreen = new Bitmap(displayWidth,
                                     displayHeight);
            //Create the Graphic Variable with screen Dimensions
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            //Copy Image from the screen
            graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
            //Create a temporal memory stream for the image
            using (MemoryStream s = new MemoryStream())
            {
                //save graphic variable into memory
                printscreen.Save(s, ImageFormat.Bmp);
                pictureBox1.Size = new System.Drawing.Size(this.Width, this.Height);
                //set the picture box with temporary stream
                pictureBox1.Image = Image.FromStream(s);
            }
            //Show Form
            this.Show();
            //Cross Cursor
            Cursor = Cursors.Cross;
        }

        private void SaveToClipboard()
        {

            //End application
            Application.Exit();
        }

        private Rectangle getRectangle()
        {
            return new Rectangle(
                Math.Min(startPos.X, currentPos.X),
                Math.Min(startPos.Y, currentPos.Y),
                Math.Abs(startPos.X - currentPos.X),
                Math.Abs(startPos.Y - currentPos.Y)
                );
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            currentPos = startPos = e.Location;
            drawing = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            currentPos = e.Location;
            if (drawing) pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawing)
            {
                drawing = false;
                var rc = getRectangle();
                if (rc.Width > 0 && rc.Height > 0) rectangles.Add(rc);
                pictureBox1.Invalidate();

                Console.WriteLine("Width: " + rc.Width + " Height: " + rc.Height);

                CopyToClipboard(rc);
            }

            this.Close();
            
        }

        // Copy the selected area to the clipboard.
        private void CopyToClipboard(Rectangle src_rect)
        {
            // Make a bitmap for the selected area's image.
            Bitmap bm = new Bitmap(src_rect.Width, src_rect.Height);

            // Copy the selected area into the bitmap.
            using (Graphics gr = Graphics.FromImage(bm))
            {
                Rectangle dest_rect =
                    new Rectangle(0, 0, src_rect.Width, src_rect.Height);
                gr.DrawImage(pictureBox1.Image, dest_rect, src_rect,
                    GraphicsUnit.Pixel);
            }

            // Copy the selection image to the clipboard.
            Clipboard.SetImage(bm);
        }


        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (rectangles.Count > 0) e.Graphics.DrawRectangles(Pens.Black, rectangles.ToArray());
            if (drawing) e.Graphics.DrawRectangle(Pens.Red, getRectangle());
        }
    }
}
