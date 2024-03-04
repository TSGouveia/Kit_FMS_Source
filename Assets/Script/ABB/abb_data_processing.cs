/****************************************************************************
MIT License
Copyright(c) 2020 Roman Parak
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*****************************************************************************
Author   : Roman Parak
Email    : Roman.Parak @outlook.com
Github   : https://github.com/rparak
File Name: abb_data_processing.cs
****************************************************************************/

// System
using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Diagnostics;
// Unity
using UnityEngine;
using Debug = UnityEngine.Debug;

public class abb_data_processing : MonoBehaviour
{
    public static class GlobalVariables_Main_Control
    {
        public static bool connect, disconnect, movetopos;
    }

    public static class ABB_Stream_Data_XML
    {
        // IP Port Number and IP Address
        public static string ip_address;
        //  The target of reading the data: jointtarget / robtarget
        public static string xml_target = "";
        // Comunication Speed (ms)
        public static int time_step;
        // Joint Space:
        //  Orientation {J1 .. J6} (°)
        public static double[] J_Orientation = new double[6];
        // Class thread information (is alive or not)
        public static bool is_alive = false;
    }

    public static class ABB_Stream_Data_JSON
    {
        // IP Port Number and IP Address
        public static string ip_address;
        //  The target of reading the data: jointtarget / robtarget
        public static string json_target = "";
        // Comunication Speed (ms)
        public static int time_step;
        // Cartesian Space:
        //  Position {X, Y, Z} (mm)
        public static double[] C_Position = new double[3];
        //  Orientation {Quaternion} (-):
        public static double[] C_Orientation = new double[4];
        // Class thread information (is alive or not)
        public static bool is_alive = false;
    }

    // Class Stream {ABB Robot Web Services - XML}
    private ABB_Stream_XML ABB_Stream_Robot_XML;
    // Start Stream {ABB Robot Web Services - JSON}
    private ABB_Stream_JSON ABB_Stream_Robot_JSON;

    private MoveToPosition MoveToPositionRobot;

    // Other variables
    private int main_abb_state = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Initialization {Robot Web Services ABB - XML}
        //  Stream Data:
        ABB_Stream_Data_XML.ip_address = PlayerPrefs.GetString("ABB", "192.168.2.60");
        //  The target of reading the data: jointtarget / robtarget
        ABB_Stream_Data_XML.xml_target = "jointtarget";
        //  Communication speed (ms)
        ABB_Stream_Data_XML.time_step = 2;
        // Initialization {Robot Web Services ABB - JSON}
        //  Stream Data:
        ABB_Stream_Data_JSON.ip_address = PlayerPrefs.GetString("ABB", "192.168.2.60"); ;
        //  The target of reading the data: jointtarget / robtarget
        ABB_Stream_Data_JSON.json_target = "robtarget";
        //  Communication speed (ms)
        ABB_Stream_Data_JSON.time_step = 200;

        // Start Stream {ABB Robot Web Services - XML}
        ABB_Stream_Robot_XML = new ABB_Stream_XML();
        // Start Stream {ABB Robot Web Services - JSON}
        ABB_Stream_Robot_JSON = new ABB_Stream_JSON();

