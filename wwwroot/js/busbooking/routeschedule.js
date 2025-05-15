document.addEventListener("DOMContentLoaded", function () {
    $(document).ready(function () {
        $(".book").click(function () {
            var form = $('<form action="/busseatlayout" method="post"></form>');

            var plateNumber = $(this).closest('.card-body').find('.plateNumber').text().trim();
            var terminal = $(this).closest('.card-body').find('.terminal').text().trim();
            var distance = $(this).closest('.card-body').find('.distance').text().trim();
            var routeSchedule = $(this).closest('.card-body').find('.routeSchedule').text().trim();
            var tariff = $(this).closest('.card-body').find('.tariff').text().trim();
            var level = $(this).closest('.card-body').find('.level').text().trim();
            var route = $(this).closest('.card-body').find('.route').text().trim();
            var operatorName = $(this).closest('.card-body').find('.operatorName').text().trim();
            var vehicleOperatorId = $(this).closest('.card-body').find('.vehicleOperatorId').text().trim();
            var scheduleDate = $(this).closest('.card-body').find('.scheduleDate').text().trim();
            var scheduleTime = $(this).closest('.card-body').find('.scheduleTime').text().trim();
            var destinationCity = $(this).closest('.card-body').find('.destinationCity').text().trim();
            var depatureCity = $(this).closest('.card-body').find('.depatureCity').text().trim();
            var arrivalDate = $(this).closest('.card-body').find('.arrivalDate').text().trim();
            var departureDate = $(this).closest('.card-body').find('.departureDate').text().trim();
            var vehicle = $(this).closest('.card-body').find('.vehicle').text().trim();
            var destinationTermianl = $(this).closest('.card-body').find('.destinationTermianl').text().trim();
            var originTerminalName = $(this).closest('.card-body').find('.originTerminalName').text().trim();
            var via = $(this).closest('.card-body').find('.via').text().trim();
            var sheduleId = $(this).closest('.card-body').find('.sheduleId').text().trim();

            form.append('<input type="hidden" name="plateNumber" value="' + plateNumber + '">');
            form.append('<input type="hidden" name="terminal" value="' + terminal + '">');
            form.append('<input type="hidden" name="level" value="' + level + '">');
            form.append('<input type="hidden" name="distance" value="' + distance + '">');
            form.append('<input type="hidden" name="tariff" value="' + tariff + '">');
            form.append('<input type="hidden" name="route" value="' + route + '">');
            form.append('<input type="hidden" name="routeSchedule" value="' + routeSchedule + '">');
            form.append('<input type="hidden" name="operatorName" value="' + operatorName + '">');
            form.append('<input type="hidden" name="scheduleDate" value="' + scheduleDate + '">');
            form.append('<input type="hidden" name="scheduleTime" value="' + scheduleTime + '">');
            form.append('<input type="hidden" name="destinationCity" value="' + destinationCity + '">');
            form.append('<input type="hidden" name="depatureCity" value="' + depatureCity + '">');
            form.append('<input type="hidden" name="arrivalDate" value="' + arrivalDate + '">');
            form.append('<input type="hidden" name="departureDate" value="' + departureDate + '">');
            form.append('<input type="hidden" name="vehicleOperatorId" value="' + vehicleOperatorId + '">');
            form.append('<input type="hidden" name="vehicle" value="' + vehicle + '">');
            form.append('<input type="hidden" name="destinationTermianl" value="' + destinationTermianl + '">');
            form.append('<input type="hidden" name="originTerminalName" value="' + originTerminalName + '">');
            form.append('<input type="hidden" name="via" value="' + via + '">');
            form.append('<input type="hidden" name="sheduleId" value="' + sheduleId + '">');
            $("body").append(form);
            form.submit();
        });


        document.querySelectorAll('.busAmenities input[type= "checkbox"]').forEach(checkbox => {
            checkbox.addEventListener('change', filterSchedules);
        });

        function filterSchedules() {
            const checkedAmenities = Array.from(document.querySelectorAll('.busAmenities input[type="checkbox"]:checked')).map(cb => cb.value.toLowerCase());
            const cards = document.querySelectorAll('.schedules .card');
            cards.forEach(card => {
                const cardAmenities = Array.from(card.querySelectorAll('[data-amenity]')).map(span => span.dataset.amenity?.toLowerCase());
                const showCard = checkedAmenities.every(amenity => cardAmenities.includes(amenity));
                card.style.display = showCard ? "block" : "none";
            });
        }
    });
});