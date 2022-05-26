namespace trains_test_task
{
    public class Passage
    {
        public int trainNumber;
        public int departureStation;
        public int arrivalStation;
        public double cost;
        public DateTime departureTime;
        public DateTime arrivalTime;
        public Passage(int trainNum, int depSt, int arrSt, double cost, DateTime depTime, DateTime arrTime)
        {
            this.trainNumber = trainNum;
            this.departureStation = depSt;
            this.arrivalStation = arrSt;
            this.cost = cost;
            this.departureTime = depTime;
            this.arrivalTime = arrTime;
        }
    }
}