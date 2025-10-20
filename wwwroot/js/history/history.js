$(document).ready(function () {
    $(document).on('click', '.details-btn', function () {
        // Get data attributes from the button
        const companyCode = $(this).data('companycode');
        const industryType = $(this).data('industrytype');
        const voucherCode = $(this).data('vouchercode');
        const issuedDate = $(this).data('issueddate');
        const companyname = $(this).data('company');

        // Create a form dynamically
        var form = $('<form>', {
            action: '/orderdetail',
            method: 'get'
        });

        // Add hidden input fields with the data
        form.append($('<input>', { type: 'hidden', name: 'CompanyName', value: companyname }));
        form.append($('<input>', { type: 'hidden', name: 'CompanyCode', value: companyCode }));
        form.append($('<input>', { type: 'hidden', name: 'IssuedDate', value: issuedDate }));
        form.append($('<input>', { type: 'hidden', name: 'IndustryType', value: industryType }));
        form.append($('<input>', { type: 'hidden', name: 'VoucherCode', value: voucherCode }));

        // Append the form to the body and submit it
        $('body').append(form);
        form.submit();
    });

    const phone = window.phoneNumber;
    let currentOrdersPage = 1;
    let currentPayemntsPage = 1;

    document.getElementById("load-more-payments").addEventListener("click", function () {
        currentPayemntsPage++;
        loadNextPage(currentPayemntsPage, "getpaymenthistory", "payment-history", "loading-indicator", "load-more-payments");
    });
    document.getElementById("load-more-orders").addEventListener("click", function () {
        currentOrdersPage++;
        loadNextPage(currentOrdersPage, "gethistory", "orders-history", "orders-loading-indicator", "load-more-orders");
    });
    function loadNextPage(page, endPoint, containerId, spinner, selectedButton) {
        const button = document.getElementById(selectedButton);
        const loader = document.getElementById(spinner);

        button.disabled = true;
        loader.style.display = "block";

        fetch(`/History/GetPaymentHistoryPartial?endPoint=${endPoint}&phone=${phone}&page=${page}`)
            .then(response => {
                if (!response.ok) throw new Error("Failed to load data");
                return response.text();
            })
            .then(html => {
                if (!html.trim()) {
                    button.remove();
                } else {
                    const container = document.getElementById(containerId);
                    container.insertAdjacentHTML("beforeend", html);
                    button.disabled = false;
                }
            })
            .catch(error => {
                console.error("Error loading more history:", error);
            })
            .finally(() => {
                loader.style.display = "none";
            });
    }
});