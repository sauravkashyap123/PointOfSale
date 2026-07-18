var table;
var allData = [];

function loadData() {
    var shopid = $("#shopid").val();
    if (!shopid) {
        showAlert('Pehle shop select karein.');
        return;
    }

    $(".sh-detail-row").remove();

    if ($.fn.DataTable.isDataTable('#tblSaleHistory')) {
        $('#tblSaleHistory').DataTable().destroy();
    }

    table = $("#tblSaleHistory").DataTable({
        processing: true,
        language: {
            processing: "Records load ho rahe hain…",
            emptyTable: "Is shop ka koi sale record nahi mila.",
            zeroRecords: "Koi record filter se match nahi kiya."
        },
        ajax: {
            url: '/Billing/AllItemShopSaleList',
            type: 'GET',
            data: { shopid: shopid },
            dataSrc: function (json) {
                allData = json.data || [];

                var grouped = {};
                allData.forEach(function (row) {
                    var key = row.invoiceNumber;
                    if (!grouped[key]) {
                        grouped[key] = {
                            invoiceNumber: row.invoiceNumber,
                            invoiceDate: row.invoiceDate,
                            shopName: row.shopName,
                            customerName: row.customerName,
                            mobileNo: row.mobileNo,
                            totalAmount: 0,
                            items: []
                        };
                    }
                    grouped[key].totalAmount += parseFloat(row.totalAmount || 0);
                    grouped[key].items.push(row);
                });

                return Object.values(grouped);
            },
            error: function () {
                showAlert('Data load karne mein error aaya. Dobara try karein.');
            }
        },
        columns: [
            {
                data: null,
                orderable: false,
                searchable: false,
                render: function (data, type, row, meta) {
                    return meta.row + 1;
                }
            },
            {
                data: 'invoiceNumber',
                defaultContent: '—',
                render: function (data, type) {
                    if (type === 'display') return '<span class="inv-no">' + (data || '—') + '</span>';
                    return data;
                }
            },
            {
                data: 'invoiceDate',
                defaultContent: '—',
                render: function (data, type) {
                    if (!data) return '—';
                    var d = new Date(data);
                    if (isNaN(d.getTime())) return data;
                    if (type === 'display') return d.toLocaleDateString('en-GB');
                    return d.toISOString();
                }
            },
            { data: 'shopName', defaultContent: '—' },
            { data: 'customerName', defaultContent: '—' },
            {
                data: 'mobileNo',
                defaultContent: '—',
                render: function (data, type) {
                    if (type === 'display' && data)
                        return '<span style="font-family:var(--mono);font-size:12.5px">' + data + '</span>';
                    return data || '—';
                }
            },
            {
                data: 'totalAmount',
                defaultContent: '0',
                render: function (data, type) {
                    if (type === 'display')
                        return '<span class="total-cell">₹' + (parseFloat(data) || 0).toLocaleString('en-IN', { minimumFractionDigits: 2, maximumFractionDigits: 2 }) + '</span>';
                    return data;
                }
            },
            {
                data: 'invoiceNumber',
                orderable: false,
                searchable: false,
                defaultContent: '—',
                render: function (data) {
                    if (!data) return '—';
                    return '<div style="display:flex;gap:8px;align-items:center">'
                        + '<button class="btn-view btn-toggle-detail" data-invoice="' + data + '">'
                        + '<svg viewBox="0 0 24 24"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg>'
                        + 'View</button>'
                        + '<button class="btn-print btn-print-invoice" data-invoice="' + data + '">'
                        + '<svg viewBox="0 0 24 24" width="13" height="13" stroke="currentColor" fill="none" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">'
                        + '<polyline points="6 9 6 2 18 2 18 9"/>'
                        + '<path d="M6 18H4a2 2 0 0 1-2-2v-5a2 2 0 0 1 2-2h16a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2h-2"/>'
                        + '<rect x="6" y="14" width="12" height="8"/></svg>'
                        + 'Print</button>'
                        + '</div>';
                }
            }
        ]
    });
}

