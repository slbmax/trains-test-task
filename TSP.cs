namespace trains_test_task
{
    public class TSP
    {
        private double[,] matrix;
        private List<(int, int)> route; // (depSt , arrSt)
        public TSP(double[,] m)
        {
            this.matrix = m;
            this.route = new List<(int, int)>();
        }
        public List<(int, int)> GetNearestWay() // *in this case - cheapest
        {
            ExecuteReduction();
            while(matrix.GetLength(0) != 3)
            {
                FindBranchingEdge();
            }

            // adding last route
            for(int i = 1; i < matrix.GetLength(0); i++)
            {
                for(int j = 1; j < matrix.GetLength(1); j++)
                {
                    if(matrix[i, j] != Double.PositiveInfinity)
                    {
                        (int, int) r1 = route.Where(t => t.Item2 == (int)matrix[i, 0]).First();
                        (int, int) r2 = route.Where(t => t.Item1 == (int)matrix[0, j]).First();
                        if((route.Where(t => t.Item2 == r1.Item1).Count() == 0 && route.Where(t => t.Item1 == (int)matrix[i, 0]).Count() == 0)
                        || (route.Where(t => t.Item1 == r2.Item2).Count() == 0 && route.Where(t => t.Item2 == (int)matrix[0, j]).Count() == 0))
                        {
                            route.Add(((int)matrix[i, 0], (int)matrix[0, j]));
                        }
                    }
                }
            }
            
            List<(int, int)> sortedRoute = new List<(int, int)>();
            sortedRoute.Add(route[0]);
            for(int i = 1; i < route.Count; i++)
            {
                (int, int) currentRoute = route.Where(t => t.Item1 == sortedRoute[i-1].Item2).First();
                sortedRoute.Add(currentRoute);
            }
            return sortedRoute;
        }
        private void ExecuteReduction()
        {
            double[] minInRows = FindMinInEveryRow(true);
            for(int i = 1; i < matrix.GetLength(0); i++)
            {
                for(int j = 1; j < matrix.GetLength(1); j++)
                {
                    matrix[i,j] -= minInRows[i-1]; 
                }
            }
            double[] minInCols = FindMinInEveryCol(true);
            for(int i = 1; i < this.matrix.GetLength(1); i++)
            {
                for(int j = 1; j < this.matrix.GetLength(0); j++)
                {
                    matrix[j,i] -= minInCols[i-1];
                }
            }
        }
        private void FindBranchingEdge()
        {
            // finding best route (depSt - arrSt)
            (int, int) bestRoute = FindMaxZeroDegree();
            
            int departureStationPos = bestRoute.Item1;
            int arrivalStationPos = bestRoute.Item2;

            //(depSt - arrSt)
            route.Add(((int)matrix[departureStationPos, 0], (int)matrix[0, arrivalStationPos])); 

            //finding reverse route (arrSt - depSt) 
            int departureInColumns = 0;
            int arrivalInRows = 0;
            for(int i = 0; i < matrix.GetLength(0); i++)
            {
                if(matrix[i, 0] == matrix[0, arrivalStationPos])
                {
                    arrivalInRows = i;
                }
                if(matrix[0, i] == matrix[departureStationPos, 0])
                {
                    departureInColumns = i;
                }
            }

            // breaking the loop by putting inf cost on reverse route
            if(arrivalInRows != 0 && departureInColumns != 0)
            {
                matrix[arrivalInRows, departureInColumns] = Double.PositiveInfinity;
            }

            // removing found route (depSt - arrSt)
            double[,] currMatrixCopy = matrix;
            matrix = new double[currMatrixCopy.GetLength(0)-1,currMatrixCopy.GetLength(1)-1];
            for(int i = 0, k = 0; i < currMatrixCopy.GetLength(0); i++)
            {
                if(i == departureStationPos) continue;
                for(int j = 0, v = 0; j < currMatrixCopy.GetLength(1); j++)
                {
                    if(j == arrivalStationPos) continue;
                    matrix[k,v] = currMatrixCopy[i,j];
                    v++;
                }
                k++;
            }
        }
        private (int, int) FindMaxZeroDegree()
        {
            double[] minInRows = FindMinInEveryRow(false);
            double[] minInCols = FindMinInEveryCol(false);
            int departureStationPos = 0, arrivalStationPos = 0;
            double maxConstSum = 0;
            for(int i = 1; i < matrix.GetLength(0); i++)
            {
                for(int j = 1; j < matrix.GetLength(0); j ++)
                {
                    if(matrix[i,j] == 0 && (minInRows[i-1] + minInCols[j-1] > maxConstSum))
                    {
                        maxConstSum = minInRows[i-1] + minInCols[j-1];
                        departureStationPos = i;
                        arrivalStationPos = j;
                    }
                }
            }
            return (departureStationPos, arrivalStationPos);
        }
        private double[] FindMinInEveryRow(bool includeZero)
        {
            double[] minInRows = new double[this.matrix.GetLength(0)-1];
            for(int i = 1; i < this.matrix.GetLength(0); i++)
            {
                double minInRow = double.PositiveInfinity;
                for(int j = 1; j < this.matrix.GetLength(1); j++)
                {
                    if(matrix[i,j] < minInRow)
                    {
                        if(matrix[i,j] != 0)
                        {
                            minInRow = matrix[i,j];
                        }
                        else
                        {
                            if(includeZero) minInRow = matrix[i,j];
                        }
                    }
                }
                if(minInRow != double.PositiveInfinity)
                {
                    minInRows[i-1] = minInRow;
                }
            }
            return minInRows;
        }
        private double[] FindMinInEveryCol(bool includeZero)
        {
            double[] minInCols = new double[this.matrix.GetLength(1)-1];
            for(int i = 1; i < this.matrix.GetLength(1); i++)
            {
                double minInCol = double.PositiveInfinity;
                for(int j = 1; j < this.matrix.GetLength(0); j++)
                {
                    if(matrix[j,i] < minInCol)
                    {
                        if(matrix[j,i] != 0)
                        {
                            minInCol = matrix[j,i];
                        }
                        else
                        {
                            if(includeZero) minInCol = matrix[j,i];
                        }
                    }
                }
                if(minInCol != double.PositiveInfinity)
                {
                    minInCols[i-1] = minInCol;
                }
            }
            return minInCols;
        }
    }
}