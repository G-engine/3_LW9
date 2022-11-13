StreamReader tickerFile = new StreamReader("ticker.txt");
string [] tickerArray = tickerFile.ReadToEnd().Split('\n', StringSplitOptions.TrimEntries);
tickerFile.Close();

StreamWriter output = new StreamWriter("averageStats.txt");
using HttpClient client = new HttpClient();
List<Task> tasks = new List<Task>();
object locker = new object();

foreach (string ticker in tickerArray)
{
    try
    {
        List<double> averagePerDay = new List<double>();
        var content = await client.GetStringAsync(
            $"https://query1.finance.yahoo.com/v7/finance/download/{ticker.Trim('\r')}?period1=1636416000&period2=1667952000&interval=1d&events=history&includeAdjustedClose=true");
        Task task = Task.Factory.StartNew(() =>
        {
            string [] dayContent = content.Split('\n', StringSplitOptions.TrimEntries);
            dayContent[0] = "";
            foreach (var day in dayContent)
            {
                int i1 = 0, i2 = 0, i3 = 0;
                for (int i = 0; i < day.Length; ++i)
                {
                    if (day[i] == ',' && i != day.IndexOf(','))
                    {
                        if (i1 == 0) i1 = i;
                        else if (i2 == 0) i2 = i;
                        else if (i3 == 0) i3 = i;
                    }
                }

                if (i1 > 0)
                {
                    double high = Double.Parse(day.Substring(i1 + 1, i2 - i1 - 1).Replace('.', ','));
                    double low = Double.Parse(day.Substring(i2 + 1, i3 - i2 - 1).Replace('.', ','));
                    averagePerDay.Add((high + low)/2);
                }
            }
            lock (locker)
            {
                output.WriteLine($"{ticker}: {averagePerDay.Average()}");
            }
        });
        tasks.Add(task);
    }
    catch(Exception e)
    {
        Console.WriteLine("Error: " + ticker + ", " + e.Message);
    }
}
Task.WaitAll(tasks.ToArray());
output.Close();