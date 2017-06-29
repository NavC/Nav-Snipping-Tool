using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Navs_Snipping_Tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //These variables control the mouse position
        private int selectX;
        private int selectY;
        private int selectWidth;
        private int selectHeight;
        public Pen selectPen;

        //This variable control when you start the right click
        private bool start = false;


        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RectangleSnip();
        }

        private void RectangleSnip()
        {
            if (rectangularSnipToolStripMenuItem.Checked == true)
            {

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            onload();
        }

        void onload()
        {
            //Hide the Form
            this.Hide();
            //Create the Bitmap
            Bitmap printscreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                     Screen.PrimaryScreen.Bounds.Height);
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

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //validate if there is an image
            if (pictureBox1.Image == null)
                return;
            //validate if right-click was trigger
            if (start)
            {
                //refresh picture box
                pictureBox1.Refresh();
                //set corner square to mouse coordinates
                selectWidth = e.X - selectX;
                selectHeight = e.Y - selectY;
                //draw dotted rectangle
                pictureBox1.CreateGraphics().DrawRectangle(selectPen,
                          selectX, selectY, selectWidth, selectHeight);
            }
        }
    }
}
