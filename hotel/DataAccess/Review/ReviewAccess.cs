using Dapper;

public static class ReviewAccess
{
    public static int InsertReview(ReviewModel review)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();

        return connection.QuerySingle<int>(
            @"
            INSERT INTO Review (Reservation_Id, Rating, Feedback, CreatedAt)
            VALUES (@ReservationId, @Rating, @Feedback, @CreatedAt);
            SELECT last_insert_rowid();",
            new
            {
                review.ReservationId,
                review.Rating,
                review.Feedback,
                CreatedAt = review.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            }
        );
    }

    public static ReviewModel? GetByReservationId(int reservationId)
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();

        return connection.QueryFirstOrDefault<ReviewModel>(
            @"
            SELECT Review_Id AS ReviewId, Reservation_Id AS ReservationId,
                   Rating, Feedback, CreatedAt
            FROM Review
            WHERE Reservation_Id = @ReservationId;",
            new { ReservationId = reservationId }
        );
    }

    public static List<ReviewModel> GetAll()
    {
        using var connection = DatabaseInitializer.Connect();
        connection.Open();
        return connection.Query<ReviewModel>(
            @"
            SELECT rv.Review_Id AS ReviewId, rv.Reservation_Id AS ReservationId,
                   rv.Rating, rv.Feedback, rv.CreatedAt, r.ReservationCode,
                   u.First_Name AS FirstName, u.Last_Name AS LastName, u.Email
            FROM Review rv
            JOIN Reservation r ON r.Reservation_Id = rv.Reservation_Id
            JOIN User u ON u.User_Id = r.User_Id
            ORDER BY rv.CreatedAt DESC, rv.Review_Id DESC;"
        ).ToList();
    }
}
