namespace trains_test_task
{
    public class RouteInfo
    {
        public List<(int, int, int)> route;
        public int generalTime;
        public RouteInfo(int t,  List<(int, int, int)> r)
        {
            this.route = r;
            this.generalTime = t;
        }
    }
}