$(document).ready(function () {
    var companyName;
    var branchCode;
    var article;
    $('#reviewModal').on('show.bs.modal', function (event) {
        console.log("hey")
        const button = $(event.relatedTarget); // Button that triggered the modal
        console.log(button)
        companyName = button.data('company'); // Extract info from data-* attributes
        article = button.data('article'); // Extract info from data-* attributes
        branchCode = button.data('branchcode'); // Extract info from data-* attributes

        const modal = $(this);
        modal.find('.modal-title').text(companyName ?? "Company Review"); // Set the company name to the modal label

        const stars = modal.find('#starRating i');
        stars.removeClass('fas text-warning').addClass('far');
        modal.find('#reviewText').val(''); // Clear the text area
    });

    $(document).on('click', '#reviewModal #starRating i', function () {
        console.log("he")
        const stars = $(this).parent().find('i');
        const rating = stars.index($(this)) + 1;

        // Reset all stars
        stars.removeClass('bi-star-fill text-warning').addClass('bi-star');

        // Highlight selected stars
        stars.slice(0, rating).removeClass('bi-star').addClass('bi-star-fill text-warning');
    });

    $('#reviewModal').on('click', '.submit', function (e) {
        e.preventDefault();
        const stars = $('#starRating i.fas');
        const rating = stars.length;
        const reviewText = $('#reviewText').val().trim();
        const companyName = $('#companyNameInput').val();

        if (rating === 0) {
            alert('Please select a star rating before submitting.');
            return;
        }

        const data = JSON.stringify({
            branchCode: branchCode,
            rating: rating,
            review: reviewText,
            article: article
        });

        $.ajax({
            type: 'POST',
            contentType: 'application/json',
            url: '/history/SubmitRating',
            data: data,
            cache: false,
            success: function (response) {
                if (response.isSuccessful) {
                    alert('Review submitted successfully.');
                } else {
                    alert(response.errorMessages.join(', '));
                }
            },
            error: function (error) {
                $('#reviewModal').modal('hide');
                alert('An error occurred while submitting your review. Please try again.');
            }
        });

        return false;
    });
    // Event listener to reset modal content when modal is hidden
    $('#reviewModal').on('hidden.bs.modal', function () {
        $('#reviewText').val(''); // Clear review text input
        $('#starRating i.fas').removeClass('fas text-warning').addClass('far'); // Reset star rating
    });
});