// ===== TOGGLE DETAIL PANEL =====
$(document).on("click", ".btn-toggle-detail", function () {
    var invoice = $(this).data("invoice");
    var tr = $(this).closest("tr");
    var nextTr = tr.next(".sh-detail-row");

    $(".sh-detail-row").slideUp(150, function () { $(this).remove(); });
    $(".btn-toggle-detail").html(
        '<svg viewBox="0 0 24 24"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/><circle cx="12" cy="12" r="3"/></svg> View'
    );

    if (nextTr.length) return;

    var items = allData.filter(function (r) { return r.invoiceNumber === invoice; });

    var rows = '';
    var grandTotal = 0;
    items.forEach(function (item, idx) {
        var amt = parseFloat(item.amount || 0);
        var tot = parseFloat(item.totalAmount || 0);
        grandTotal += tot;
        rows += '<tr>'
            + '<td>' + (idx + 1) + '</td>'
            + '<td><strong>' + (item.productName || '—') + '</strong></td>'
            + '<td>' + (item.categoryName || '—') + '</td>'
            + '<td class="qty-cell">' + (parseFloat(item.quantity) || 0).toLocaleString('en-IN') + '</td>'
            + '<td class="amt-cell">₹' + amt.toLocaleString('en-IN', { minimumFractionDigits: 2 }) + '</td>'
            + '<td class="total-cell">₹' + tot.toLocaleString('en-IN', { minimumFractionDigits: 2 }) + '</td>'
            + '</tr>';
    });

    var detailHtml = '<tr class="sh-detail-row">'
        + '<td colspan="8" class="sh-detail-td">'
        + '<div class="sh-detail-panel">'
        + '<div class="sh-detail-header">'
        + '<svg viewBox="0 0 24 24" width="14" height="14" stroke="currentColor" fill="none" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">'
        + '<line x1="8" y1="6" x2="21" y2="6"/><line x1="8" y1="12" x2="21" y2="12"/>'
        + '<line x1="8" y1="18" x2="21" y2="18"/><line x1="3" y1="6" x2="3.01" y2="6"/>'
        + '<line x1="3" y1="12" x2="3.01" y2="12"/><line x1="3" y1="18" x2="3.01" y2="18"/>'
        + '</svg>'
        + 'Invoice Items — <span class="inv-no" style="font-size:11px">' + invoice + '</span>'
        + '</div>'
        + '<div class="sh-detail-table-wrap">'
        + '<table class="sh-detail-table">'
        + '<thead><tr><th>#</th><th>Product</th><th>Category</th><th>Qty</th><th>Amount (₹)</th><th>Total (₹)</th></tr></thead>'
        + '<tbody>' + rows + '</tbody>'
        + '<tfoot><tr>'
        + '<td colspan="5" class="sh-detail-foot-label">Grand Total</td>'
        + '<td class="sh-detail-foot-val">₹' + grandTotal.toLocaleString('en-IN', { minimumFractionDigits: 2 }) + '</td>'
        + '</tr></tfoot>'
        + '</table>'
        + '</div></div></td></tr>';

    $(detailHtml).insertAfter(tr).find(".sh-detail-panel").hide().slideDown(200);

    $(this).html(
        '<svg viewBox="0 0 24 24" width="13" height="13" stroke="currentColor" fill="none" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">'
        + '<path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94"/>'
        + '<path d="M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19"/>'
        + '<line x1="1" y1="1" x2="23" y2="23"/>'
        + '</svg> Hide'
    );
});

// // ===== PRINT INVOICE =====
// $(document).on("click", ".btn-print-invoice", function () {
//     var invoice = $(this).data("invoice");
//     var items = allData.filter(function (r) { return r.invoiceNumber === invoice; });
//     if (!items.length) return;

//     var info = items[0];
//     var grandTotal = 0;
//     var rows = '';

