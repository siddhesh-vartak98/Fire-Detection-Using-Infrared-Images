

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using AForge;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.Media;




namespace cam_aforge1
{
   
    public partial class Form1 : Form

    {
        private bool DeviceExist = false;
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource = null;
        //private FileVideoSource videoSource = null;

        

        public Form1()
        {
            InitializeComponent();
        }

        private void getCamList()
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                comboBox1.Items.Clear();
                if (videoDevices.Count == 0)
                    throw new ApplicationException();

                DeviceExist = true;
                foreach (FilterInfo device in videoDevices)
                {
                    comboBox1.Items.Add(device.Name);
                }
                comboBox1.SelectedIndex = 0; //make dafault to first cam
            }
            catch (ApplicationException)
            {
                DeviceExist = false;
                comboBox1.Items.Add("No capture device on your system");
            }
        }

        private void rfsh_Click(object sender, EventArgs e)
        {
            getCamList();
        }

        private void start_Click(object sender, EventArgs e)
        {
            if (start.Text == "&Start")
            {
                if (DeviceExist)
                {
                    videoSource = new VideoCaptureDevice(videoDevices[comboBox1.SelectedIndex].MonikerString);
                   // videoSource = new VideoCaptureDevice();
                   //  videoSource = new FileVideoSource("IR2.mp4");
                   videoSource.DesiredFrameSize = new Size(320, 240);
                    videoSource.DesiredFrameRate = 30;
                    
                    videoSource.NewFrame += new NewFrameEventHandler(video_NewFrame);
                   
                    
                    CloseVideoSource();
                   
                    videoSource.Start();
                    
                    
                    label2.Text = "Device running...";
                    start.Text = "&Stop";
                    timer1.Enabled = true;
                   
                    
                }
                else
                {
                    label2.Text = "Error: No Device selected.";
                }
            }
            else
            {
                if (videoSource.IsRunning)
                {
                    timer1.Enabled = false;
                    CloseVideoSource();
                    label2.Text = "Device stopped.";
                    start.Text = "&Start";                    
                }
            }
        }

        

      
        public void  video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap globaly, globalcb, globalcr, yChannel, cbChannel, crChannel;
      
            
            Bitmap img = (Bitmap)eventArgs.Frame.Clone();
           // Bitmap imgsmoke = (Bitmap)eventArgs.Frame.Clone();
            // create filter
            //Erosion filterer = new Erosion();
            // apply the filter
            //filterer.Apply(img);
      

            
            globaly = (Bitmap)eventArgs.Frame.Clone(); //bitmap image to store Y extract

            // create filter
            BrightnessCorrection filterb1 = new BrightnessCorrection(-0.5);
            // apply the filter
            filterb1.ApplyInPlace(globaly);

            globalcb = (Bitmap)eventArgs.Frame.Clone(); //bitmap image to store Cb extract

            // create filter
            BrightnessCorrection filterb2 = new BrightnessCorrection(-0.5);
            // apply the filter
            filterb2.ApplyInPlace(globalcb);

            globalcr = (Bitmap)eventArgs.Frame.Clone(); //bitmap image to store Cr extract
            //globaly.Height = globalcb.Height = globalcr.Height = 25;
            //globaly.Width = globalcb.Width = globalcr.Width = 50;

            
            YCbCrExtractChannel filtery = new YCbCrExtractChannel(YCbCr.YIndex);
            // apply the filter and extract Y channel
            //globaly.Clone();
            yChannel = filtery.Apply(globaly);
           
           
            yChannel.Clone();
            Bitmap y2Channel = (Bitmap)yChannel.Clone();       
            
                  

            YCbCrExtractChannel filtercb = new YCbCrExtractChannel(YCbCr.CbIndex);
            // apply the filter and extract Cb channel
            globalcb.Clone();
            cbChannel = filtercb.Apply(globalcb);
          
            
            cbChannel.Clone();
            Bitmap cb2Channel = (Bitmap)cbChannel.Clone();
            


            YCbCrExtractChannel filtercr = new YCbCrExtractChannel(YCbCr.CrIndex);
            // apply the filter and extract Cr channel
            globalcr.Clone();
            crChannel = filtercr.Apply(globalcr);
            
            crChannel.Clone();
            Bitmap cr2Channel = (Bitmap)crChannel.Clone();
            
           
             
            

            //fire pixel processing
            BitmapData subdata2 = cb2Channel.LockBits(new Rectangle(0, 0, cb2Channel.Width, cb2Channel.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
            BitmapData subdata1 = y2Channel.LockBits(new Rectangle(0, 0, y2Channel.Width, y2Channel.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
           // BitmapData subdata3 = cr2Channel.LockBits(new Rectangle(0, 0, cr2Channel.Width, cr2Channel.Height), ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);
        
            int stride1 = subdata2.Stride;
            int stride0 = subdata1.Stride;
            //int stride2 = subdata3.Stride;
           
            System.IntPtr Scan01 = subdata2.Scan0;
            System.IntPtr Scan02 = subdata1.Scan0;
            //System.IntPtr Scan2 = subdata3.Scan0;


            unsafe
            { 
                byte* q = (byte*)(void*)Scan01;
                byte* p = (byte*)(void*)Scan02;
                
                //byte* r = (byte*)(void*)Scan2;


                int cnt = 0 ;
                //int nOffset1 = stride1 - cbChannel.Width * 3;



                for (int y = 0; y < cbChannel.Height; y++) //scanning pixels horizontally
                {
                    for (int x = 0; x < cbChannel.Width; x++) //scanning pixels vertically
                    {


                        int x1 = int.Parse(p[0].ToString());//y                  
                        int y1 = int.Parse(q[0].ToString());//cb
                        //int z1 = int.Parse(r[0].ToString());//cr
                        int z1 = x1 - y1;

                        if (z1 > 125) //defining condition for difference between y and cb channel for a fire pixel of ligter flame (trial & error)
                        {
                            cnt++;
                        }
                        p += 1;
                        //r +=1;
                        q += 1;
                    }

                    //q += nOffset1;
                }


                if (cnt > 125)
                {



                   
                    //SoundPlayer simpleSound = new SoundPlayer(@"C:\voice.wav");
                    //simpleSound.Play();
                    
                    SoundPlayer simpleSound = new SoundPlayer(@"D:\fire detected_MAIN - Copy\fire detected_MAIN - Copy\Resources\\fewsec.wav");
                    //SoundPlayer simpleSound = new SoundPlayer(@"F:\fire detected_MAIN - Copy\fire detected_MAIN - Copy\\Resources\\");
                    simpleSound.Play();
                    if (cnt < 150)
                    {
                        setLabel1TextSafe("Fire Detected!!! \n Low Intesity");
                    }
                    else if (cnt > 150 && cnt < 300)
                    {
                        setLabel1TextSafe("Fire Detected!!! \n Medium intesity");
                    }
                    else if (cnt > 300 && cnt <= 500)
                    {
                        setLabel1TextSafe("Fire Detected!!! \n High intesity");
                    }



                }
                else
                {
                    setLabel1TextSafe(" ");
                }
               
            cb2Channel.UnlockBits(subdata2);
            y2Channel.UnlockBits(subdata1);
            }
            pictureBox1.Image = img; //picturebox stores cloned image of frame
            pictureBox2.Image = yChannel;
            pictureBox3.Image = cbChannel;
            pictureBox4.Image = crChannel;       


              
       
                         
                                                                                      
        }



        private void setLabel1TextSafe(string txt)
        {
            if (label7.InvokeRequired)
            {
                label7.Invoke(new Action(() => label7.Text = txt));
                return;
            }
            label7.Text = txt;
        }

        public delegate void Action();                   
        private void CloseVideoSource()
        {
            if (!(videoSource == null))
                if (videoSource.IsRunning)
                {
                    videoSource.SignalToStop();
                    videoSource = null;
                }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = "Device running... " + videoSource.FramesReceived.ToString() + " FPS";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseVideoSource();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
            getCamList();
            this.Text = "Fire Detection Through Image Processing";
            label8.Text = DateTime.Now.ToString("HH:mm:ss");
            label9.Text = DateTime.Now.ToString("dd/MMMM/yyyy ,(dddd)");          
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label8.Text = DateTime.Now.ToString("HH:mm:ss");
            label9.Text = DateTime.Now.ToString("dd/MMMM/yyyy ,(dddd)");

            
        }

    

       
    }

  
}
