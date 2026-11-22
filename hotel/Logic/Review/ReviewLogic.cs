public class ReviewLogic
{
    public List<ReviewErrors> ValidateReview(
        int reservationId,
        int? rating,
        string? feedback,
        DateTime today
    )
    {
        AccountModel? account = AccountsLogic.CurrentAccount;
        if (account == null)
            return new List<ReviewErrors> { ReviewErrors.LoginRequired };

        ReservationModel? reservation = ReservationsAccess.GetById(reservationId);
        bool hasReview = ReviewAccess.GetByReservationId(reservationId) != null;
        var errors = ReviewValidator.ValidateStay(reservation, hasReview, today);
        errors.AddRange(ReviewValidator.ValidateRating(rating));
        errors.AddRange(ReviewValidator.ValidateFeedback(feedback));
        return errors;
    }

    public (ReviewModel? Review, List<ReviewErrors> Errors) SubmitReview(
        int reservationId,
        int? rating,
        string? feedback,
        DateTime now
    )
    {
        var errors = ValidateReview(reservationId, rating, feedback, now);
        if (errors.Count > 0)
            return (null, errors);

        var review = new ReviewModel
        {
            ReservationId = reservationId,
            Rating = rating!.Value,
            Feedback = string.IsNullOrWhiteSpace(feedback) ? null : feedback.Trim(),
            CreatedAt = now,
        };

        review.ReviewId = ReviewAccess.InsertReview(review);
        return (review, errors);
    }
    public ReviewModel? GetByReservationId(int reservationId)
    {
        AccountModel? account = AccountsLogic.CurrentAccount;
        if (account == null || account.Role != "guest")
            return null;

        return ReviewAccess.GetByReservationId(reservationId);
    }

    public List<ReviewModel> GetAll()
    {
        if (!new AdminLogic().IsAdmin(AccountsLogic.CurrentAccount))
            return new List<ReviewModel>();

        return ReviewAccess.GetAll();
    }
}
