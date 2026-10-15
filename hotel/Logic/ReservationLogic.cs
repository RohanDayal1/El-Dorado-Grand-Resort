public class ReservationLogic
{
	public IEnumerable<ReservationModel> GetByUserId(int userId)
	{
		return ReservationsAccess.GetByUserId(userId);
	}
}
