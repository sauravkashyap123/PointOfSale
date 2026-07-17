
    $(document).ready(function () {

        $.ajax({
            url: '/Purchase/AllPurchaseProductList',
            type: 'GET',
            success: function (res) {

                let rows = '';
                let cards = '';
                let i = 1;

                res.data.forEach(function (item) {

                    let date = item.createdDate
                        ? new Date(item.createdDate).toLocaleDateString('en-IN', { day: '2-digit', month: 'short', year: 'numeric' })
                        : '—';

                    let warehouse = item.warehouseName ?? '—';
                    let category = item.categoryName ?? '—';
                    let code = item.purchaseProductCode ?? '—';

                    rows += `
    <tr>
    <td class="sno">${i}</td>
    <td style="font-weight:500;">${item.productName ?? ''}</td>
    <td><span class="pp-code">${code}</span></td>
    <td><span class="badge-cat">${category}</span></td>
    <td><span class="badge-wh">${warehouse}</span></td>
    <td style="color:#6b7280; font-size:13px;">${date}</td>
    <td>
    <a href="/Purchase/AddPurchaseProduct/${item.id}" class="btn-action">
    <i class="ti ti-edit" style="font-size:14px;"></i> Edit
    </a>
    </td>
    </tr>`;

                    cards += `
    <div class="m-card-box">
    <div class="m-card-name">${item.productName ?? ''}</div>
    <div class="m-card-row"><b>Code</b> <span class="pp-code">${code}</span></div>
    <div class="m-card-row"><b>Category</b> <span class="badge-cat">${category}</span></div>
    <div class="m-card-row"><b>Warehouse</b> <span class="badge-wh">${warehouse}</span></div>
    <div class="m-card-row"><b>Date</b> ${date}</div>
    <div class="m-card-actions">
    <a href="/Purchase/AddPurchaseProduct/${item.id}" class="btn-action">
    <i class="ti ti-edit" style="font-size:14px;"></i> Edit
    </a>
    </div>
    </div>`;

                    i++;
                });

                $('#tblBody').html(rows);
                $('#mobileList').html(cards);

                if ($.fn.DataTable.isDataTable('#purchaseTable')) {
                    $('#purchaseTable').DataTable().destroy();
                }

                $('#purchaseTable').DataTable({
                    paging: true,
                    searching: true,
                    ordering: true,
                    pageLength: 10,
                    language: {
                        search: "",
                        searchPlaceholder: "Search products…"
                    }
                });
            }
        });
    });
