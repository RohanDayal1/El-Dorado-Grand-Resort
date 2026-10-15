public static class ReservationMenu
{
	private const string Border = "============================================================";

	public static void ShowReservations(AccountModel account)
	{
		Console.Clear();
		Console.WriteLine(Border);
		Console.WriteLine($"  EL DORADO GRAND RESORT  |  RESERVATIONS FOR {(account.FirstName + " " + account.LastName).ToUpperInvariant()}");
		Console.WriteLine(Border);
		Console.WriteLine();

		List<ReservationModel> reservations = new ReservationLogic()
			.GetByUserId(account.UserId)
			.ToList();

		if (reservations.Count == 0)
		{
			Console.WriteLine("  No reservations were found for your account.");
		}
		else
		{
			foreach (ReservationModel reservation in reservations)
			{
				Console.WriteLine($"  Reservation ID:   {reservation.ReservationId}");
				Console.WriteLine($"  Check-in:         {reservation.CheckInDate:yyyy-MM-dd}");
				Console.WriteLine($"  Check-out:        {reservation.CheckOutDate:yyyy-MM-dd}");
				Console.WriteLine($"  Status:           {reservation.Status}");
				Console.WriteLine($"  Total:            {reservation.TotalCost:C}");
				Console.WriteLine();
			}
		}

		Console.WriteLine(Border);
		Console.Write("  Press Enter to return to your home menu...");
		Console.ReadLine();
	}
}
