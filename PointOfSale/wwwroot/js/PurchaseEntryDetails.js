
$(document).on("click", ".btn-toggle-detail", function () {
    let id = $(this).data("id");

    let desktopRow = $("#detail-row-" + id);
    let desktopPanel = $("#detail-panel-" + id);
    let mobWrap = $("#mob-detail-" + id);
    let mobPanel = $("#mob-detail-panel-" + id);

    let isOpen = desktopRow.is(":visible") || mobWrap.is(":visible");

    $(".detail-row").hide();
    $(".mob-detail-wrap").hide();
    $(".btn-toggle-detail").html('<i class="ti ti-eye" aria-hidden="true"></i> View');

    if (isOpen) return;


    desktopRow.show();
    mobWrap.show();
    $(this).html('<i class="ti ti-eye-off" aria-hidden="true"></i> Hide');


    if (desktopPanel.data("loaded")) return;

    $.ajax({
        url: '/Purchase/AllPurchaseEntryDetailList/' + id,
        type: 'GET',
        success: function (response) {
            let items = response.data;

            if (!items || items.length === 0) {
                let empty = `<div class="detail-empty">
    <i class="ti ti-inbox" aria-hidden="true"></i>
    No items found.
    </div>`;
                desktopPanel.html(empty);
                mobPanel.html(empty);
                return;
            }

            let rows = '';
            let grandTotal = 0;

            items.forEach(function (item, index) {
                grandTotal += item.totalAmount;
                rows += `
    <tr>
    <td>${index + 1}</td>
    <td><strong>${item.productName ?? '-'}</strong></td>
    <td>${item.unitName ?? '-'}</td>
    <td>${item.quantity}</td>
    <td>₹ ${parseFloat(item.rate).toFixed(2)}</td>
    <td><span class="gst-badge">${item.gstPercent}%</span></td>
    <td>₹ ${parseFloat(item.subTotal).toFixed(2)}</td>
    <td>₹ ${parseFloat(item.gstAmount).toFixed(2)}</td>
    <td class="detail-total">₹ ${parseFloat(item.totalAmount).toFixed(2)}</td>
    </tr>`;
            });

            let html = `
    <div class="detail-header">
    <i class="ti ti-list-details" aria-hidden="true"></i>
    Purchase items
    </div>
    <div class="detail-table-wrap">
    <table class="detail-table">
    <thead>
    <tr>
    <th>#</th>
    <th>Product</th>
    <th>Unit</th>
    <th>Qty</th>
    <th>Rate (₹)</th>
    <th>GST %</th>
    <th>Sub total (₹)</th>
    <th>GST amt (₹)</th>
    <th>Total (₹)</th>
    </tr>
    </thead>
    <tbody>${rows}</tbody>
    <tfoot>
    <tr>
    <td colspan="8" class="detail-foot-label">Grand total</td>
    <td class="detail-foot-val">₹ ${grandTotal.toFixed(2)}</td>
    </tr>
    </tfoot>
    </table>
    </div>`;

            desktopPanel.html(html).data("loaded", true);
            mobPanel.html(html).data("loaded", true);
        },
        error: function () {
            let err = `<div class="detail-empty">
    <i class="ti ti-wifi-off" aria-hidden="true"></i>
    Failed to load details.
    </div>`;
            desktopPanel.html(err);
            mobPanel.html(err);
        }
    });
});


function confirmDelete(id) {
    Swal.fire({
        title: 'Delete purchase?',
        text: 'This action cannot be undone.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#c0392b',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Purchase/DeletePurchaseEntry/' + id,
                type: 'DELETE',
                success: function (response) {
                    if (response.status) {
                        Swal.fire({
                            icon: 'success',
                            title: 'Deleted!',
                            text: response.message,
                            timer: 1500,
                            showConfirmButton: false
                        }).then(() => location.reload());
                    } else {
                        Swal.fire({ icon: 'error', title: 'Failed', text: response.message });
                    }
                },
                error: function () {
                    Swal.fire({ icon: 'error', title: 'Error', text: 'Something went wrong.' });
                }
            });
        }
    });
}