function ToggleBtn() {
    if ($('.toggleBtn').parent('tr').next('tr').is(":visible")) {
        $('.toggleBtn').removeClass("fa fa-minus-square")
        $('.toggleBtn').addClass("fa fa-plus-square")
        $('.toggleBtn').parent('tr').next('tr').toggle("fast", function () {
        });
    }
    else {
        $('.toggleBtn').removeClass("fa fa-plus-square")
        $('.toggleBtn').addClass("fa fa-minus-square")
        $('.toggleBtn').parent('tr').next('tr').toggle("fast", function () {
        });
    }
}
ToggleBtn();
$('.toggleBtn').on('click', function () {
    if ($(this).parent('tr').next('tr').is(":visible")) {
        $(this).removeClass("fa fa-minus-square")
        $(this).addClass("fa fa-plus-square")
    }
    else {
        $(this).removeClass("fa fa-plus-square")
        $(this).addClass("fa fa-minus-square")
    }
    $(this).parent('tr').next('tr').toggle("fast", function () {
    });
})