//     items.forEach(function (item, idx) {
//         var amt = parseFloat(item.amount || 0);
//         var tot = parseFloat(item.totalAmount || 0);
//         grandTotal += tot;
//         rows += '<tr>'
//             + '<td>' + (idx + 1) + '</td>'
//             + '<td>' + (item.productName || '—') + '</td>'
//             + '<td>' + (item.categoryName || '—') + '</td>'
//             + '<td style="text-align:center">' + (parseFloat(item.quantity) || 0).toLocaleString('en-IN') + '</td>'
//             + '<td style="text-align:right">₹' + amt.toLocaleString('en-IN', { minimumFractionDigits: 2 }) + '</td>'
//             + '<td style="text-align:right">₹' + tot.toLocaleString('en-IN', { minimumFractionDigits: 2 }) + '</td>'
//             + '</tr>';
//     });

//     var invoiceDate = '—';
//     if (info.invoiceDate) {
//         var d = new Date(info.invoiceDate);
//         if (!isNaN(d.getTime())) invoiceDate = d.toLocaleDateString('en-GB');
//     }

//     var printHtml = '<!DOCTYPE html>'
//         + '<html><head><meta charset="UTF-8">'
//         + '<title>Invoice — ' + invoice + '</title>'
//         + '<style>'
//         + '* { box-sizing: border-box; margin: 0; padding: 0; }'
//         + 'body { font-family: "Segoe UI", Arial, sans-serif; font-size: 13px; color: #1e293b; background: #fff; padding: 32px; }'
//         + '.inv-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 28px; padding-bottom: 16px; border-bottom: 2px solid #0C447C; }'
//         + '.inv-shop { font-size: 20px; font-weight: 700; color: #0C447C; margin-bottom: 4px; }'
//         + '.inv-sub { font-size: 12px; color: #64748b; }'
//         + '.inv-meta { text-align: right; }'
//         + '.inv-no { font-size: 16px; font-weight: 700; color: #0C447C; }'
//         + '.inv-date { font-size: 12px; color: #64748b; margin-top: 4px; }'
//         + '.cust-box { background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 12px 16px; margin-bottom: 24px; }'
//         + '.cust-box h3 { font-size: 10px; font-weight: 700; text-transform: uppercase; letter-spacing: .8px; color: #94a3b8; margin-bottom: 8px; }'
//         + '.cust-row { display: flex; gap: 40px; }'
//         + '.cust-field label { font-size: 11px; color: #94a3b8; display: block; margin-bottom: 2px; }'
//         + '.cust-field span { font-size: 13.5px; font-weight: 600; color: #1e293b; }'
//         + 'table { width: 100%; border-collapse: collapse; margin-bottom: 16px; }'
//         + 'thead tr { background: #0C447C; }'
//         + 'thead th { padding: 9px 12px; font-size: 11px; font-weight: 600; color: rgba(255,255,255,.85); text-transform: uppercase; letter-spacing: .5px; text-align: left; }'
//         + 'thead th:nth-child(4), thead th:nth-child(5), thead th:nth-child(6) { text-align: right; }'
//         + 'tbody td { padding: 9px 12px; border-bottom: 1px solid #e2e8f0; color: #1e293b; }'
//         + 'tbody tr:nth-child(even) td { background: #f8fafc; }'
//         + 'tfoot td { padding: 10px 12px; background: #f1f5f9; font-weight: 700; border-top: 2px solid #0C447C; }'
//         + '.foot-label { text-align: right; font-size: 12px; color: #475569; text-transform: uppercase; letter-spacing: .4px; }'
//         + '.foot-val { text-align: right; font-size: 15px; color: #0d9488; font-weight: 700; }'
//         + '.inv-footer { margin-top: 32px; padding-top: 12px; border-top: 1px solid #e2e8f0; font-size: 11px; color: #94a3b8; text-align: center; }'
//         + '@media print { body { padding: 16px; } button { display: none !important; } }'
//         + '</style></head>'
//         + '<body>'
//         + '<div class="inv-header">'
//         + '<div>'
//         + '<div class="inv-shop">' + (info.shopName || 'MithaiShop') + '</div>'
//         + '<div class="inv-sub">Sale Invoice</div>'
//         + '</div>'
//         + '<div class="inv-meta">'
//         + '<div class="inv-no"># ' + invoice + '</div>'
//         + '<div class="inv-date">Date: ' + invoiceDate + '</div>'
//         + '</div>'
//         + '</div>'
//         + '<div class="cust-box">'
//         + '<h3>Customer Details</h3>'
//         + '<div class="cust-row">'
//         + '<div class="cust-field"><label>Name</label><span>' + (info.customerName || '—') + '</span></div>'
//         + '<div class="cust-field"><label>Mobile</label><span>' + (info.mobileNo || '—') + '</span></div>'
//         + '</div></div>'
//         + '<table>'
//         + '<thead><tr>'
//         + '<th>#</th><th>Product</th><th>Category</th>'
//         + '<th style="text-align:right">Qty</th>'
//         + '<th style="text-align:right">Amount (₹)</th>'
//         + '<th style="text-align:right">Total (₹)</th>'
//         + '</tr></thead>'
//         + '<tbody>' + rows + '</tbody>'
//         + '<tfoot><tr>'
//         + '<td colspan="5" class="foot-label">Grand Total</td>'
//         + '<td class="foot-val">₹' + grandTotal.toLocaleString('en-IN', { minimumFractionDigits: 2 }) + '</td>'
//         + '</tr></tfoot>'
//         + '</table>'
//         + '<div class="inv-footer">Thank you for your purchase!</div>'
//         + '<script>window.onload = function() { window.print(); }<\/script>'
//         + '</body></html>';

