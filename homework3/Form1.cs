using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace homework3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap a,b,c,d,e;
        Rectangle clone;

        private void button1_Click(object sender, EventArgs e)
        {
            FileDialog tomatofile = new OpenFileDialog();
            if (tomatofile.ShowDialog()==DialogResult.OK)
            {
                a = new Bitmap(tomatofile.FileName);
                pictureBox1.Image = a;

                clone = new Rectangle(0, 0, a.Width, a.Height);
            }
            
        }
        private void button2_Click(object sender, EventArgs e)
        { //低通綠波按鈕
            mean();   //每個按鈕寫成不同的FUNCTION
        }

        private void button3_Click(object sender, EventArgs e)//shirnking
        { //值方圖收縮按鈕
            shrink();
        }

        private void button4_Click(object sender, EventArgs e)
        { //alpha - beta按鈕
            alpha();
            
        }
        private void button5_Click(object sender, EventArgs e)
        { //值方圖擴展按鈕

            strench();
        }

        private void button6_Click(object sender, EventArgs e)
        {  //一鍵到底的按鈕
            if (a == null)
            {
                MessageBox.Show("尚未載入圖片");
                return;
            }
            mean();
            shrink();
            alpha();
            strench();
        }
        public static int [,,] GetRGBData(Bitmap bitImg)
        {
            int height = bitImg.Height;
            int width = bitImg.Width;
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            // get the starting memory place
            IntPtr imgPtr = bitmapData.Scan0;
            //scan width
            int stride = bitmapData.Stride;
            //scan ectual
            int widthByte = width*3;
            // the byte num of padding
            int skipByte = stride - widthByte;
            //set the place to save values
            int[,,] rgbData = new int[width, height, 3];
            #region
            unsafe//專案－＞屬性－＞建置－＞容許Unsafe程式碼須選取。
            {
                byte* p = (byte*)(void*)imgPtr;
                for(int j=0;j<height;j++)
                {
                    for(int i=0;i<width;i++)
                    {
                        //B channel
                        rgbData[i, j, 2] = p[0];
                        p++;
                        //g channel
                        rgbData[i, j, 1] = p[0];
                        p++;
                        //R channel
                        rgbData[i, j, 0] = p[0];
                        p++;
                    }
                    p += skipByte;
                }
            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return rgbData;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public static Bitmap setRGBData(int [,,] rgbData)
        {
            Bitmap bitImg;
            int width = rgbData.GetLength(0);
            int height = rgbData.GetLength(1);
            bitImg = new Bitmap(width, height, PixelFormat.Format24bppRgb);// 24bit per pixel 8x8x8
            //locking
            BitmapData bitmapData = bitImg.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            //get image starting place
            IntPtr imgPtr = bitmapData.Scan0;
            //image scan width
            int stride = bitmapData.Stride;
            int widthByte = width * 3;
            int skipByte = stride - widthByte;
            #region
            unsafe
            {
                byte* p = (byte*)(void*)imgPtr;
                for(int j=0;j<height;j++)
                {
                    for(int i=0;i<width;i++)
                    {
                        p[0] = (byte)rgbData[i, j, 2];
                        p++;
                        p[0] = (byte)rgbData[i, j, 1];
                        p++;
                        p[0] = (byte)rgbData[i, j, 0];
                        p++;
                    }
                    p += skipByte;
                }

            }
            bitImg.UnlockBits(bitmapData);
            #endregion
            return bitImg;
        }
        void mean()
        {
            if (a == null)
            {
                MessageBox.Show("尚未載入圖片");
                return;
            }

            string kk = comboBox1.SelectedItem.ToString(); //取得矩陣SIZE
            int k = int.Parse(Convert.ToString(kk[0]));

            int[] total = new int[3];
            int[,] mean = new int[k, k];
            b = new Bitmap(a.Width, a.Height, PixelFormat.Format24bppRgb);
            int[,,] source,newimg;
            source = GetRGBData(a);
            newimg = new int[a.Width, a.Height, 3];
            for (int i = k / 2; i < a.Width - k / 2; i++)
            {
                for (int j = k / 2; j < a.Height - k / 2; j++)
                {
                    total[0] = 0;
                    total[1] = 0;
                    total[2] = 0;
                    for (int z = i - k / 2; z <= i + k / 2; z++)
                    {
                        for (int x = j - k / 2; x <= j + k / 2; x++)
                        {
                            total[0] += source[z, x, 0];
                            total[1] += source[z, x, 1];
                            total[2] += source[z, x, 2];
                        }
                    }
                    newimg[i, j, 0] = total[0] / (k * k);
                    newimg[i, j, 1] = total[1] / (k * k);
                    newimg[i, j, 2] = total[2] / (k * k);

                }
            }
            for(int i=0;i<k/2;i++)
            {
                for(int j=0;j<a.Width;j++)
                {
                    newimg[j, i, 0] = source[j, i, 0];
                    newimg[j, i, 1] = source[j, i, 1];
                    newimg[j, i, 2] = source[j, i, 2];
                    newimg[j, a.Height - i-1, 0] = source[j, a.Height - i-1, 0];
                    newimg[j, a.Height - i - 1, 1] = source[j, a.Height - i - 1, 1];
                    newimg[j, a.Height - i - 1, 2] = source[j, a.Height - i - 1, 2];
                }
                for(int j=0;j<a.Height;j++)
                {
                    newimg[i, j, 0] = source[i, j, 0];
                    newimg[i, j, 1] = source[i, j, 1];
                    newimg[i, j, 2] = source[i, j, 2];
                    newimg[a.Width - i - 1, j, 0] = source[a.Width - i - 1, j, 0];
                    newimg[a.Width - i - 1, j, 1] = source[a.Width - i - 1, j, 1];
                    newimg[a.Width - i - 1, j, 2] = source[a.Width - i - 1, j, 2];
                }
                
            }
            b = setRGBData(newimg);
            pictureBox2.Image = b;
        }
        void shrink()
        {  //收縮
            if (b == null)
            {
                MessageBox.Show("尚未載入圖片");
                return;
            }
            int[] imgmax = new int[3];
            int[] imgmin = new int[3];

            for (int i = 0; i < 3; i++)
            {
                imgmax[i] = 0;
                imgmin[i] = 255;
            }
           

            c = new Bitmap(b.Width, b.Height, PixelFormat.Format24bppRgb);
            int[,,] source = GetRGBData(b);
            int[,,] newimg = new int[b.Width, b.Height, 3];

            double max = Convert.ToDouble(textBox2.Text);
            int min= Convert.ToInt16(textBox1.Text);
            for (int z = 0; z < 3; z++)
            {
                for (int i = 0; i < c.Width; i++)
                {
                    for (int j = 0; j < c.Height; j++)
                    {
                        if (source[i, j, z] > imgmax[z])
                            imgmax[z] = source[i, j, z];
                        if (source[i, j, z] < imgmin[z])
                            imgmin[z] = source[i, j, z];
                    }
                }
                for (int i = 0; i < c.Width; i++)
                    for (int j = 0; j < c.Height; j++)
                    {
                        double k = (source[i, j, z] - imgmin[z]) * ((max - min) / (imgmax[z] - imgmin[z])) + min;
                        newimg[i, j, z] = Convert.ToInt16(k);
                    }
            }
            c = setRGBData(newimg);
            pictureBox3.Image = c;
        }
        void strench()
        { //值方圖擴展
            if (d == null)
            {
                MessageBox.Show("尚未載入圖片");
                return;
            }
            int[] imgmax = new int[3];
            int[] imgmin = new int[3];

            for (int i = 0; i < 3; i++)
            {
                imgmax[i] = 0;
                imgmin[i] = 255;
            }
           

            e = new Bitmap(d.Width, d.Height, PixelFormat.Format24bppRgb);
            int[,,] source = GetRGBData(d);
            int[,,] newimg = new int[d.Width, d.Height, 3];

            double max =255;
            int min= 0;
            for (int z = 0; z < 3; z++)
            {
                for (int i = 0; i < e.Width; i++)
                {
                    for (int j = 0; j < e.Height; j++)
                    {
                        if (source[i, j, z] > imgmax[z])
                            imgmax[z] = source[i, j, z];
                        if (source[i, j, z] < imgmin[z])
                            imgmin[z] = source[i, j, z];
                    }
                }
                for (int i = 0; i < e.Width; i++)
                    for (int j = 0; j < e.Height; j++)
                    {
                        double k = (source[i, j, z] - imgmin[z]) * ((max - min) / (imgmax[z] - imgmin[z])) + min;
                        newimg[i, j, z] = Convert.ToInt16(k);
                    }
            }
             e= setRGBData(newimg);
            pictureBox5.Image = e;
        }
        void alpha()
        {   // a- b
            if (c == null)
            {
                MessageBox.Show("尚未載入圖片");
                return;
            }
            int[,,] alpha = GetRGBData(a);
            int[,,] beta = GetRGBData(c);
            int temp;
            for (int i = 0; i < c.Width; i++)
            {
                for (int j = 0; j < c.Height; j++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        temp = Convert.ToInt16(alpha[i, j, k]) - Convert.ToInt16(beta[i, j, k]);
                        if (temp > 255)
                            temp = 255;
                        else if (temp < 0)
                            temp = 0;

                        beta[i, j, k] = temp;
                    }
                }
            }

            d = setRGBData(beta);
            pictureBox4.Image = d;
        }
    }
}
