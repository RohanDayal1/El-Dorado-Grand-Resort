public static class ReviewMenu
{
    public static void Start(ReservationModel reservation)
    {
        var logic = new ReviewLogic();
        ConsoleHelper.ShowHeader("YOUR REVIEW");
        Console.WriteLine($"  Reservation code: {reservation.ReservationCode}\n");

        ReviewModel? existingReview = logic.GetByReservationId(reservation.ReservationId);
        if (existingReview != null)
        {
            ShowReview(existingReview);
            Console.WriteLine(
                "  You have already reviewed this stay. Reviews cannot be changed or deleted."
            );
            ConsoleHelper.WaitForEnter();
            return;
        }

        var errors = logic
            .ValidateReview(reservation.ReservationId, null, null, DateTime.Today)
            .Where(error => error != ReviewErrors.RatingRequired)
            .ToList();
        if (errors.Count > 0)
        {
            ShowValidationErrors(errors);
            ConsoleHelper.WaitForEnter();
            return;
        }

        Console.WriteLine("  Rate your overall stay from 1 to 5. Press Esc to cancel.\n");
        int? rating = ReadRating();
        if (rating == null)
            return;

        var feedback = ReadFeedback();
        if (feedback == null)
            return;

        var details =
            $"  Reservation code: {reservation.ReservationCode}\n"
            + $"  Rating:           {rating}/5\n"
            + $"  Feedback:         {(feedback.Length == 0 ? "No written feedback." : feedback)}\n";

        int choice = ConsoleHelper.SelectionMenu(
            new List<string> { "Submit review", "Cancel" },
            "CONFIRM YOUR REVIEW",
            "Reviews cannot be changed or deleted",
            information: details
        );

        if (choice != 0)
            return;

        var result = logic.SubmitReview(reservation.ReservationId, rating, feedback, DateTime.Now);
        if (result.Errors.Count > 0)
        {
            ShowValidationErrors(result.Errors);
            ConsoleHelper.WaitForEnter();
            return;
        }

        if (result.Review != null)
        {
            ConsoleHelper.ShowHeader("REVIEW SUBMITTED");
            Console.WriteLine("  Thank you! Your review has been saved.\n");
            ShowReview(result.Review);
            ConsoleHelper.WaitForEnter();
        }
    }

    public static void ShowAllReviews()
    {
        ConsoleHelper.ShowHeader("GUEST REVIEWS");
        List<ReviewModel> reviews = new ReviewLogic().GetAll();
        if (reviews.Count == 0)
            Console.WriteLine("  No reviews have been submitted yet.");

        foreach (ReviewModel review in reviews)
        {
            Console.WriteLine($"  Guest:            {review.FirstName} {review.LastName}");
            Console.WriteLine($"  Email:            {review.Email}");
            Console.WriteLine($"  Reservation code: {review.ReservationCode}");
            ShowReview(review);
            Console.WriteLine("------------------------------------------------------------");
        }

        ConsoleHelper.WaitForEnter();
    }

    private static int? ReadRating()
    {
        while (true)
        {
            int? rating = ReservationInput.ReadInt("Rating (1-5): ");
            if (rating == null)
                return null;

            var errors = ReviewValidator.ValidateRating(rating);
            if (errors.Count == 0)
                return rating;

            ShowValidationErrors(errors);
        }
    }

    private static string? ReadFeedback()
    {
        while (true)
        {
            var feedback = ReservationInput.ReadText(
                "Feedback (optional, max 1,000 characters; Enter to skip): "
            );
            if (feedback == null)
                return null;

            var errors = ReviewValidator.ValidateFeedback(feedback);
            if (errors.Count == 0)
                return feedback;

            ShowValidationErrors(errors);
        }
    }

    private static void ShowReview(ReviewModel review)
    {
        Console.WriteLine($"  Rating:           {review.Rating}/5");
        Console.WriteLine($"  Feedback:         {review.Feedback ?? "No written feedback."}");
        Console.WriteLine($"  Submitted:        {review.CreatedAt:yyyy-MM-dd HH:mm}\n");
    }

    private static void ShowValidationErrors(List<ReviewErrors> errors)
    {
        foreach (ReviewErrors error in errors)
        {
            string message = error switch
            {
                ReviewErrors.LoginRequired => "Please log in to leave a review.",
                ReviewErrors.ReservationNotFound => "The reservation was not found.",
                ReviewErrors.ReservationNotConfirmed =>
                    "Cancelled reservations cannot be reviewed.",
                ReviewErrors.StayNotCompleted =>
                    "You can leave a review from the day after checkout.",
                ReviewErrors.ReviewAlreadySubmitted => "You have already reviewed this stay.",
                ReviewErrors.RatingRequired => "Please enter a rating.",
                ReviewErrors.RatingOutOfRange => "Choose a rating from 1 to 5.",
                ReviewErrors.FeedbackTooLong => "Feedback cannot exceed 1,000 characters.",
                _ => "Please check your review details.",
            };
            Console.WriteLine($"  {message}");
        }
    }
}
