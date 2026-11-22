public static class ReviewValidator
{
    public static List<ReviewErrors> ValidateStay(
        ReservationModel? reservation,
        bool hasReview,
        DateTime today
    )
    {
        var errors = new List<ReviewErrors>();

        if (reservation == null)
        {
            errors.Add(ReviewErrors.ReservationNotFound);
            return errors;
        }

        if (reservation.Status != "confirmed")
            errors.Add(ReviewErrors.ReservationNotConfirmed);

        if (reservation.CheckOutDate.Date >= today.Date)
            errors.Add(ReviewErrors.StayNotCompleted);

        if (hasReview)
            errors.Add(ReviewErrors.ReviewAlreadySubmitted);

        return errors;
    }

    public static List<ReviewErrors> ValidateRating(int? rating)
    {
        var errors = new List<ReviewErrors>();

        if (rating == null)
            errors.Add(ReviewErrors.RatingRequired);
        else if (rating < 1 || rating > 5)
            errors.Add(ReviewErrors.RatingOutOfRange);

        return errors;
    }

    public static List<ReviewErrors> ValidateFeedback(string? feedback)
    {
        var errors = new List<ReviewErrors>();

        if (feedback != null && feedback.Trim().Length > 1000)
            errors.Add(ReviewErrors.FeedbackTooLong);

        return errors;
    }
}
