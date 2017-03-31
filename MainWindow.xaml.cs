//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp; //C#에서 opencv 사용하기 위해 opencvsharp를 사용함. emgu.cv도 있음
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading;


namespace Microsoft.Samples.Kinect.DepthBasics
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Microsoft.Kinect;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Windows.Controls;
    using System.Windows.Shapes;
    using System.Net.Sockets;
    using System.Xml;




    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        /// <summary>
        /// Map depth range to byte range
        /// </summary>

        private const int MapDepthToByte = 3000 / 256;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Reader for depth frames
        /// </summary>
        private DepthFrameReader depthFrameReader = null;

        /// <summary>
        /// Description of the data contained in the depth frame
        /// </summary>
        private FrameDescription depthFrameDescription = null;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap depthBitmap = null;
        private IplImage cvImage = null;
        /// <summary>
        /// Intermediate storage for frame data converted to color
        /// </summary>
        //private byte[] depthPixels = null;

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;


        private Boolean mFindingCenterFlag = true;

        private Boolean mkinectFloorMode = true;

        private static int mMinPixel = 0;
        private static int mMaxPixel = 255;

        private static ushort mMinDepth = 0;
        private static ushort mMaxDepth = 1837;

        private static int mPaddingRightWidth = 0;
        private static int mPaddingLeftWidth = 0;
        private static int mPaddingTopHeight = 0;
        private static int mPaddingBottomtHeight = 0;

        private static int mStartX = 0;
        private static int mStartY = 0;

        private static int mUnityWidth = 0;
        private static int mUnityHeight = 0;

        private static int mKinectDepthStreamHeight = 424;
        private static int mKinectDepthStreamWidth = 512;

        private Ellipse[] ellipses = new Ellipse[10];

        private double windowWidth = 0;
        private double windowHeight = 0;

        private double screenWidth = 0;
        private double screenHeight = 0;

        private static int mWallStartY = 250;
        private static double mWallScale = 0;


        private Boolean reverseFlag = false;
        private static HttpClient httpClient = HttpClient.getInstance();


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // get the kinectSensor object
            this.kinectSensor = KinectSensor.GetDefault();

            // open the reader for the depth frames
            this.depthFrameReader = this.kinectSensor.DepthFrameSource.OpenReader();

            // wire handler for frame arrival
            this.depthFrameReader.FrameArrived += this.Reader_FrameArrived;

            // get FrameDescription from DepthFrameSource
            this.depthFrameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            // allocate space to put the pixels being received and converted
            //this.depthPixels = new byte[this.depthFrameDescription.Width * this.depthFrameDescription.Height];

            // create the bitmap to display //최종적으로 화면에 그릴 비트맵을 생성한다.
            this.depthBitmap = new WriteableBitmap(this.depthFrameDescription.Width, this.depthFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray8, null);

            // bytearray를 받기 위한 IpIImage를 생성한다.
            this.cvImage = new IplImage(new CvSize(this.depthFrameDescription.Width, this.depthFrameDescription.Height), BitDepth.U8, 1);

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;

            // open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();

            //기본적으로 키넥트상 좌표 위치에 표시할 원을 생성한다.
            for (int i = 0; i < ellipses.Length; i++)
            {
                ellipses[i] = CreateAnEllipse(20, 20);
                LayoutRoot.Children.Add(ellipses[i]);
            }

            //초기 이전 설정값을 불러서 세팅한다.
            ReadXML();
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get
            {
                //실제 화면에 그려주는 부분
                // depthBitmap 의 bitmap를 화면에 그려준다.
                return this.depthBitmap;
            }
        }

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get
            {
                return this.statusText;
            }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                    }
                }
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.depthFrameReader != null)
            {
                // DepthFrameReader is IDisposable
                this.depthFrameReader.Dispose();
                this.depthFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }



        /// <summary>
        /// Handles the depth frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        /// 센서로부터 영상심도프레임을 전달받는 부분.
        private void Reader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            bool depthFrameProcessed = false;

            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    // the fastest way to process the body index data is to directly access 
                    // the underlying buffer
                    using (Microsoft.Kinect.KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                    {
                        // verify data and write the color data to the display bitmap
                        if (depthBitmap != null)
                            if (((this.depthFrameDescription.Width * this.depthFrameDescription.Height) == (depthBuffer.Size / this.depthFrameDescription.BytesPerPixel)) &&
                                (this.depthFrameDescription.Width == this.depthBitmap.PixelWidth) && (this.depthFrameDescription.Height == this.depthBitmap.PixelHeight))
                            {
                                // Note: In order to see the full range of depth (including the less reliable far field depth)
                                // we are setting maxDepth to the extreme potential depth threshold

                                ushort maxDepth = ushort.MaxValue;

                                // If you wish to filter by reliable depth distance, uncomment the following line:
                                // maxDepth = depthFrame.DepthMaxReliableDistance

                                //1.6m ~ 1.837m 에 심도값만 센서로부터 받은 프레임을 전달한다.
                                if (mkinectFloorMode)
                                {
                                    this.ProcessDepthFrameData(depthBuffer.UnderlyingBuffer, depthBuffer.Size, getMinDepth(), getMaxDepth());
                                    depthFrameProcessed = true;
                                }
                                else
                                {
                                    this.ProcessDepthFrameDataWall(depthBuffer.UnderlyingBuffer, depthBuffer.Size, (ushort)mWallStartY, getMinDepth(), getMaxDepth());
                                    depthFrameProcessed = true;
                                }

                            }


                    }
                }
            }

            if (depthFrameProcessed)
            {
                //  비트맵을 랜더링시킨다.
                this.RenderDepthPixels();
            }
        }

        /// <summary>
        /// Directly accesses the underlying image buffer of the DepthFrame to 
        /// create a displayable bitmap.
        /// This function requires the /unsafe compiler option as we make use of direct
        /// access to the native memory pointed to by the depthFrameData pointer.
        /// </summary>
        /// <param name="depthFrameData">Pointer to the DepthFrame image data</param>
        /// <param name="depthFrameDataSize">Size of the DepthFrame image data</param>
        /// <param name="minDepth">The minimum reliable depth value for the frame</param>
        /// <param name="maxDepth">The maximum reliable depth value for the frame</param>
        /// 센서로 부터 바이트어레이를 받아서 cvImage ImageData에 넣는다.
        private unsafe void ProcessDepthFrameData(IntPtr depthFrameData, uint depthFrameDataSize, ushort minDepth, ushort maxDepth)
        {
            // depth frame data is a 16 bit value 
            // 1행으로 ~쭉 해당 bit를 가져온다.
            ushort* frameData = (ushort*)depthFrameData;

            // convert depth to a visual representation
            int value = mPaddingTopHeight;
            int startVal = this.depthFrameDescription.Width * value;
            int endVal = this.depthFrameDescription.Width * (value + (mKinectDepthStreamHeight - value - mPaddingBottomtHeight));

            //Kinect v2 Depth stream 512 x 424
            //for (int i = 0; i < (int)(depthFrameDataSize / this.depthFrameDescription.BytesPerPixel); ++i)
            //for (int i = startVal; i < endVal; i++)
            for (int i = startVal + mPaddingLeftWidth; i < endVal; i += mKinectDepthStreamWidth)
            {
                ushort depth;
                for (int j = 1; j < (mKinectDepthStreamWidth - mPaddingLeftWidth) - mPaddingRightWidth; j++)
                {

                    depth = frameData[i + j];
                    Marshal.WriteInt32(cvImage.ImageData, i + j, depth >= minDepth && depth <= maxDepth ? mMaxPixel : 0);

                }

            }
        }



        // Wall Mode kinect
        private unsafe void ProcessDepthFrameDataWall(IntPtr depthFrameData, uint depthFrameDataSize, ushort mmvalue, ushort minDepth, ushort maxDepth)
        {
            // depth frame data is a 16 bit value
            ushort* frameData = (ushort*)depthFrameData;

            //벽면모드에서 실제로 그려줄 부분의 시작과 끝
            //  int startVal = this.depthFrameDescription.Width * value;
            //  int endVal = this.depthFrameDescription.Width * (value + 1);


            int value = mmvalue;
            int startVal = this.depthFrameDescription.Width * value;
            int endVal = this.depthFrameDescription.Width * (value + 1);

            ushort startDepth = 0;
            ushort lastDepth = 0;
            int startX = 0;
            double lastX = 0;
            Boolean startFlag = false;
            ArrayList pointList = new ArrayList();

            for (int i = startVal; i < endVal; i++)
            {
                ushort depth = frameData[i];

                if (depth != 0 && depth <= mMaxDepth)
                {
                    //최초 유효값이 들어온 경우만 이 조건을 만족함.
                    if (!startFlag)
                    {   //유효한 값이 최초로 인식될 때.
                        startX = i - startVal;
                        startDepth = frameData[i];
                        startFlag = !startFlag;

                    }

                }
                else if (startFlag)
                {
                    //무효값이 들어올 때 저장해논 최초유효값 좌표를 참고해 중앙값을 계산 후 좌표리스트에 저장 (PointList)
                    lastX = (startX + (i - startVal)) / 2;

                    
                    //  Console.WriteLine("R TEST lastX = " + lastX + " , " + "startDepth = " + startDepth);
                    lastX = lastX * ((double)startDepth / minDepth) - (mKinectDepthStreamWidth * ((double)startDepth / minDepth) - mKinectDepthStreamWidth) / 2; //HS8 offset 한수 알고리즘
                    Console.WriteLine("R TEST AFTER lastX = " + lastX + " , " + "startDepth = " + startDepth);


                    if (pointList.Count > 0)
                    {
                        for (int num = 0; num < pointList.Count; num++)
                        {
                            double X = ((Point)pointList[num]).X;
                            if (X - 40 <= lastX && lastX <= X + 40)
                            {
                                startFlag = !startFlag;
                            }
                            else
                            {

                                pointList.Add(new Point(lastX, (int)(startDepth) - minDepth));
                                startFlag = !startFlag;
                            }

                        }
                    }
                    else
                    {

                        pointList.Add(new Point(lastX, (int)(startDepth) - minDepth));
                        startFlag = !startFlag;
                    }




                }

                lastDepth = depth;
            }


            int cllipse_index = 0;

            //if (pointList.Count > 0)
            {
                for (int i = 0; i < pointList.Count; i++)
                {
                    if (ellipses.Length > i)
                    {
                        //reverse를 체크하지 않은 경우.
                        if (reverseFlag)
                        {

                            Console.Write("X = " + ((Point)pointList[i]).X + " , Y = " + ((Point)pointList[i]).Y);

                            double x = (double)(1 - ((Point)pointList[i]).X / mKinectDepthStreamWidth);
                            double y = (double)(1 - ((Point)pointList[i]).Y / (mMaxDepth - minDepth));


                            Canvas.SetLeft(ellipses[i], screenWidth * x);
                            Canvas.SetTop(ellipses[i], screenHeight * y);

                        x = (x + (((double)mKinectDepthStreamWidth * mWallScale) / 512f)) / (1 + ((((double)mKinectDepthStreamWidth * mWallScale) * 2f) / 512f));
  

                        //    x = (x + (145f / 512f)) / (1 + ((145f * 2f) / 512f));

                            //  Console.WriteLine("TEST : " + screenWidth * x +" , "+ screenWidth * y);
                            //Unity로 실시간 좌표 전송하기
                            if (unityConnectSuccess)
                                if (0 < x)
                                    Update((double)x, (double)y);

                        }
                        else
                        {

                            Console.Write(((Point)pointList[i]).X + "," + ((Point)pointList[i]).Y + " ");

                            double x = (double)(((Point)pointList[i]).X / mKinectDepthStreamWidth);
                            double y = (double)(((Point)pointList[i]).Y / (mMaxDepth - minDepth));

                            x = (x + (((double)mKinectDepthStreamWidth * mWallScale) / 512f)) / (1 + ((((double)mKinectDepthStreamWidth * mWallScale) * 2f) / 512f));
                        //    x = (x + (145f / 512f)) / (1 + ((145f * 2f) / 512f));


                            Canvas.SetLeft(ellipses[i], screenWidth * x);
                            Canvas.SetTop(ellipses[i], screenHeight * y);

                            //   Console.WriteLine("TEST : " + screenWidth * x + " , " + screenWidth * y);

                            //Unity로 실시간 좌표 전송하기
                            if (unityConnectSuccess)
                                if(0 < x)
                                    Update((double)x , (double)y);


                        }

                    }
                }
                //그려주지 않고 있는 잉여 원은 다른 위치로 보낸다.
                for (int i = pointList.Count; i < ellipses.Length; i++)
                {

                    Canvas.SetLeft(ellipses[i], -100);
                    Canvas.SetTop(ellipses[i], -100);

                }

            }

            if (pointList.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine();
            }



        }


        /// <summary>
        /// Renders color pixels into the writeableBitmap.
        /// </summary>
        private void RenderDepthPixels()
        {
            //mFindingCenterFlag 가 true이면 무게중심을 찾는 알고리즘을 실행시킨다.
            if (mFindingCenterFlag)
                findCenterContourImage(cvImage, 150);

            // cvImage 이미지 데이터를 기반으로 실제 비트맵을 랜더링한다.
            this.depthBitmap.WritePixels(
                new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                cvImage.ImageData,
                this.depthBitmap.PixelWidth * this.depthBitmap.PixelHeight,
                this.depthBitmap.PixelWidth);

        }

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).ㅋ
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }




        //물체 중심점 찾기 알고리즘
        private void findCenterContourImage(IplImage image, int minArea)
        {

            CvSeq<CvPoint> contoursRaw;

            ArrayList pointList = new ArrayList();

            using (CvMemStorage storage = new CvMemStorage())
            {
                //find contours 물체의 윤곽선을 추출한다.
                Cv.FindContours(image, storage, out contoursRaw, CvContour.SizeOf, ContourRetrieval.Tree, ContourChain.ApproxSimple);

                //Taken straight from one of the OpenCvSharp samples
                using (CvContourScanner scanner = new CvContourScanner(image, storage, CvContour.SizeOf, ContourRetrieval.Tree, ContourChain.ApproxSimple))
                {
                    foreach (CvSeq<CvPoint> c in scanner)
                    {

                        CvMoments mmt;
                        Cv.Moments(c, out mmt, true);//무게중심을 찾는 함수.

                        double d00, d10, d01;
                        double posX, PosY;
                        d00 = Cv.GetSpatialMoment(mmt, 0, 0);
                        d10 = Cv.GetSpatialMoment(mmt, 1, 0);
                        d01 = Cv.GetSpatialMoment(mmt, 0, 1);
                        posX = d10 / d00;
                        PosY = d01 / d00;

                        CvPoint cp = Cv.Point(Cv.Round(posX), Cv.Round(PosY));

                        //Some contours are negative so make them all positive for easy comparison
                        double area = Math.Abs(c.ContourArea());
                        //Uncomment below to see the area of each contour
                        //  Console.WriteLine(area.ToString());
                        if (area >= minArea)
                        {
                            pointList.Add(cp);

                        }
                    }

                    // Scanner 안에서 육관선과 중심점을 Read하면서 동시에 write할 수 없기에 밖에서 데이터를 그린다.
                    foreach (CvPoint p in pointList)
                    {

                        // Console.WriteLine("중심좌표 X : "+p.X + " 중심좌표 Y :" + p.Y);
                        // 무게중심을 화면에 그린다.

                        image.Circle(p, 4, CvColor.White);

                        //Unity로 실시간 좌표 전송하기
                        //unityConnectSuccess , true
                        if (true)
                        {
                            if (!reverseFlag)
                            {
                                //  Console.WriteLine("중심좌표 X : " + x + " 중심좌표 Y :" + y); 
                                double x = (double)p.X * ((double)mUnityWidth / (double)mKinectDepthStreamWidth);
                                double y = (double)p.Y * ((double)mUnityHeight / (double)mKinectDepthStreamHeight);

                                Update(x, y);

                            }
                            else
                            {


                            }

                        }
                        //   new CvWindow("circle image", image);

                    }
                }
            }

        }

        /* * * * * * * * * * * * * * * * * * *   Kinect Mode 설정  * * * * * * * * * * * * * * * * * * * * */
        //Floor모드 기본 디폴트 모드
        private void floor_radioButton_Checked(object sender, RoutedEventArgs e)
        {
            mkinectFloorMode = true;
        }

        //Wall모드 벽면모드
        private void wall_radioButton_Checked(object sender, RoutedEventArgs e)
        {

            mkinectFloorMode = false;
            mFindingCenterFlag = false;
            clearWhiteBitmap();

        }

     

        /* * * * * * * * * * * * * * * * * * *   심도설정  * * * * * * * * * * * * * * * * * * * * */
        //MinDepth 설정
        private void minDepth_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox MinDepth_textBox = (TextBox)sender;
            Console.WriteLine("MinDepth = " + MinDepth_textBox);
            if (!minDepth_textBox.Text.Equals(""))
            {
                try
                {
                    ushort minDepthValue = Convert.ToUInt16(MinDepth_textBox.Text);
                    if (minDepthValue >= 0)
                        mMinDepth = minDepthValue;

                }
                catch
                {
                    Console.WriteLine("값이 유효하지 않습니다.");

                }

            }
        }

        //MaxDepth 설정
        private void maxDepth_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox MaxDepth_textBox = (TextBox)sender;
            Console.WriteLine("MaxDepth = " + MaxDepth_textBox);
            if (!maxDepth_textBox.Text.Equals(""))
            {
                try
                {

                    ushort maxDepthValue = Convert.ToUInt16(MaxDepth_textBox.Text);
                    if (maxDepthValue >= 0)
                        mMaxDepth = maxDepthValue;


                }
                catch
                {
                    Console.WriteLine("값이 유효하지 않습니다.");

                }
            }


        }

        private ushort getMinDepth()
        {
            return mMinDepth;
        }

        private ushort getMaxDepth()
        {
            return mMaxDepth;
        }




        /* * * * * * * * * * * * * * * * *  중심점 실행시작,실행중단  * * * * * * * * * * * * * * * * * * * * */
        //findCenterContourImage 시작
        private void findCenter_button_Click(object sender, RoutedEventArgs e)
        {
            mFindingCenterFlag = true;
            //벽모드인데 중심점을 찾으려고 하는경우 강제로 중심점 못차게 하기.
            //if(!mkinectFloorMode) mFindingCenterFlag = false;

        }

        //findCenterContourImage 중지
        private void stopCenter_button_Click(object sender, RoutedEventArgs e)
        {
            mFindingCenterFlag = false;
        }

        //벽면모드에서 그릴 원에 대한 크기와 디자인 설정
        public Ellipse CreateAnEllipse(int height, int width)
        {
            SolidColorBrush fillBrush = new SolidColorBrush() { Color = Colors.Red };
            SolidColorBrush borderBrush = new SolidColorBrush() { Color = Colors.Black };

            return new Ellipse()
            {
                Height = height,
                Width = width,
                StrokeThickness = 1,
                Stroke = borderBrush,
                Fill = fillBrush
            };
        }

        //윈도우 창 크기가 변할 때 이벤트
        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //실제 윈도우 창 해상도와 디스플레이 화면 해상도를 구한다.
            windowWidth = this.Width;
            windowHeight = this.Height;
            Console.WriteLine("MainWindow_SizeChanged" + windowWidth + " , " + windowHeight);

            screenWidth = SystemParameters.PrimaryScreenWidth;
            screenHeight = SystemParameters.PrimaryScreenHeight;
            Console.WriteLine("screenWidth" + screenWidth + " , " + screenHeight);


        }

        //소켓 연결 이벤트
        private void connectUnity_button_Click(object sender, RoutedEventArgs e)
        {
            Awake();
        }
        //소켓 연결끊기 이벤트
        private void stopUnity_button_Click(object sender, RoutedEventArgs e)
        {
            OnApplicationQuit();
        }

        //좌표 상하반전 이벤트
        private void reversePoint_checkBox_Checked(object sender, RoutedEventArgs e)
        {
            reverseFlag = !reverseFlag;
        }


        //Kinect 설정 저장
        private void saveConfig_button_Click(object sender, RoutedEventArgs e)
        {

            //     httpClient.sendXMLToServer(SERVER_IP + SERVER_PORT);

            CreateXML();
        }
        //kinect 설정 불러오기
        private void readConfig_button_Click(object sender, RoutedEventArgs e)
        {

            /*   XmlDocument doc = httpClient.loadXMLFromServer(SERVER_IP + SERVER_PORT);
                if (doc != null)
                {
                    doc.Save("./config.xml");
                }
             */

            ReadXML();
        }
        //Textbox util 
        private int convertTextBoxValue(TextBox sender)
        {
            TextBox rightPaddingWidth_textBox = (TextBox)sender;
            if (!rightPaddingWidth_textBox.Text.Equals(""))
            {
                try
                {
                    ushort paddingRightWidth = Convert.ToUInt16(rightPaddingWidth_textBox.Text);
                    if (paddingRightWidth >= 0)
                        return paddingRightWidth;
                }
                catch
                {
                    Console.WriteLine("값이 유효하지 않습니다.");
                }
            }
            return -1;
        }

        //clear bitmap
        private void clearBitmap()
        {
            int size = cvImage.Size.Height * cvImage.Size.Width;
            for (int i = 0; i < size; ++i)
            {
                Marshal.WriteInt32(cvImage.ImageData, i, 0);
            }
        }
        //clear bitmap
        private void clearWhiteBitmap()
        {
            int size = cvImage.Size.Height * cvImage.Size.Width;
            for (int i = 0; i < size; ++i)
            {
                Marshal.WriteInt32(cvImage.ImageData, i, 255);
            }
        }

        private void right_padding_width_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue >= 0)
            {
                mPaddingRightWidth = convertValue;
                Console.WriteLine("mPaddingRightWidth = " + mPaddingRightWidth);
                clearBitmap();
            }
        }

        private void left_padding_width_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue >= 0)
            {
                mPaddingLeftWidth = convertValue;
                Console.WriteLine("mPaddingLeftWidth = " + mPaddingLeftWidth);
                clearBitmap();
            }

        }

        private void top_padding_height_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue >= 0)
            {
                mPaddingTopHeight = convertValue;
                Console.WriteLine("mPaddingTopWidth = " + mPaddingTopHeight);
                clearBitmap();
            }
        }

        private void bottom_padding_height_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue >= 0)
            {
                mPaddingBottomtHeight = convertValue;
                Console.WriteLine("mPaddingBottomtWidth = " + mPaddingBottomtHeight);
                clearBitmap();
            }
        }

        private void startX_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue >= 0)
            {
                mStartX = convertValue;
                Console.WriteLine("mStartX = " + mStartX);

            }
        }

        private void startY_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue >= 0)
            {
                mStartY = convertValue;
                Console.WriteLine("mStartY = " + mStartY);

            }
        }

        private void unity_width_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue >= 0)
            {
                mUnityWidth = convertValue;
                Console.WriteLine("mUnityWidth = " + mUnityWidth);

            }
        }

        private void unity_height_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue >= 0)
            {
                mUnityHeight = convertValue;
                Console.WriteLine("mUnityHeight = " + mUnityHeight);

            }
        }

        //벽면모드 1px 감지 벽과의 거리 조절
        private void wall_startY_textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue >= 0 && convertValue < 512)
            {
                mWallStartY = convertValue;
                Console.WriteLine("mWallStartY = " + mWallStartY);

            }
            else
            {
                Console.WriteLine("=====유효하지 않는 값입니다. defualt 250 할당 ==== ");
                mWallStartY = 250;
            }
        }

      

        private void wall_scale_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            Console.WriteLine("mWallScale = " + (double)slider.Value);
            mWallScale = (double)slider.Value;
 
            wall_scale_value_label.Content = mWallScale.ToString("N4");

        }


        //xml을 생성하고 설정값을 저장한다.
        private void CreateXML()
        {
            try
            {

                // 생성할 XML 파일 경로와 이름, 인코딩 방식을 설정합니다.         
                XmlTextWriter textWriter = new XmlTextWriter(@"./config.xml", Encoding.UTF8);

                // 들여쓰기 설정
                textWriter.Formatting = Formatting.Indented;

                // 문서에 쓰기를 시작합니다.
                textWriter.WriteStartDocument();

                // 루트 설정
                textWriter.WriteStartElement("root");

                // 노드 안에 하위 노드 설정
                textWriter.WriteStartElement("mode");

                textWriter.WriteStartElement("floor");
                textWriter.WriteString(Convert.ToString(mkinectFloorMode));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("wall");
                textWriter.WriteString(Convert.ToString(!mkinectFloorMode));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("reverse");
                textWriter.WriteString(Convert.ToString(reverseFlag));
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("depth");

                textWriter.WriteStartElement("minDepth");
                textWriter.WriteString(Convert.ToString(getMinDepth()));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("maxDepth");
                textWriter.WriteString(Convert.ToString(getMaxDepth()));
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("unityScreen");

                textWriter.WriteStartElement("width");
                textWriter.WriteString(Convert.ToString(mUnityWidth));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("height");
                textWriter.WriteString(Convert.ToString(mUnityHeight));
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("unitIpAddress");

                textWriter.WriteStartElement("ip");
                textWriter.WriteString(Convert.ToString(iPAdress));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("port");
                textWriter.WriteString(Convert.ToString(kPort));
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();


                textWriter.WriteStartElement("floorPaddingDetect");

                textWriter.WriteStartElement("left");
                textWriter.WriteString(Convert.ToString(mPaddingLeftWidth));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("right");
                textWriter.WriteString(Convert.ToString(mPaddingRightWidth));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("top");
                textWriter.WriteString(Convert.ToString(mPaddingTopHeight));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("bottom");
                textWriter.WriteString(Convert.ToString(mPaddingBottomtHeight));
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("startXY");

                textWriter.WriteStartElement("startX");
                textWriter.WriteString(Convert.ToString(mStartX));
                textWriter.WriteEndElement();

                textWriter.WriteStartElement("startY");
                textWriter.WriteString(Convert.ToString(mStartY));
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();

                textWriter.WriteStartElement("wallDetection");
                textWriter.WriteStartElement("wallY");
                textWriter.WriteString(Convert.ToString(mWallStartY));
                textWriter.WriteEndElement();
                textWriter.WriteStartElement("wallScale");
                textWriter.WriteString(Convert.ToString(mWallScale));
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();


                textWriter.WriteStartElement("time");
                textWriter.WriteStartElement("savedTime");
                textWriter.WriteString(Convert.ToString(System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
                textWriter.WriteEndElement();

                textWriter.WriteEndElement();

                textWriter.WriteEndDocument();
                textWriter.Close();

                String currentPath = Environment.CurrentDirectory;
                Console.WriteLine("XML file saved in local " + currentPath);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //지정된 경로의 설정 xml을 불러와 설정값을 세팅한다.
        private void ReadXML()
        {
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(@"./config.xml");
                XmlElement root = xmldoc.DocumentElement;

                // 노드 요소들
                XmlNodeList nodes = root.ChildNodes;

                // 노드 요소의 값을 읽어 옵니다.
                foreach (XmlNode node in nodes)
                {
                    switch (node.Name)
                    {
                        case "mode":
                            mkinectFloorMode = Convert.ToBoolean(node["floor"].InnerText);
                            floor_radioButton.SetCurrentValue(CheckBox.IsCheckedProperty, mkinectFloorMode);
                            wall_radioButton.SetCurrentValue(CheckBox.IsCheckedProperty, !mkinectFloorMode);

                            reverseFlag = Convert.ToBoolean(node["reverse"].InnerText);
                            reversePoint_checkBox.SetCurrentValue(CheckBox.IsCheckedProperty, reverseFlag);
                            break;


                        case "depth":
                            mMinDepth = Convert.ToUInt16(node["minDepth"].InnerText);
                            minDepth_textBox.Text = Convert.ToString(mMinDepth);
                            mMaxDepth = Convert.ToUInt16(node["maxDepth"].InnerText);
                            maxDepth_textBox.Text = Convert.ToString(mMaxDepth);
                            break;


                        case "unityScreen":
                            mUnityWidth = Convert.ToInt16(node["width"].InnerText);
                            unity_width_textBox.Text = Convert.ToString(mUnityWidth);
                            mUnityHeight = Convert.ToInt16(node["height"].InnerText);
                            unity_height_textBox.Text = Convert.ToString(mUnityHeight);
                            break;


                        case "unitIpAddress":
                            iPAdress = Convert.ToString(node["ip"].InnerText);
                            String[] SERVER_OCTET = iPAdress.Split('.');
                            int i = 0;
                            foreach (string s in SERVER_OCTET)
                            {
                                UNITY_SERVER_OCTET[i] = Convert.ToInt16(s);
                                i++;
                            }
                            unity_IPaddress_Octet_01.Text = UNITY_SERVER_OCTET[0].ToString();
                            unity_IPaddress_Octet_02.Text = UNITY_SERVER_OCTET[1].ToString();
                            unity_IPaddress_Octet_03.Text = UNITY_SERVER_OCTET[2].ToString();
                            unity_IPaddress_Octet_04.Text = UNITY_SERVER_OCTET[3].ToString();


                            kPort = Convert.ToInt16(node["port"].InnerText);
                            unity_Port_Number.Text = kPort.ToString();

                            break;


                        case "floorPaddingDetect":
                            mPaddingLeftWidth = Convert.ToInt16(node["left"].InnerText);
                            left_padding_width_textBox.Text = Convert.ToString(mPaddingLeftWidth);
                            mPaddingRightWidth = Convert.ToInt16(node["right"].InnerText);
                            right_padding_width_textBox.Text = Convert.ToString(mPaddingRightWidth);
                            mPaddingTopHeight = Convert.ToInt16(node["top"].InnerText);
                            top_padding_height_textBox.Text = Convert.ToString(mPaddingTopHeight);
                            mPaddingBottomtHeight = Convert.ToInt16(node["bottom"].InnerText);
                            bottom_padding_height_textBox.Text = Convert.ToString(mPaddingBottomtHeight);
                            break;

                        case "startXY":
                            mStartX = Convert.ToInt16(node["startX"].InnerText);
                            startX_textBox.Text = Convert.ToString(mStartX);
                            mStartY = Convert.ToInt16(node["startY"].InnerText);
                            startY_textBox.Text = Convert.ToString(mStartY);
                            break;


                        
                        case "wallDetection":
                            mWallStartY = Convert.ToInt16(node["wallY"].InnerText);
                            wall_startY_textBox.Text = Convert.ToString(mWallStartY);
                            mWallScale = Convert.ToDouble(node["wallScale"].InnerText);
                            wall_scale_slider.Value = mWallScale;
                            break;

                    }

                }

                Console.WriteLine("Kinect configuration successfully loaded XML file from local location ~ !!!");
            }
            catch (IOException ex)
            {
                Console.WriteLine("------------ReadXML err-----------");
            }
        }






        /******************* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **************************/
        /******************* * * * * * * * * * * * * * * * * * * 소켓 통신 모듈 * * * * * * * * * * * * * * * * * *************************/
        /******************* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * **************************/

        private Socket m_Socket;

        public static string iPAdress = "192.168.1.100";
        public static int kPort = 3000;

        private int SenddataLength;                     // Send Data Length. (byte)
        private int ReceivedataLength;                     // Receive Data Length. (byte)

        private byte[] Sendbyte;                        // Data encoding to send. ( to Change bytes)
        private byte[] Receivebyte = new byte[2000];    // Receive data by this array to save.
        private string ReceiveString;                     // Receive bytes to Change string. 

        private Boolean unityConnectSuccess = false;

        public static int[] UNITY_SERVER_OCTET = new int[4];
        public static string UNITY_SERVER_PORT;


        void Awake()
        {
            //=======================================================
            // Socket create.
            m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
            m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);

            //=======================================================
            // Socket connect.
            try
            {
                IPAddress ipAddr = System.Net.IPAddress.Parse(iPAdress);
                IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddr, kPort);
                m_Socket.Connect(ipEndPoint);


            }
            catch (SocketException SCE)
            {
                Console.WriteLine("************************************ Socket connect error! : " + SCE + " ************************************");
                // Debug.Log("Socket connect error! : " + SCE.ToString());
                unity_connect_status.Content = "Error";
                return;
            }
            String currentTime = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            Console.WriteLine("************************************ Socket connect success !! Time : " + currentTime + " ************************************");
            unity_connect_status.Content = "Success";
            unityConnectSuccess = !unityConnectSuccess;
        }

        void Update(double X, double Y)
        {
            //실시간으로 데이터를 보낸다. E를 보내는 이유 -> 좌표하나임을 구분시켜주기 위해
            String vectorData = X + mStartX + "," + Y + mStartY + "E";
              Console.WriteLine("sending point :" + vectorData);
            SendLocation(vectorData);

        }

        void OnApplicationQuit()
        {
            if (m_Socket != null)
            {
                String currentTime = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                m_Socket.Close();
                m_Socket = null;
                Console.WriteLine("************************************ Socket disconnect success !! Time : " + currentTime + " ************************************");
                unity_connect_status.Content = "Disconnect";
                unityConnectSuccess = !unityConnectSuccess;
            }

        }

        void SendLocation(String vectorData)
        {
            //=======================================================
            // Send data write.
            StringBuilder sb = new StringBuilder(vectorData); // String Builder Create

            try
            {
                //=======================================================
                // Send.
                if (m_Socket != null)
                {

                    SenddataLength = Encoding.Default.GetByteCount(sb.ToString());
                    Sendbyte = Encoding.Default.GetBytes(sb.ToString());
                    m_Socket.Send(Sendbyte, Sendbyte.Length, 0);

                }

            }
            catch (SocketException err)
            {

                Console.WriteLine("************************************ Socket send or receive error! : " + err + "************************************");
                OnApplicationQuit();
                // Debug.Log("Socket send or receive error! : " + err.ToString());
            }
        }

        private void unity_IPaddress_Octet_01_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue > 0)
            {
                UNITY_SERVER_OCTET[0] = convertValue;
                Console.WriteLine("UNITY_SERVER_OCTET[0] = " + UNITY_SERVER_OCTET[0]);
            }
        }

        private void unity_IPaddress_Octet_02_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue > 0)
            {
                UNITY_SERVER_OCTET[1] = convertValue;
                Console.WriteLine("UNITY_SERVER_OCTET[1] = " + UNITY_SERVER_OCTET[1]);
            }
        }

        private void unity_IPaddress_Octet_03_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue > 0)
            {
                UNITY_SERVER_OCTET[2] = convertValue;
                Console.WriteLine("UNITY_SERVER_OCTET[2] = " + UNITY_SERVER_OCTET[2]);
            }
        }

        private void unity_IPaddress_Octet_04_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue > 0)
            {
                UNITY_SERVER_OCTET[3] = convertValue;
                Console.WriteLine("UNITY_SERVER_OCTET[3] = " + UNITY_SERVER_OCTET[3]);
            }
        }

        private void unity_Port_Number_TextChanged(object sender, TextChangedEventArgs e)
        {
            int convertValue = convertTextBoxValue((TextBox)sender);
            if (convertValue > 0)
            {
                //UNITY_SERVER_PORT = convertValue.ToString();
                kPort = convertValue;
                Console.WriteLine("UNITY_SERVER_PORT = " + UNITY_SERVER_PORT);
            }
        }

        private void unity_Server_IP_save_button_Click(object sender, RoutedEventArgs e)
        {
            iPAdress = UNITY_SERVER_OCTET[0].ToString() + "." + UNITY_SERVER_OCTET[1].ToString() + "." + UNITY_SERVER_OCTET[2].ToString() + "." + UNITY_SERVER_OCTET[3].ToString();

            Console.WriteLine("Setting UNITY_SERVER_IP = " + iPAdress + " : " + kPort.ToString());
        }

      
    }



}
