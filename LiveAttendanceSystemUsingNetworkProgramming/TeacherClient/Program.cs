using System;
using System.Net.Sockets;
using System.Text;

class TeacherClient
{
    static void Main()
    {
        // Infinite loop to keep showing teacher menu
        while (true)
        {
            Console.WriteLine("\n--- Teacher Control Panel ---");
            Console.WriteLine("1. Start Attendance Session");
            Console.WriteLine("2. View Formatted Report");
            Console.WriteLine("3. Exit");
            Console.Write("Select: ");

            string choice = Console.ReadLine();

            // Exit option
            if (choice == "3") break;

            try
            {
                // Create TCP connection to server (localhost, port 9000)
                TcpClient client = new TcpClient("127.0.0.1", 9000);

                // Get network stream for communication
                NetworkStream stream = client.GetStream();

                // Option 1: Start attendance session
                if (choice == "1")
                {
                    Console.Write("Enter duration (sec): ");
                    string sec = Console.ReadLine();

                    // Send session start command with duration to server
                    byte[] data = Encoding.UTF8.GetBytes("START_SESSION:" + sec);
                    stream.Write(data, 0, data.Length);

                    Console.WriteLine("Session Started!");
                }
                // Option 2: Request attendance report
                else if (choice == "2")
                {
                    // Send request to server to get attendance report
                    byte[] data = Encoding.UTF8.GetBytes("GET_REPORT");
                    stream.Write(data, 0, data.Length);

                    // Buffer to receive large formatted table
                    byte[] buffer = new byte[8192];

                    // Read response from server (blocking call)
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

                    // Convert received bytes into string
                    string report = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Clear console and display report neatly
                    Console.Clear();
                    Console.WriteLine("\n=== LIVE ATTENDANCE REPORT TABLE ===");
                    Console.WriteLine(report);
                }

                // Close connection after each request (short-lived connection)
                client.Close();
            }
            catch
            {
                // Handle case when server is not running
                Console.WriteLine("Error: Server Offline!");
            }
        }
    }
}