//     var w = window.open('', '_blank', 'width=800,height=700');
//     w.document.write(printHtml);
//     w.document.close();
// });

// ===== PRINT INVOICE (Blob URL style) =====
$(document).on("click", ".btn-print-invoice", function () {
    var invoice = $(this).data("invoice");
    var items = allData.filter(function (r) { return r.invoiceNumber === invoice; });
    if (!items.length) {
        showAlert('Invoice ka data nahi mila.');
        return;
    }

    var info = items[0];
    var grandTotal = 0;
    var rows = '';

    items.forEach(function (item, idx) {
        var amt = parseFloat(item.amount || 0);
        var tot = parseFloat(item.totalAmount || 0);
        grandTotal += tot;
        rows += '<div class="bill-item-row">'
            + '<div class="bi-name">' + (item.productName || '—') + '<br><span style="font-size:9px;color:#aaa">' + (item.categoryName || '') + '</span></div>'
            + '<div class="bi-qty">' + (parseFloat(item.quantity) || 0).toLocaleString('en-IN') + '</div>'
            + '<div class="bi-amt">&#8377;' + tot.toLocaleString('en-IN', { minimumFractionDigits: 2 }) + '</div>'
            + '</div>';
    });

    var invoiceDate = '—';
    if (info.invoiceDate) {
        var d = new Date(info.invoiceDate);
        if (!isNaN(d.getTime())) invoiceDate = d.toLocaleDateString('en-GB');
    }

    var css = '* { box-sizing: border-box; margin: 0; padding: 0; }'
        + 'body { font-family: monospace; padding: 20px; font-size: 12px; color: #111; max-width: 380px; margin: 0 auto; }'
        + '.bill-top { text-align: center; padding-bottom: 12px; border-bottom: 2px dashed #ccc; margin-bottom: 12px; }'
        + '.bill-logo { font-size: 20px; font-weight: 700; }'
        + '.bill-store { font-size: 10px; color: #666; margin-top: 3px; }'
        + '.bill-meta { font-size: 10px; color: #666; margin-top: 3px; }'
        + '.bill-cust { padding: 8px 0; border-bottom: 1px dashed #ccc; margin-bottom: 10px; font-size: 11px; }'
        + '.bill-cust-row { display: flex; justify-content: space-between; padding: 2px 0; }'
        + '.bill-cust-label { color: #999; }'
        + '.bill-items-head { display: flex; font-size: 10px; font-weight: 700; color: #999; padding: 4px 0; border-bottom: 1px solid #ddd; margin-bottom: 5px; }'
        + '.bh-item { flex: 1; } .bh-qty { width: 34px; text-align: center; } .bh-amt { width: 72px; text-align: right; }'
        + '.bill-item-row { display: flex; font-size: 11px; padding: 4px 0; border-bottom: 1px dotted #eee; align-items: center; }'
        + '.bi-name { flex: 1; line-height: 1.3; } .bi-qty { width: 34px; text-align: center; color: #888; } .bi-amt { width: 72px; text-align: right; font-weight: 700; }'
        + '.bill-tots { padding-top: 9px; margin-top: 3px; border-top: 1px dashed #ccc; }'
        + '.bill-tot-row { display: flex; justify-content: space-between; font-size: 11px; padding: 3px 0; }'
        + '.bill-tot-row.grand { font-size: 14px; font-weight: 700; border-top: 2px dashed #ccc; margin-top: 5px; padding-top: 7px; }'
        + '.bill-footer { text-align: center; margin-top: 12px; padding-top: 10px; border-top: 2px dashed #ccc; font-size: 10px; color: #999; line-height: 1.7; }';

    var content = '<div class="bill-top">'
        + '<div class="bill-logo">' + (info.shopName || 'MithaiShop') + '</div>'
        + '<div class="bill-store">Sale Invoice</div>'
        + '<div class="bill-meta">Invoice: ' + invoice + ' &nbsp;|&nbsp; Date: ' + invoiceDate + '</div>'
        + '</div>'
        + '<div class="bill-cust">'
        + '<div class="bill-cust-row"><span class="bill-cust-label">Customer</span><span>' + (info.customerName || '—') + '</span></div>'
        + '<div class="bill-cust-row"><span class="bill-cust-label">Mobile</span><span>' + (info.mobileNo || '—') + '</span></div>'
        + '</div>'
        + '<div class="bill-items-head">'
        + '<div class="bh-item">Item</div>'
        + '<div class="bh-qty">Qty</div>'
        + '<div class="bh-amt">Amount</div>'
        + '</div>'
        + rows
        + '<div class="bill-tots">'
        + '<div class="bill-tot-row grand">'
        + '<span>Grand Total</span>'
        + '<span>&#8377;' + grandTotal.toLocaleString('en-IN', { minimumFractionDigits: 2 }) + '</span>'
        + '</div>'
        + '</div>'
        + '<div class="bill-footer">Thank you for your purchase!<br>' + (info.shopName || 'MithaiShop') + '</div>';

    var html = '<!DOCTYPE html><html><head><title>Invoice ' + invoice + '</title><style>' + css + '</style></head><body>' + content 
             + '<script>'
             + 'window.onload = function() { '
             + '  setTimeout(function() { '
             + '    window.print(); '
             + '    try { window.close(); } catch(e) {} '
             + '  }, 300); '
             + '};'
             + '</script>'
             + '</body></html>';

    var blob = new Blob([html], { type: 'text/html' });
    var url = URL.createObjectURL(blob);
    var w = window.open(url, '_blank', 'width=420,height=650');
    if (w) {
        setTimeout(function() {
            URL.revokeObjectURL(url);
        }, 10000);
    }
});

// ===== ALERT =====
function showAlert(msg) {
    var existing = document.getElementById('sh-alert');
    if (existing) existing.remove();
    var div = document.createElement('div');
    div.id = 'sh-alert';
    div.style.cssText = [
        'position:fixed', 'top:24px', 'right:24px', 'z-index:9999',
        'background:#0f172a', 'color:#fff',
        'padding:12px 18px', 'border-radius:10px',
        'font-family:Outfit,sans-serif', 'font-size:13.5px', 'font-weight:500',
        'box-shadow:0 4px 20px rgba(15,23,42,.18)',
        'opacity:0', 'transform:translateY(8px)',
        'transition:opacity .2s,transform .2s'
    ].join(';');
    div.textContent = msg;
    document.body.appendChild(div);
    requestAnimationFrame(function () {
        div.style.opacity = '1';
        div.style.transform = 'translateY(0)';
    });
    setTimeout(function () {
        div.style.opacity = '0';
        div.style.transform = 'translateY(8px)';
        setTimeout(function () { div.remove(); }, 250);
    }, 3000);
}
