using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;//Need to include to talk to arduino


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //Font = new Font(Font.Name, 8.25f * 96f / CreateGraphics().DpiX, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont);

            InitializeComponent();//This is a function predefined when form is created
            getAvailablePorts();//function that loads avaliable ports into combo box
            onStartUp();//function that will execute when started up
            
        }
        static SerialPort serialPort1;

    
        void onStartUp()
        {

            try
            {
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
            }
            catch
            {
                MessageBox.Show("Arduino is not connected!\nPlease connect the arduino and select the port in settings :)","!No Arduino!");
                
                comboBox1.Items.Add("EMPTYCOM");
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
            }
            this.Size = new System.Drawing.Size(1920, 1080);


            //https://www.smartgrid.gov/the_smart_grid/smart_home.html //webstie with video on HEMS
        }
        private void button1_Click(object sender, EventArgs e)//this is the test connection button
        {
            progressBar1.Value = 0;
            serialPort1 = new SerialPort();//Allocates a new serial port
            serialPort1.PortName = comboBox1.Text;//grabs the port from the combo box in the settins page
            serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);//this sets the baudrate
            
            if (!serialPort1.IsOpen)//checked to make sure the serial port is open
            {
                try//try isspecial to c#, and is complicated to explain.  please refer to msdn
                {
                    serialPort1.Open();//opens port
                    serialPort1.Write("1");//writes to port
                    serialPort1.Close();//closes [port
                    System.Threading.Thread.Sleep(1000);//like delay(123);
                    serialPort1.Open();
                    serialPort1.Write("0");
                    serialPort1.Close();
                    progressBar1.Value = 100;
                }
                catch// a catch is like an else statement. 
                {
                    MessageBox.Show("There was an error. Please make sure that the correct port was selected, and the device, plugged in.");
                }
            }
            
            
        }
        void getAvailablePorts()
        {
            string[] ports = SerialPort.GetPortNames();//loads Ports into an array and puts it inot combo box
            comboBox1.Items.AddRange(ports);
        }



        private void buttonNavigate_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabNavigate;//changes tab
            moveToRoom("1");
        }
        private void button2_Click_1(object sender, EventArgs e)//refresh button
        {
            getAvailablePorts();//loads combo box
        }
        
        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabLights;//Moves user to Lighitng Tab
        }



        int currentRoom = 1;

        void navigateBack()//function navigates to the previous room the user was in
        {
            switch (currentRoom)
            {
                case 1:
                    tabControl1.SelectedTab = tabLivingRoom;
                    break;
                case 2:
                    
                    tabControl1.SelectedTab = tabBedroom;
                    break;
                case 3:
                    tabControl1.SelectedTab = tabKitchen;
                    break;

                case 4:
                    tabControl1.SelectedTab = tabBathroom;
                    break;



                default:

                    break;

            }
            //tabControl1.SelectedTab = tabNavigate;//changes tab
        }
        
        private void buttonToLivingRoom_Click(object sender, EventArgs e)//Move to living Room
        {
            tabControl1.SelectedTab = tabLivingRoom;//Changes selected tab to living room
            moveToRoom("1");
            currentRoom = 1;

        }
        private void buttonToBedRoom_Click(object sender, EventArgs e)//Move to Bed Room
        {
            tabControl1.SelectedTab = tabBedroom;
            moveToRoom("2");
            currentRoom = 2;
        }


        private void buttonToKitchen_Click(object sender, EventArgs e)//demos Kitchen
        {
            tabControl1.SelectedTab = tabKitchen;
            moveToRoom("3");
            currentRoom = 3;
        }

        private void buttonToBathRoom_Click(object sender, EventArgs e)//Demos Bathroom
        {
            tabControl1.SelectedTab = tabBathroom;
            moveToRoom("4");
            currentRoom = 4;
        }

        private void button7_Click(object sender, EventArgs e)//Allows user to skip to settings tab
        {
            tabControl1.SelectedTab = tabSettings;
        }

        private void label15_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabHome2;//Go back to home from back label
        }

        void moveToRoom(string room)
        {
            try
            {
                serialPort1 = new SerialPort();
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                serialPort1.Open();
                serialPort1.Write(room);
                serialPort1.Close();
            }
            catch { }
        }
        //These are unused functions automatically created by Visual studios.  I will delete them when i figure out how

        bool loopMotionSensor = false;
        
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                string motionVal;
                serialPort1 = new SerialPort();
                serialPort1.PortName = comboBox1.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                loopMotionSensor = true;
                int point = 0;
                int pointd = 0;
                do
                {

                    serialPort1.Open();
                    motionVal = serialPort1.ReadLine();



                    if (motionVal == "1")
                    {
                        richTextBox2.Text = "FOUND MOTION";
                    }
                    else if (motionVal == "0")
                    {
                        richTextBox2.Text = "No motion found...";
                    }







                    Application.DoEvents();
                    //System.Threading.Thread.Sleep(1000);
                    serialPort1.Close();


                    if (checkBox1.Checked)
                    {
                        chart1.Series["Non Dimmable"].Points.AddXY(point, point);
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(100);

                        if (motionVal == "FOUND MOTION!")
                        {
                            pointd++;
                        }
                        chart1.Series["Dimming Lights"].Points.AddXY(point, pointd);
                    }
                    point++;




                } while (loopMotionSensor);
                richTextBox2.Text = "Stopped\n";
                richTextBox2.Text += motionVal;
            }
            catch
            {
                richTextBox2.Text = "ERROR";
            }






        }

        private void button9_Click(object sender, EventArgs e)
        {
            loopMotionSensor = false;
            chart1.Series["Non Dimmable"].Points.Clear();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabAbout;
        }


 
        private void button11_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabLights;//Moves user to Lighitng Tab
        }


        private void button15_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabHEMS;//Moves user to HEMS
            webBrowser1.Navigate("https://www.smartgrid.gov/the_smart_grid/smart_home.html");//Starts video
        }



        private void label17_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabNavigate;//like back button
            webBrowser1.Navigate("google.com");//ends video
        }




        //This is so that the user can click on the pictures as well
        private void pictureBox4_Click(object sender, EventArgs e)
        {
            buttonToKitchen.PerformClick();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            buttonToLivingRoom.PerformClick();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            buttonToBathRoom.PerformClick();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            buttonToBedRoom.PerformClick();
        }



        //***********Wrappers for button navigations****************
        //Navigate back buttons
        private void label14_Click(object sender, EventArgs e)//This will allows the user to navigate back to their previous screen
        {
            navigateBack();
        }

        private void label23_Click(object sender, EventArgs e)
        {
            navigateBack();
        }

        private void label26_Click(object sender, EventArgs e)
        {
            navigateBack();
        }

        private void label20_Click(object sender, EventArgs e)
        {
            navigateBack();
        }

        private void label27_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabHome2;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabMotionSensor;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            //tabControl1.SelectedTab = tabHVAC;
        }

        private void button14_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabTesla;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            //tabControl1.SelectedTab = tabPowerFlush;
        }

        private void button21_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabMotionSensor;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabLights;
        }

        private void button22_Click(object sender, EventArgs e)
        {
            //tabControl1.SelectedTab = tabHVAC;
        }

        private void button23_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabHEMS;
            webBrowser1.Navigate("https://www.smartgrid.gov/the_smart_grid/smart_home.html");//Starts video
        }



        //***************MOVE MOTOR

        private void button27_Click(object sender, EventArgs e)
        {
            moveToRoom("8");
        }

        private void button28_Click(object sender, EventArgs e)
        {

            moveToRoom("7");

        }

        private void button29_Click(object sender, EventArgs e)
        {
            //webBrowser2.Navigate("http://viewpure.com/k_04AdmrzAk?start=0&end=0");
        
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void label30_Click(object sender, EventArgs e)
        {
            
        }
        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }

        private void tabLighting_Click(object sender, EventArgs e)
        {

        }

        private void button25_Click(object sender, EventArgs e)
        {

        }
        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabLights;
        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void button26_Click(object sender, EventArgs e)
        {
            //http://viewpure.com/k_04AdmrzAk?start=0&end=0
        }

        private void label25_Click(object sender, EventArgs e)
        {

        }


        private void tabLivingRoom_Click(object sender, EventArgs e)
        {

        }
        private void button15_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabHEMS;
            webBrowser1.Navigate("https://www.smartgrid.gov/the_smart_grid/smart_home.html");//Starts video
        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {

        }

        private void label38_Click(object sender, EventArgs e)
        {
            navigateBack();
            loopMotionSensor = false;

        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {

        }

        private void label42_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabNavigate;
        }

        private void panel18_Paint(object sender, PaintEventArgs e)
        {
            navigateBack();
        }

        private void label43_Click(object sender, EventArgs e)
        {

        }

        private void label44_Click(object sender, EventArgs e)
        {
            navigateBack();
        }

        private void pictureBox10_Click(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabLights;
        }

        private void tabKitchen_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void button30_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabLights;
        }

        private void label47_Click(object sender, EventArgs e)
        {
            navigateBack();
        }

        private void button25_Click_1(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }
    }
}
