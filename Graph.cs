namespace trains_test_task
{
    public class Graph
    {
        public List<Edge> edges;
        public List<Vertex> vertices;
        public List<RouteInfo> routes;
        public Graph()
        {
            this.edges = new List<Edge>();
            this.vertices = new List<Vertex>();
            this.routes = new List<RouteInfo>();
        }
        public void AddEdge(Passage passage)
        {
            Edge newEdge = new Edge(passage);
            edges.Add(newEdge);
            UpdateVertex(new Vertex(passage.departureStation), newEdge);
        }
        private void UpdateVertex(Vertex vert, Edge edge)
        {
            if(this.vertices.Where(v => v.departureStation == vert.departureStation).Count() == 0)
            {
                vert.edges.Add(edge);
                vertices.Add(vert);
            }
            else
            {
                vertices.FindAll(v => v.departureStation == vert.departureStation).ForEach(v => v.edges.Add(edge));
            }
        }
        public double[,] GetMinCostMatrix()
        {
            double[,] matrix = new double[this.vertices.Count+1,this.vertices.Count+1];

            // inserting station numbers
            int position = 1;
            foreach(Vertex v in vertices) 
            {
                matrix[0,position] = v.departureStation;
                matrix[position,0] = v.departureStation;
                position ++;
            }

            // inserting min cost of every route
            foreach(Edge e in edges)
            {
                int depPos = FindPositionInMatrix(matrix, e.vertexFrom);
                int arrPos = FindPositionInMatrix(matrix, e.vertexTo);
                if(matrix[depPos,arrPos] == 0)
                {
                    matrix[depPos,arrPos] = e.passage.cost;
                }
                else
                {
                    if(e.passage.cost < matrix[depPos,arrPos])
                    {
                        matrix[depPos,arrPos] = e.passage.cost;
                    }
                }
            }

            // inserting inf if there is no such route
            for(int i = 1; i < matrix.GetLength(0); i++)
            {
                for(int j = 1; j < matrix.GetLength(1); j++)
                {
                    if(matrix[i,j] == 0)
                    {
                        matrix[i,j] = Double.PositiveInfinity;
                    }
                }
            }
            return matrix;
        }
        private int FindPositionInMatrix(double[,] matrix, int station)
        {
            int pos = 0;
            for(int i = 0; i < matrix.GetLength(0); i++)
            {
                if(matrix[0,i] == station)
                {
                    pos = i; break;
                }
            }
            return pos;
        }
        public List<RouteInfo> GetFastestWay()
        {
            this.routes = new List<RouteInfo>();
            foreach(Vertex v in this.vertices)
            {
                VerticesTraversal(v, 0, new DateTime(), new List<(int, int, int)>());
            }
            double min = double.PositiveInfinity;
            foreach (RouteInfo item in routes)
            {
                if(item.generalTime < min)
                {
                    min = item.generalTime;
                }
            }
            return routes.FindAll(t => t.generalTime == min); 
        }
        private void VerticesTraversal(Vertex v,int genSum, DateTime lastArrival, List<(int,int,int)> currRoute) 
        {   // should also include time to wait the next train
            foreach(Edge e in v.edges)
            {
                List<(int, int, int)> currIt = new List<(int, int, int)>();
                foreach ((int, int, int) item in currRoute)
                {
                    currIt.Add(item);
                }

                // exclude loops
                if(currIt.Where(t => t.Item1 == e.vertexTo).Count() != 0) 
                {
                    continue;
                }
                
                // time in travel (minutes)
                if(e.passage.departureTime < e.passage.arrivalTime)
                {
                    genSum += e.passage.arrivalTime.Hour*60 + e.passage.arrivalTime.Minute - e.passage.departureTime.Hour*60 - e.passage.departureTime.Minute;
                }
                else
                {
                    genSum += 24*60 - (e.passage.departureTime.Hour*60 + e.passage.departureTime.Minute - e.passage.arrivalTime.Hour*60 - e.passage.arrivalTime.Minute);
                }

                // time to wait for train (except the 1-st station)
                if(currIt.Count != 0)
                {
                    if(e.passage.departureTime < lastArrival)
                    {
                        genSum += 24*60 - (lastArrival.Hour * 60 + lastArrival.Minute - e.passage.departureTime.Hour*60 - e.passage.departureTime.Minute); 
                    }
                    else
                    {
                        genSum += e.passage.departureTime.Hour * 60 + e.passage.departureTime.Minute - lastArrival.Hour * 60 - lastArrival.Minute;
                    }
                }

                currIt.Add((e.vertexFrom, e.vertexTo, e.passage.trainNumber));
                
                // check when to cancel the recursion
                if(currIt.Count != 5)
                {
                    VerticesTraversal(this.GetVertexByStation(e.vertexTo), genSum, e.passage.arrivalTime, currIt);
                }
                else
                {
                    RouteInfo r = new RouteInfo(genSum, currIt);
                    this.routes.Add(r);
                }
            }
        }
        public Vertex GetVertexByStation(int number)
        {
            return vertices.Where(v => v.departureStation == number).First();
        }
        public List<Edge> GetEdgesByStations(int departureStation, int arrivalStation)
        {
            List<Edge> edges = new List<Edge>();
            List<Edge> departureVertices = this.vertices.Find(v => v.departureStation == departureStation).edges;
            foreach (Edge e in departureVertices)
            {
                if(e.vertexTo == arrivalStation)
                {
                    edges.Add(e);
                }
            }
            return edges;
        }
    }
    public class Edge
    {
        public Passage passage;
        public int vertexFrom;
        public int vertexTo;
        public Edge(Passage passage)
        {
            this.passage = passage;
            this.vertexFrom = passage.departureStation;
            this.vertexTo = passage.arrivalStation;
        }
    }
    public class Vertex
    {
        public int departureStation;
        public List<Edge> edges;
        public Vertex(int station)
        {
            this.edges = new List<Edge>();
            this.departureStation = station;
        }
    }
}