using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;

class AttendanceServer
{
    // Stores connected students in lobby (Key = RollNo | Name, Value = TcpClient)
    static Dictionary<string, TcpClient> lobbyStudents = new Dictionary<string, TcpClient>();

    // Stores attendance records (Present students with timestamp)
    static List<string> attendanceList = new List<string>();

    static bool isSessionActive = false; // Indicates if attendance session is running
    static System.Timers.Timer sessionTimer; // Timer for session countdown
    static int timeLeft; // Remaining time in seconds

    const string fileName = "AttendanceReport.csv"; // File to store attendance report

    static void Main()
    {
        // Create TCP server listening on port 9000
        TcpListener server = new TcpListener(IPAddress.Any, 9000);
        server.Start();
        Console.WriteLine("--- Attendance Server Started (Port: 9000) ---");

        // Continuously accept incoming client connections
        while (true)
        {
            TcpClient client = server.AcceptTcpClient();

            // Handle each client in a separate thread (multi-client support)
            Thread t = new Thread(() => HandleClient(client));
            t.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        string currentStudentRoll = ""; // Stores current student's identity

        try
        {
            while (true)
            {
                byte[] buffer = new byte[1024];

                // Read incoming data from client (blocking call)
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                // Teacher starts attendance session
                if (message.StartsWith("START_SESSION"))
                {
                    int seconds = int.Parse(message.Split(':')[1]);

                    // Notify all students that session has started
                    BroadcastToStudents("ALERT: Attendance Session Started! Press 'P' now.");

                    // Start countdown timer
                    StartAttendanceTimer(seconds);
                }
                // Teacher requests attendance report
                else if (message == "GET_REPORT")
                {
                    string tableReport = GenerateTableReport();

                    // Send formatted report back to teacher
                    SendMessage(stream, tableReport);
                }
                // Student connects and enters lobby
                else if (message.StartsWith("STUDENT_CONNECT:"))
                {
                    // Format: STUDENT_CONNECT:RollNo | Name
                    currentStudentRoll = message.Split(':')[1];

                    lock (lobbyStudents)
                    {
                        // Add student if not already present
                        if (!lobbyStudents.ContainsKey(currentStudentRoll))
                            lobbyStudents.Add(currentStudentRoll, client);
                    }

                    Console.WriteLine($"[LOBBY] New student waiting: {currentStudentRoll}");
                }
                // Handle attendance marking when session is active
                else if (isSessionActive)
                {
                    // Create timestamp for attendance record
                    string timestamp = DateTime.Now.ToString("yyyy-MM-dd | HH:mm:ss");

                    // Full attendance entry format
                    string entry = $"{message} | {timestamp} | Present";

                    lock (attendanceList)
                    {
                        // Extract only roll number for duplicate checking
                        string rollOnly = message.Split('|')[0].Trim();

                        // Prevent duplicate attendance entries
                        if (!attendanceList.Exists(x => x.StartsWith(rollOnly)))
                        {
                            attendanceList.Add(entry);
                            Console.WriteLine($"\n[SUCCESS] Marked Present: {rollOnly}");

                            // Send success response to student
                            SendMessage(stream, "Attendance Recorded!");
                        }
                        else
                        {
                            // If already marked present
                            SendMessage(stream, "Error: Already Marked.");
                        }
                    }
                }
            }
        }
        catch
        {
            // Ignore errors (can be improved with logging)
        }
        finally
        {
            // Remove student from lobby on disconnect
            if (!string.IsNullOrEmpty(currentStudentRoll))
            {
                lock (lobbyStudents)
                {
                    lobbyStudents.Remove(currentStudentRoll);
                }

                Console.WriteLine($"[LOBBY] Student disconnected: {currentStudentRoll}");
            }

            client.Close();
        }
    }

    static string GenerateTableReport()
    {
        StringBuilder sb = new StringBuilder();

        // Table header
        sb.AppendLine("------------------------------------------------------------------------------------------");
        sb.AppendLine(string.Format("| {0,-4} | {1,-25} | {2,-12} | {3,-10} | {4,-10} |", "SNo", "Student (Roll | Name)", "Date", "Time", "Status"));
        sb.AppendLine("------------------------------------------------------------------------------------------");

        int count = 1;

        lock (lobbyStudents)
        {
            foreach (var studentKey in lobbyStudents.Keys)
            {
                // Check if student exists in attendance list
                string record = attendanceList.FirstOrDefault(x => x.StartsWith(studentKey));

                if (record != null)
                {
                    // If present, extract details
                    string[] p = record.Split('|');

                    sb.AppendLine(string.Format("| {0,-4} | {1,-25} | {2,-12} | {3,-10} | {4,-10} |",
                        count++, $"{p[0].Trim()} | {p[1].Trim()}", p[2].Trim(), p[3].Trim(), "Present"));
                }
                else
                {
                    // If not marked present → absent
                    sb.AppendLine(string.Format("| {0,-4} | {1,-25} | {2,-12} | {3,-10} | {4,-10} |",
                        count++, studentKey, "---", "---", "ABSENT"));
                }
            }
        }

        sb.AppendLine("------------------------------------------------------------------------------------------");
        return sb.ToString();
    }

    static void StartAttendanceTimer(int seconds)
    {
        // Clear previous attendance records
        lock (attendanceList)
        {
            attendanceList.Clear();
        }

        isSessionActive = true;
        timeLeft = seconds;

        // Stop previous timer if running
        if (sessionTimer != null)
            sessionTimer.Stop();

        // Create new timer (ticks every 1 second)
        sessionTimer = new System.Timers.Timer(1000);

        sessionTimer.Elapsed += (s, e) =>
        {
            timeLeft--;

            if (timeLeft <= 0)
            {
                sessionTimer.Stop();
                isSessionActive = false;

                // Save attendance data to CSV file
                SaveToCSV();

                // Display session end message on server console
                Console.Write("\rTime Remaining: 0s  \n");
                Console.WriteLine("***************************");
                Console.WriteLine("      SESSION ENDED        ");
                Console.WriteLine("***************************");

                // Notify all students that session has ended
                BroadcastToStudents("ALERT: Session Ended!");
            }
            else
            {
                // Update remaining time on server console
                Console.Write($"\rTime Remaining: {timeLeft}s  ");
            }
        };

        sessionTimer.Start();
    }

    static void BroadcastToStudents(string alert)
    {
        byte[] data = Encoding.UTF8.GetBytes(alert);

        // Send message to all connected students
        lock (lobbyStudents)
        {
            foreach (var s in lobbyStudents.Values)
            {
                try
                {
                    s.GetStream().Write(data, 0, data.Length);
                }
                catch
                {
                    // Ignore failed connections
                }
            }
        }
    }

    // Send message to a specific client
    static void SendMessage(NetworkStream stream, string msg)
    {
        byte[] data = Encoding.UTF8.GetBytes(msg);
        stream.Write(data, 0, data.Length);
    }

    // Save attendance records to CSV file
    static void SaveToCSV()
    {
        lock (attendanceList)
        {
            File.WriteAllLines(fileName, attendanceList);
        }
    }
}