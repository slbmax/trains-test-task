namespace trains_test_task
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            // graph
            string filePath = "test_task_data.csv";
            Graph graph = CreateGraphFromFile(filePath);

            // 1. travelling salesman problem
            double[,] matrix = graph.GetMinCostMatrix();
            TSP tsp = new TSP(matrix);
            List<(int, int)> cheapestRoute = tsp.GetNearestWay();

            Console.WriteLine("Best route by price:");
            double totalPrice = 0;
            foreach((int, int) passInfo in cheapestRoute)
            {
                List<Edge> stationEdges = graph.GetEdgesByStations(passInfo.Item1, passInfo.Item2);
                double minPrice = double.PositiveInfinity;
                List<int> trainNumbers = new List<int>();
                foreach(Edge e in stationEdges)
                {
                    if(e.passage.cost < minPrice)
                    {
                        trainNumbers = new List<int>();
                        minPrice = e.passage.cost;
                        trainNumbers.Add(e.passage.trainNumber);
                    }
                    else if(e.passage.cost == minPrice)
                    {
                        trainNumbers.Add(e.passage.trainNumber);
                    }
                }
                totalPrice += minPrice;
                Console.Write($"[{passInfo.Item1} - {passInfo.Item2}] | train № ");
                for(int i = 0; i < trainNumbers.Count; i++)
                {
                    if(i != 0)  Console.Write("/");
                    Console.Write(trainNumbers[i]);
                }
                Console.WriteLine();
            }
            Console.WriteLine("Total price: "+ totalPrice.ToString("N2") + " UAH");

            // 2. graph traversal
            List<RouteInfo> fastestRoutes = graph.GetFastestWay();
            
            Console.WriteLine("\nBest route(s) by time:");
            for(int i = 0; i < fastestRoutes.Count; i++)
            {
                foreach ((int, int, int) passInfo in fastestRoutes[i].route)
                {
                    Console.WriteLine($"[{passInfo.Item1} - {passInfo.Item2}] | train № {passInfo.Item3}");
                }
                Console.WriteLine($"Total time: {fastestRoutes[i].generalTime/60} hours {fastestRoutes[i].generalTime%60} minutes");
            }
        }
        static Graph CreateGraphFromFile(string filePath)
        {
            List<Passage> list = new List<Passage>();
            try
            {
                list = FileManager.ParseCsvToPassageList("test_task_data.csv");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
            Graph graph = new Graph();
            foreach(Passage passage in list)
            {
                graph.AddEdge(passage);
            }
            return graph;
        }
    }
}
