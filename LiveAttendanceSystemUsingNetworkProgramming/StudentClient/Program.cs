using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class StudentClient
{
    static TcpClient client;
    static NetworkStream stream;
    static string studentDetails; // Stores "RollNo | Name"
    static bool isSessionOpen = false; // Tracks whether attendance session is active

    static void Main()
    {
        try
        {
            // Connect to server using TCP (localhost and port 9000)
            client = new TcpClient("127.0.0.1", 9000);
            stream = client.GetStream();

            Console.WriteLine("--- Student Portal Connected ---");

            // Take student input (Roll No and Name)
            Console.Write("Enter Roll No: ");
            string roll = Console.ReadLine();
            Console.Write("Enter Full Name: ");
            string name = Console.ReadLine();

            // Combine roll and name into single string
            studentDetails = $"{roll} | {name}";

            // Send connection message to server (Lobby registration)
            byte[] connectMsg = Encoding.UTF8.GetBytes("STUDENT_CONNECT:" + studentDetails);
            stream.Write(connectMsg, 0, connectMsg.Length);

            // Start background thread to listen for server alerts (non-blocking)
            Thread listenerThread = new Thread(ListenForAlerts);
            listenerThread.IsBackground = true;
            listenerThread.Start();

            Console.WriteLine("\n[STATUS] You are now in the Lobby.");
            Console.WriteLine("[INFO] Please wait for the teacher to start the session...");

            // Main loop to detect key press from user
            while (true)
            {
                var key = Console.ReadKey(true).Key;

                // If user presses 'P' → try to mark attendance
                if (key == ConsoleKey.P)
                {
                    if (isSessionOpen)
                    {
                        // Send attendance data to server
                        byte[] bytes = Encoding.UTF8.GetBytes(studentDetails);
                        stream.Write(bytes, 0, bytes.Length);

                        Console.WriteLine(">> Attendance request sent to server...");
                    }
                    else
                    {
                        // Prevent marking attendance when session is closed
                        Console.WriteLine("\n[WAIT] Session is not active. You cannot mark attendance yet.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle connection errors
            Console.WriteLine("\n[ERROR] Connection Lost: " + ex.Message);
            Console.ReadKey();
        }
    }

    static void ListenForAlerts()
    {
        try
        {
            while (true)
            {
                byte[] buffer = new byte[1024];

                // Read incoming data from server (blocking call)
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break; // Server disconnected

                string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Detect session start message
                if (msg.Contains("Attendance Session Started"))
                {
                    isSessionOpen = true; // Enable attendance marking
                    Console.Beep(); // Notification sound

                    Console.WriteLine($"\n\n[NOTIFICATION] {msg.Replace("ALERT:", "").Trim()}");
                    Console.Write("Press 'P' now to mark Present: ");
                }
                // Detect session end message
                else if (msg.Contains("Session Ended"))
                {
                    isSessionOpen = false; // Disable attendance marking

                    Console.WriteLine($"\n\n[NOTIFICATION] {msg.Replace("ALERT:", "").Trim()}");
                    Console.WriteLine("Attendance is now locked.");
                }
                else
                {
                    // Handle general server responses (e.g., success/error messages)
                    Console.WriteLine($"\n[SERVER] {msg}");
                }
            }
        }
        catch
        {
            // If connection is lost or server stops
            Console.WriteLine("\n[SYSTEM] Server connection closed.");
        }
    }
}