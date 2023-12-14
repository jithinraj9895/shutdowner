using System.Diagnostics;
using System.Management;

class Program
{
    static void Main()
    {

        // Start a new thread to continuously monitor processes
        System.Threading.Thread monitoringThread = new System.Threading.Thread(MonitorProcesses);
        monitoringThread.Start();

        // Keep the main thread alive
        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();

        int batteryPercentage = GetBatteryPercentage();
        if (batteryPercentage != -1 || batteryPercentage > 20 || false)
        {
            Console.WriteLine($"Current battery percentage: {batteryPercentage}%");
            
            if(batteryPercentage < 40)
            {
                ShutdownSystem(5);
            }

            if (batteryPercentage > 85)
            {
                Console.WriteLine("Battery is higher than 85 so give me some hours to shut down:");
                if(int.TryParse(Console.ReadLine(), out int hour) && hour > 0)
                    ShutdownSystem(hourTosecond(hour));
                else
                    Console.WriteLine("Dude need a valid num like an int");
            }

        }
        else
        {
            Console.WriteLine("Unable to retrieve battery information.");
        }

    }

    static void ShutdownSystem(int seconds)
    {
        if(seconds < 600)
            Console.WriteLine($"System is going to shutdown in {seconds} seconds - EVERYONE RUNNNN");
        string cmd1 = "/s /t "+seconds;
        Process.Start("shutdown",cmd1);

        Console.WriteLine("to ABORT PRESS any alphabet");
        char.TryParse(Console.ReadLine(), out char val);
        if ((val >= 97 && val <= 122) || (val >= 65 && val <= 90))
            ShutdownAbort();
        else
            Console.WriteLine("DAMN it, ABORT ABORTED do it manually");
    }

    static void ShutdownAbort()
    {

        Console.WriteLine("Trying shut down abort - MAYDAY");
        Process.Start("shutdown", "/a");
    }

    static int GetBatteryPercentage()
    {
        try
        {
            ObjectQuery query = new ObjectQuery("SELECT * FROM Win32_Battery");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            foreach (ManagementObject mo in searcher.Get())
            {
                return Convert.ToInt32(mo["EstimatedChargeRemaining"]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        return -1; // Indicates an error occurred
    }

    static int hourTosecond(int hour)
    {
        Console.WriteLine($"System is shutting down in {hour} hour - JUST CHILL");
        int second = hour * 3600; 
        return second;
    }

    static void MonitorProcesses()
    {
        while (true)
        {
            // Get the list of all processes currently running on the system
            Process[] processes = Process.GetProcesses();

            Console.WriteLine("Processes:");

            // Display information about each process
            foreach (Process process in processes)
            {
                if(process.MainWindowHandle != IntPtr.Zero)
                    Console.WriteLine($"Name: {process.ProcessName}, ID: {process.Id}, Memory Usage: {process.WorkingSet64} bytes");
            }

            // Sleep for a short interval (e.g., 1 second) before checking processes again
            System.Threading.Thread.Sleep(1000);
            break;
        }
    }
}
