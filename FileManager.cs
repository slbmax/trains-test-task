namespace trains_test_task
{
    public static class FileManager
    {
        public static List<Passage> ParseCsvToPassageList(string filePath)
        {
            if(!File.Exists(filePath))
            {
                throw new Exception("Error: file is not exist");
            }
            List<Passage> result = new List<Passage>();
            StreamReader reader = new StreamReader(filePath);
            while(true)
            {
                string currRow = reader.ReadLine();
                if(currRow == null)
                {
                    break;
                }

                string[] values = currRow.Split(";");
                if(values.Length != 6)
                {
                    throw new Exception("Error: inappropriate csv file");
                }

                int trainNumber, departureStation, arrivalStation;
                double cost;
                DateTime departureTime, arrivalTime;

                if(!int.TryParse(values[0], out trainNumber)
                || !int.TryParse(values[1], out departureStation)
                || !int.TryParse(values[2], out arrivalStation))
                {
                    throw new Exception("Error: inappropriate integer values in csv file");
                }
                if(!double.TryParse(values[3].Replace(".",","), out cost))
                {
                    throw new Exception("Error: inappropriate double values in csv file");
                }
                if(!DateTime.TryParse(values[4], out departureTime)
                || !DateTime.TryParse(values[5], out arrivalTime))
                {
                    throw new Exception("Error: inappropriate time values in csv file");
                }

                result.Add(new Passage(trainNumber, departureStation, arrivalStation,cost,departureTime, arrivalTime));
            }
            reader.Close();
            return result;
        }
    }
}