        MoveToPositionRobot = new MoveToPosition();

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        switch (main_abb_state)
        {
            case 0:
                {
                    // ------------------------ Wait State {Disconnect State} ------------------------//
                    if (GlobalVariables_Main_Control.connect == true)
                    {
                        //Start Stream { ABB Robot Web Services - XML}
                        ABB_Stream_Robot_XML.Start();
                        //Start Stream { ABB Robot Web Services - JSON}
                        ABB_Stream_Robot_JSON.Start();

                        MoveToPositionRobot.Start();

                        // go to connect state
                        main_abb_state = 1;
                    }
                }
                break;
            case 1:
                {
                    // ------------------------ Data Processing State {Connect State} ------------------------//
                    if (GlobalVariables_Main_Control.disconnect == true)
                    {
                        // Stop threading block {ABB Robot Web Services - XML}
                        if (ABB_Stream_Data_XML.is_alive == true)
                        {
                            ABB_Stream_Robot_XML.Stop();
                        }

                        // Stop threading block {ABB Robot Web Services - JSON}
                        if (ABB_Stream_Data_JSON.is_alive == true)
                        {
                            ABB_Stream_Robot_JSON.Stop();
                            MoveToPositionRobot.Stop();
                        }

                        if (ABB_Stream_Data_XML.is_alive == false && ABB_Stream_Data_JSON.is_alive == false)
                        {
                            // go to initialization state {wait state -> disconnect state}
                            main_abb_state = 0;
                        }
                    }
                }
                break;
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            // Destroy Stream {ABB Robot Web Services - XML}
            ABB_Stream_Robot_XML.Destroy();
            //Start Stream { ABB Robot Web Services - JSON}
            ABB_Stream_Robot_JSON.Destroy();

            MoveToPositionRobot.Destroy();

            Destroy(this);
        }
        catch (Exception)
        {
        }
    }



    class ABB_Stream_JSON
    {
        // Initialization of Class variables
        //  Thread
        private Thread robot_thread = null;
        private bool exit_thread = false;

        async void ABB_Stream_Thread_JSON()
        {
            var handler = new HttpClientHandler { Credentials = new NetworkCredential("Default User", "robotics") };
            // disable the proxy, the controller is connected on same subnet as the PC 
            handler.Proxy = null;
            handler.UseProxy = false;

            try
            {
                // Send a request continue when complete
                using (HttpClient client = new HttpClient(handler))
                {
                    // Initialization timer
                    var t = new Stopwatch();

                    while (exit_thread == false)
                    {
                        // t_{0}: Timer start.
                        t.Start();

                        // Current data streaming from the source page
                        using (HttpResponseMessage response = await client.GetAsync("http://" + ABB_Stream_Data_JSON.ip_address + "/rw/rapid/tasks/T_ROB1/motion?resource=" + ABB_Stream_Data_JSON.json_target + "&json=1"))
                        {
                            using (HttpContent content = response.Content)
                            {
                                try
                                {
                                    // Check that response was successful or throw exception
                                    response.EnsureSuccessStatusCode();
                                    // Get HTTP response from completed task.
                                    string result = await content.ReadAsStringAsync();
                                    //Debug.Log(result);
                                    // Deserialize the returned json string
                                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

                                    // Display controller name, version and version name
                                    var service = obj._embedded._state[0];

                                    // TCP {X, Y, Z} -> Read RWS JSON
                                    ABB_Stream_Data_JSON.C_Position[0] = (double)service.x;
                                    ABB_Stream_Data_JSON.C_Position[1] = (double)service.y;
                                    ABB_Stream_Data_JSON.C_Position[2] = (double)service.z;
                                    // Quaternion {q1 .. q4} -> Read RWS JSON
                                    ABB_Stream_Data_JSON.C_Orientation[0] = (double)service.q1;
                                    ABB_Stream_Data_JSON.C_Orientation[1] = (double)service.q2;
                                    ABB_Stream_Data_JSON.C_Orientation[2] = (double)service.q3;
                                    ABB_Stream_Data_JSON.C_Orientation[3] = (double)service.q4;
                                    //Debug.Log(service);

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }

                        // t_{1}: Timer stop.
                        t.Stop();

                        // Recalculate the time: t = t_{1} - t_{0} -> Elapsed Time in milliseconds
                        if (t.ElapsedMilliseconds < ABB_Stream_Data_JSON.time_step)
                        {
                            Thread.Sleep(ABB_Stream_Data_JSON.time_step - (int)t.ElapsedMilliseconds);
                        }

                        // Reset (Restart) timer.
                        t.Restart();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication Problem: {0}", e);
            }
        }

        public void Start()
        {
            exit_thread = false;
            // Start a thread to stream ABB Robot
            robot_thread = new Thread(new ThreadStart(ABB_Stream_Thread_JSON));
            robot_thread.IsBackground = true;
            robot_thread.Start();
            // Thread is active
            ABB_Stream_Data_JSON.is_alive = true;
        }
        public void Stop()
        {
            exit_thread = true;
            // Stop a thread
            Thread.Sleep(100);
            ABB_Stream_Data_JSON.is_alive = robot_thread.IsAlive;
            robot_thread.Abort();
        }
        public void Destroy()
        {
            // Stop a thread (Robot Web Services communication)
            Stop();
            Thread.Sleep(100);
        }
    }






    class ABB_Stream_XML
    {
        // Initialization of Class variables
        //  Thread
        private Thread robot_thread = null;
        private bool exit_thread = false;

        async void ABB_Stream_Thread_XML()
        {
            var handler = new HttpClientHandler { Credentials = new NetworkCredential("Default User", "robotics") };
            // disable the proxy, the controller is connected on same subnet as the PC 
            handler.Proxy = null;
            handler.UseProxy = false;

            try
            {
                // Send a request continue when complete
                using (HttpClient client = new HttpClient(handler))
                {
                    // Initialization timer
                    var t = new Stopwatch();

                    while (exit_thread == false)
                    {
                        // t_{0}: Timer start.
                        t.Start();

                        // Current data streaming from the source page
                        using (HttpResponseMessage response = await client.GetAsync("http://" + ABB_Stream_Data_JSON.ip_address + "/rw/motionsystem/mechunits/ROB_1/" + ABB_Stream_Data_XML.xml_target + "?json=1"))
                        {
                            using (HttpContent content = response.Content)
                            {
                                try
                                {
                                    // Check that response was successful or throw exception
                                    response.EnsureSuccessStatusCode();
                                    
                                    // Get HTTP response from completed task.
                                    string result = await content.ReadAsStringAsync();
                                    //Debug.Log(result);
                                    
                                    // Deserialize the returned json string
                                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

                                    // Display controller name, version and version name
                                    var service = obj._embedded._state[0];

                                    ABB_Stream_Data_XML.J_Orientation[0] = (double)service.rax_1;
                                    ABB_Stream_Data_XML.J_Orientation[1] = (double)service.rax_2;
                                    ABB_Stream_Data_XML.J_Orientation[2] = (double)service.rax_3;
                                    ABB_Stream_Data_XML.J_Orientation[3] = (double)service.rax_4;
                                    ABB_Stream_Data_XML.J_Orientation[4] = (double)service.rax_5;
                                    ABB_Stream_Data_XML.J_Orientation[5] = (double)service.rax_6;

                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            }
                        }

                        // t_{1}: Timer stop.
                        t.Stop();

                        // Recalculate the time: t = t_{1} - t_{0} -> Elapsed Time in milliseconds
                        if (t.ElapsedMilliseconds < ABB_Stream_Data_XML.time_step)
                        {
                            Thread.Sleep(ABB_Stream_Data_XML.time_step - (int)t.ElapsedMilliseconds);
                        }

                        // Reset (Restart) timer.
                        t.Restart();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication Problem: {0}", e);
            }
        }

        public void Start()
        {
            exit_thread = false;
            // Start a thread to stream ABB Robot
            robot_thread = new Thread(new ThreadStart(ABB_Stream_Thread_XML));
            robot_thread.IsBackground = true;
            robot_thread.Start();
            //Debug.Log("avv");
            // Thread is active
            ABB_Stream_Data_XML.is_alive = true;
        }
        public void Stop()
        {
            exit_thread = true;
            // Stop a thread
            Thread.Sleep(100);
            //ABB_Stream_Data_XML.is_alive = robot_thread.IsAlive;
            ABB_Stream_Data_XML.is_alive = false;
            
            //robot_thread.Abort();
        }
        public void Destroy()
        {
            // Stop a thread (Robot Web Services communication)
            Stop();
            Thread.Sleep(100);
        }
    }


   /* void MoveToPosition()
    {

        if (GlobalVariables_Main_Control.movetopos == true){
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://" + ABB_Stream_Data_JSON.ip_address + "/rw/motionsystem/mechunits/ROB_1?action=mechunit-position");
        request.Content = new StringContent("rob_joint=[18.23,8.45,-13.23,-5.25,13.63,-72.31]&ext_joint=[0,0,0,0,0,0]");
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        }
    }*/


    class MoveToPosition
    {
        // Initialization of Class variables
        //  Thread
        private Thread robot_thread = null;
        private bool exit_thread = false;

        async void MoveToPosition_Thread()
        {
            var handler = new HttpClientHandler { Credentials = new NetworkCredential("Default User", "robotics") };
            // disable the proxy, the controller is connected on same subnet as the PC 
            handler.Proxy = null;
            handler.UseProxy = false;

            try
            {
                //Debug.Log("a");
                // Send a request continue when complete
                using (HttpClient client = new HttpClient(handler))
                {
                    // Initialization timer
                    var t = new Stopwatch();

                    while (exit_thread == false)
                    {
                        // t_{0}: Timer start.
                        t.Start();

                        if (GlobalVariables_Main_Control.movetopos == true){
                            Debug.Log("b");

                            /*HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://" + ABB_Stream_Data_JSON.ip_address + "/rw/motionsystem/mechunits/ROB_1?action=mechunit-position");
                            request.Content = new StringContent("rob_joint=[18.23,8.45,-13.23,-5.25,13.63,-72.31]&ext_joint=[0,0,0,0,0,0]");
                            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                            

                            HttpResponseMessage response = await client.SendAsync(request);
                            Debug.Log(response);
                            response.EnsureSuccessStatusCode();
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Debug.Log(responseBody);*/

                            /*
                                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "http://" + ABB_Stream_Data_JSON.ip_address + "/rw/motionsystem/mechunits/ROB_1?action=mechunit-position"))
                                {
                                    var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("Default User:robotics"));
                                    request.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}"); 

                                    request.Content = new StringContent("rob_joint=[-0.62,-3.54,-11.98,0.36,103.57,0]&ext_joint=[0,0,0,0,0,0]");
                                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/xhtml+xml"); 
                                    request.Content.Headers.a = MediaTypeHeaderValue.Parse("application/json");

                                    var response = await client.SendAsync(request);
                                    Debug.Log(response);

                                    HttpContent responseContent = response.Content;

                                    string responseBody = await responseContent.ReadAsStringAsync();
                                    Debug.Log(responseBody);
                                }
                            */







                            var requestContent = new StringContent("rob_joint=[-0.62,-3.54,-11.98,0.36,103.56,0]&ext_joint=[0,0,0,0,0,0]");
                            

                            // Get the response.
                            HttpResponseMessage response = await client.PostAsync(
                                "http://" + ABB_Stream_Data_JSON.ip_address + "/rw/motionsystem/mechunits/ROB_1?action=mechunit-position",
                                requestContent);

                                Debug.Log(response);

                            // Get the response content.
                            HttpContent responseContent = response.Content;

                            string responseBody = await responseContent.ReadAsStringAsync();
                            Debug.Log(responseBody);





                            // Get the stream of the content.
                            /*using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                            {
                                // Write the output.
                                //Console.WriteLine(await reader.ReadToEndAsync());
                            }*/





                            GlobalVariables_Main_Control.movetopos = false;
                        }
                        

                        // t_{1}: Timer stop.
                        t.Stop();

                        // Reset (Restart) timer.
                        t.Restart();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication Problem: {0}", e);
            }
        }

        public void Start()
        {
            exit_thread = false;
            // Start a thread to stream ABB Robot
            robot_thread = new Thread(new ThreadStart(MoveToPosition_Thread));
            robot_thread.IsBackground = true;
            robot_thread.Start();
            //Debug.Log("a");
            // Thread is active
            //ABB_Stream_Data_XML.is_alive = true;
            
        }
        public void Stop()
        {
            exit_thread = true;
            // Stop a thread
            Thread.Sleep(100);
            //ABB_Stream_Data_XML.is_alive = robot_thread.IsAlive;
            //ABB_Stream_Data_XML.is_alive = false;
            
            //robot_thread.Abort();
        }
        public void Destroy()
        {
            // Stop a thread (Robot Web Services communication)
            Stop();
            Thread.Sleep(100);
        }
    }

}