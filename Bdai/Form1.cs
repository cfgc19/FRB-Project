using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bdai
{
    public partial class form1 : Form
    {
        Socket client_robot_socket;
        Socket client_robot_socket1;
        int system_state_on_flag;
        string[] parameters;
        string[] parameters1;
        int motor_is_on;
        int program_is_running;
        int robot_availability;
        int num_robo = 0;
        int num_decision = 0;
        int num_decision1 = 0;


        public form1()
        {
            InitializeComponent();
        }

        private void button_1robot_Click(object sender, EventArgs e)
        {
            ip1.Visible = true;
            port1.Visible = true;
            textBox_ip1.Visible = true;
            textBox_port1.Visible = true;
            ip2.Visible = false;
            port2.Visible = false;
            textBox_ip2.Visible = false;
            textBox_port2.Visible = false;
            go_home.Visible = false;
            executar_tarefa.Visible = false;
            label41.Visible = false;
            robot_reveive_message1.Visible = false;

            num_robo = 0;
            

        }



        private void form1_Load(object sender, EventArgs e)
        {
            system_state_on_flag = 1;
            motor_is_on = -1;
            program_is_running = -1;
            robot_availability = -1;

            processamento.Enabled = true;

        }
        public void commnad_register()
        {
            textBox_command.Text += robot_send_message.Text + Environment.NewLine;
            textBox_command.Text += robot_reveive_message.Text + Environment.NewLine;
        }
        public void initiate_robot_connection()
        {
            string msg;
            int port;
            string msg1;
            int port1;

            try
            {

                //defines the type of socket to open, namely, family, type and data protocol
                client_robot_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                //fetch the IP of the robot
                msg = textBox_ip1.Text;

                //serializes the IP of the robot and generates a proper IPAdress structure
                IPAddress robot_remote_IP = IPAddress.Parse(msg);

                //serializes the remote port, making it possible to be transmited
                port = int.Parse(textBox_port1.Text);

                //creates a proper IPEndpoint structure required by socket_connect
                IPEndPoint IP_Endpoint_remote = new IPEndPoint(robot_remote_IP, port);

                //try to connect to robot using the defined structures
                client_robot_socket.Connect(IP_Endpoint_remote);
                if (num_robo == 1)
                {
                    //defines the type of socket to open, namely, family, type and data protocol
                    client_robot_socket1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    //fetch the IP of the robot
                    msg1 = textBox_ip2.Text;
                    //serializes the IP of the robot and generates a proper IPAdress structure
                    IPAddress robot_remote_IP1 = IPAddress.Parse(msg1);
                    //serializes the remote port, making it possible to be transmited
                    port1 = int.Parse(textBox_port2.Text);
                    //creates a proper IPEndpoint structure required by socket_connect
                    IPEndPoint IP_Endpoint_remote1 = new IPEndPoint(robot_remote_IP1, port1);
                    //try to connect to robot using the defined structures
                    client_robot_socket1.Connect(IP_Endpoint_remote1);
                }

            }

            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }

        }

        private void processamento_Tick(object sender, EventArgs e)
        {
            if (system_state_on_flag == 0)
            {
                if (program_is_running == 1)
                {
                    if (robot_availability == 1) button_sensor.Enabled = true; else button_sensor.Enabled = false;
                    if (robot_availability == 1) button_automatic.Enabled = true; else button_automatic.Enabled = false;
                    if (robot_availability == 1) button_home.Enabled = true; else button_home.Enabled = false;
                    stop.Enabled = true;
                    program_start_main.Enabled = false;
                }
                else
                {
                    button_sensor.Enabled = false;
                    button_automatic.Enabled = false;
                    button_home.Enabled = false;
                    stop.Enabled = false;
                    if (motor_is_on == 1) program_start_main.Enabled = true;
                }

                if (motor_is_on == 1)
                {
                    motor_off.Enabled = true;
                    motor_on.Enabled = false;
                }
                else
                {
                    motor_off.Enabled = false;
                    motor_on.Enabled = true;
                    program_start_main.Enabled = false;

                }
            }

        }

        private void timer_robot_Tick(object sender, EventArgs e)
        {

            string msg;
            string msg2;

            timer_robot.Enabled = false;
            robot_send_message.Text = "system_state";
            send_command_robot_Click(sender, e);

            msg = robot_reveive_message.Text;
            msg2 = robot_reveive_message1.Text;
            parameters = msg.Split(' ');
            parameters1 = msg2.Split(' ');

            if (parameters[1] == "1")
            {
                label4.Text = "Motor: ON";
                label4.ForeColor = System.Drawing.Color.Green;
                motor_is_on = 1;
            }
            if (parameters[1] == "0")
            {
                motor_is_on = 0;
                label4.Text = "Motor: OFF";
                label4.ForeColor = System.Drawing.Color.Red;
            }
            if (parameters[2] == "1")
            {
                program_is_running = 1;
                label3.Text = "Program ON";
                label3.ForeColor = System.Drawing.Color.Green;
            }
            if (parameters[2] == "0")
            {
                program_is_running = 0;
                label3.Text = "Program OFF";
                label3.ForeColor = System.Drawing.Color.Red;
            }

            if (parameters[3] == "0")
            {
                if (program_is_running == 1) label5.Text = "Availability of robot: READY";
                else label5.Text = "Availability of robot: STANDBY";
                robot_availability = 1;
            }
            if (parameters[3] != "0")
            {
                label5.Text = "Availability of robot: BUSY";
                robot_availability = 0;
            }
            if (num_robo == 1 && parameters1[4] == "0")
            {
                executar_tarefa.Text = "DI 1: OFF";
                executar_tarefa.ForeColor = System.Drawing.Color.Red;
            }
            if (num_robo == 1 && parameters1[4] == "1")
            {
                executar_tarefa.Text = "DI 1: ON";
                executar_tarefa.ForeColor = System.Drawing.Color.Green;
            }
            if (num_robo == 1 && parameters1[5] == "0")
            {
                go_home.Text = "DI 2: OFF";
                go_home.ForeColor = System.Drawing.Color.Red;
            }
            if (num_robo == 1 && parameters1[5] == "1")
            {
                go_home.Text = "DI 2: ON";
                go_home.ForeColor = System.Drawing.Color.Green;
            }
            if (parameters[4] == "0")
            {
                executa_tarefa1.Text = "DI 1: OFF";
                executa_tarefa1.ForeColor = System.Drawing.Color.Red;
            }
            if (parameters[4] == "1")
            {
                executa_tarefa1.Text = "DI 1: ON";
                executa_tarefa1.ForeColor = System.Drawing.Color.Green;
            }
            if (parameters[5] == "0")
            {
                go_home1.Text = "DI 2: OFF";
                go_home1.ForeColor = System.Drawing.Color.Red;
            }
            if (parameters[5] == "1")
            {
                go_home1.Text = "DI 2: ON";
                go_home1.ForeColor = System.Drawing.Color.Green;
            }
            if (num_robo==1)
            {
                num_decision = Int32.Parse(parameters[6]);
                if (num_decision == 3)
                {
                    robot_send_message.Text = "write_num decision1 5";
                    send_command_robot_Click(sender, e);
                    commnad_register();
                }
            }

            timer_robot.Enabled = true;
        }

        private void stop_Click(object sender, EventArgs e)
        {
            robot_send_message.Text = "program_stop";
            send_command_robot_Click(sender, e);
            commnad_register();
        }

        private void motor_on_Click(object sender, EventArgs e)
        {
            robot_send_message.Text = "motor_on";
            send_command_robot_Click(sender, e);
            commnad_register();
        }

        private void motor_off_Click(object sender, EventArgs e)
        {
            robot_send_message.Text = "motor_off";
            send_command_robot_Click(sender, e);
            commnad_register();
            control.SelectedTab = configuration;
        }

        private void start_Click(object sender, EventArgs e)
        {
            robot_send_message.Text = "program_start_pp";
            send_command_robot_Click(sender, e);
            commnad_register();
        }

        private void automatico_Click(object sender, EventArgs e)
        {
            if (num_robo == 0)
            {
                robot_send_message.Text = "write_num decision1 5";
                send_command_robot_Click(sender, e);
                commnad_register();
            }
            robot_send_message.Text = "write_num comando 2";
            send_command_robot_Click(sender, e);
            commnad_register();
        }

        private void go_home_Click(object sender, EventArgs e)
        {
            robot_send_message.Text = "write_num comando 3";
            send_command_robot_Click(sender, e);
            commnad_register();
        }


        private void send_command_robot_Click(object sender, EventArgs e)
        {
            string msg;
            string msg1;
            byte[] sent_data;
            byte[] sent_data1;
            byte[] received_data = new byte[256];
            byte[] received_data1 = new byte[256];
            int number_of_received_bytes;
            int number_of_received_bytes1;

            //intiates the connection with the robot
            initiate_robot_connection();

            try
            {
                if (client_robot_socket.Connected)
                {
                    msg = robot_send_message.Text;
                    msg1 = robot_send_message.Text;
                    //serializes and encodes using a proper ASCII table
                    sent_data = System.Text.ASCIIEncoding.ASCII.GetBytes(msg);

                    //send message over open socket
                    client_robot_socket.Send(sent_data);

                    //read the answer
                    number_of_received_bytes = client_robot_socket.Receive(received_data);

                    if (num_robo == 1 && client_robot_socket1.Connected)
                    {
                        sent_data1 = System.Text.ASCIIEncoding.ASCII.GetBytes(msg1);
                        //send message over open socket
                        client_robot_socket1.Send(sent_data1);

                        //read the answer
                        number_of_received_bytes1 = client_robot_socket1.Receive(received_data1);

                        msg1 = System.Text.Encoding.ASCII.GetString(received_data1, 0, number_of_received_bytes1);

                        robot_reveive_message1.Text = msg1;
                        //close the socket
                        client_robot_socket1.Close();

                    }
                    //get data and make it writable on a text windows
                    msg = System.Text.Encoding.ASCII.GetString(received_data, 0, number_of_received_bytes);

                    //write message on text window
                    robot_reveive_message.Text = msg;

                    //close the socket
                    client_robot_socket.Close();
                }
                else MessageBox.Show("Application Message: Socket NOT connect, verify your cables and conections.");

            }
            catch (SocketException se)
            {
                MessageBox.Show(se.Message);
            }
        }

        private void system_state_on_Click(object sender, EventArgs e)
        {
            if(button_system.Text == "System State ON")
            {
              control.SelectedTab = mode;
            }

            if (system_state_on_flag == 1)
            {
                timer_robot.Enabled = true;
                button_system.Text = "System State OFF";
                system_state_on_flag = 0;
                timer_robot_Tick(sender, e);
                processamento_Tick(sender, e);
            }
            else
            {
                timer_robot.Enabled = false;
                button_system.Text = "System State ON";
                system_state_on_flag = 1;
                robot_send_message.Text = "motor_on";
            }
        }

        private void program_start_main_Click(object sender, EventArgs e)
        {
            robot_send_message.Text = "program_start_pp";
            send_command_robot_Click(sender, e);
            commnad_register();
        }

        private void button_2robots_Click(object sender, EventArgs e)
        {
            ip1.Visible = true;
            port1.Visible = true;
            ip2.Visible = true;
            port2.Visible = true;
            ip2.Visible = true;
            port2.Visible = true;
            textBox_ip2.Visible = true;
            textBox_port2.Visible = true;
            textBox_ip1.Visible = true;
            textBox_port1.Visible = true;
            go_home.Visible = true;
            executar_tarefa.Visible = true;
            label41.Visible = true;
            robot_reveive_message1.Visible = true;
            num_robo = 1;
        }

        private void button_sensor_Click(object sender, EventArgs e)
        {
            if((executa_tarefa1.Text == "DI 1: ON" && executar_tarefa.Text== "DI 1: OFF") || (executa_tarefa1.Text == "DI 1: OFF" && executar_tarefa.Text == "DI 1: ON") || num_robo==0)
            {
                robot_send_message.Text = "write_num decision1 5";
                send_command_robot_Click(sender, e);
                commnad_register();
            }
            robot_send_message.Text = "write_num comando 1";
            send_command_robot_Click(sender, e);
            commnad_register();
        }

        private void button_di1_Click(object sender, EventArgs e)
        {
            robot_send_message.Text = "digital_input 1";
            send_command_robot_Click(sender, e);
            commnad_register();
        }

        private void button_di2_Click(object sender, EventArgs e)
        {
            robot_send_message.Text = "write_num decision2 1";
            send_command_robot_Click(sender, e);
            commnad_register();
        }
    }
}
