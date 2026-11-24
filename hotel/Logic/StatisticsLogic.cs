public class StatisticsLogic
{
    public ResortStatisticsModel GetResortStatistics(DateTime? today = null)
    {
        return StatisticsAccess.GetResortStatistics(today ?? DateTime.Today);
    }
}
