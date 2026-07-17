// ============================================================
//  shop.js  –  MithaiShop POS · Shop Management
// ============================================================

var PAGE_SIZE = 10;
var currentPage = 1;
var allShops = [];

$(document).ready(function () {

    
    loadShop();

   
    $('#searchInput').on('input', function () {
        currentPage = 1;
        renderTable();
    });

   
    $('#pageBtns').on('click', '.page-btn', function () {
        var p = $(this).data('page');
        if (p) {
            currentPage = p;
            renderTable();
            return;
        }
        if ($(this).attr('id') === 'prevBtn') { currentPage--; renderTable(); }
        if ($(this).attr('id') === 'nextBtn') { currentPage++; renderTable(); }
    });

    // ── 4. SAVE / UPDATE FORM SUBMIT ─────────────────────────
    $('#shopForm').submit(function (e) {
        e.preventDefault();

        var shopName = $.trim($('#ShopName').val());
        var whId = $('#WarehouseId').val();

        if (!shopName) {
            highlightError('#ShopName', 'Shop name is required.');
            return false;
        }
        if (!whId) {
            highlightError('#WarehouseId', 'Please select a warehouse.');
            return false;
        }

        var model = {
            Id: $('#Id').val(),
            ShopName: shopName,
            OwnerName: $.trim($('#OwnerName').val()),
            ContactNumber: $.trim($('#ContactNumber').val()),
            WarehouseId: whId
        };

        $('#btnSave').prop('disabled', true);
        $('#btnSaveText').text('Saving…');

        $.ajax({
            url: '/Home/AddShop',
            type: 'POST',
            data: model,
            success: function (res) {
                if (res.status === true) {
                    showToast(res.message, 'success');
                    cancelEdit();
                    loadShop();
                } else {
                    showToast(res.message, 'error');
                }
            },
            error: function () {
                showToast('Server error. Please try again.', 'error');
            },
            complete: function () {
                $('#btnSave').prop('disabled', false);
                $('#btnSaveText').text(
                    parseInt($('#Id').val()) > 0 ? 'Update Shop' : 'Save Shop'
                );
            }
        });
    });

    // Remove error highlight on input
    $('.form-control, .select-control').on('change input focus', function () {
        $(this).removeClass('is-invalid');
    });
});

// ── 5. LOAD DATA ─────────────────────────────────────────────
function loadShop() {
    $.ajax({
        url: '/Home/AllShopList',
        type: 'GET',
        success: function (res) {
            allShops = res.data || [];
            currentPage = 1;
            renderTable();
        },
        error: function () {
            showToast('Failed to load shop list.', 'error');
        }
    });
}

// ── 6. RENDER TABLE ──────────────────────────────────────────
function renderTable() {
    var query = $.trim($('#searchInput').val()).toLowerCase();

    var filtered = query
        ? allShops.filter(function (s) {
            return ((s.shopName || '') + ' ' +
                (s.ownerName || '') + ' ' +
                (s.contactNumber || '') + ' ' +
                (s.warehouseName || ''))
                .toLowerCase().indexOf(query) !== -1;
        })
        : allShops.slice();

    var total = filtered.length;
    var pages = Math.max(1, Math.ceil(total / PAGE_SIZE));
    currentPage = Math.min(currentPage, pages);

    var start = (currentPage - 1) * PAGE_SIZE;
    var paged = filtered.slice(start, start + PAGE_SIZE);

    // Badge
    $('#countBadge').text(total + ' shop' + (total === 1 ? '' : 's'));

    // Show search bar
    if (total > 0) $('#customSearch').show();

    // Rows
    if (!paged.length) {
        $('#shopTbody').html(
            '<tr class="empty-row"><td colspan="6">' +
            '<div class="empty-state">' +
            '<svg xmlns="http://www.w3.org/2000/svg" width="38" height="38" fill="none" ' +
            'viewBox="0 0 24 24" stroke="#d1d5db" stroke-width="1.5">' +
            '<path stroke-linecap="round" stroke-linejoin="round" ' +
            'd="M3 9.5L12 4l9 5.5V20H3V9.5z"/>' +
            '<path stroke-linecap="round" stroke-linejoin="round" d="M9 20v-6h6v6"/>' +
            '</svg>' +
            '<p>No shops found.</p>' +
            '</div></td></tr>'
        );
    } else {
        var html = '';
        $.each(paged, function (i, item) {
            html += '<tr>' +
                '<td class="cell-num">' + (start + i + 1) + '</td>' +
                '<td>' +
                '<span class="status-dot"></span>' +
                '<span class="cell-name">' + escHtml(item.shopName) + '</span>' +
                '</td>' +
                '<td>' + (escHtml(item.ownerName) || '—') + '</td>' +
                '<td>' + (escHtml(item.contactNumber) || '—') + '</td>' +
                '<td><span class="cell-code">' + (escHtml(item.warehouseName) || '—') + '</span></td>' +
                '<td>' +
                '<button type="button" class="btn-tbl-edit" ' +
                'onclick=\'editShop(' + JSON.stringify(item) + ')\'>' +
                '<svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" fill="none" ' +
                'viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">' +
                '<path stroke-linecap="round" stroke-linejoin="round" ' +
                'd="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5' +
                'm-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>' +
                '</svg> Edit' +
                '</button>' +
                '<button type="button" class="btn-tbl-edit" ' +
                'onclick=\'deleteShop(' + JSON.stringify(item.id) + ')\'>' +
                '<svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" fill="none" ' +
                'viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">' +
                '<path stroke-linecap="round" stroke-linejoin="round" ' +
                'd="M19 7L18.132 19.142A2 2 0 0116.138 21H7.862A2 2 0 015.868 19.142L5 7m5 4v6m4-6v6M9 7V4a1 1 0 011-1h4a1 1 0 011 1v3m-7 0h8"/>' +
                '</svg> Delete' +
                '</button>' +
                '</td>' +
                '</tr>';
        });
        $('#shopTbody').html(html);
    }

    // Pagination info
    var from = total ? start + 1 : 0;
    var to = Math.min(start + PAGE_SIZE, total);
    $('#pageInfo').text('Showing ' + from + '–' + to + ' of ' + total);

    // Pagination buttons
    var btns = '';
    btns += '<button class="page-btn" id="prevBtn"' +
        (currentPage <= 1 ? ' disabled' : '') + '>&#9664; Prev</button>';

    for (var p = 1; p <= pages; p++) {
        btns += '<button class="page-btn' + (p === currentPage ? ' active' : '') +
            '" data-page="' + p + '">' + p + '</button>';
    }

    btns += '<button class="page-btn" id="nextBtn"' +
        (currentPage >= pages ? ' disabled' : '') + '>Next &#9654;</button>';

    $('#pageBtns').html(btns);
}

// ── 7. EDIT  ← camelCase fix kiya yahan ─────────────────────
function editShop(item) {
    // Server camelCase se aata hai: item.id, item.shopName etc.
    $('#Id').val(item.id || item.Id || 0);
    $('#ShopName').val(item.shopName || item.ShopName || '');
    $('#OwnerName').val(item.ownerName || item.OwnerName || '');
    $('#ContactNumber').val(item.contactNumber || item.ContactNumber || '');
    $('#WarehouseId').val(item.warehouseId || item.WarehouseId || '');

    $('#formCardTitle').text('Update Shop');
    $('#btnSaveText').text('Update Shop');
    $('#editBanner').addClass('show');

    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function deleteShop(id) {
    $.ajax({
        url: "/Home/DeleteShop",
        type: "DELETE",
        data: { id: id },
        success: function (resp) {
            if (resp.status == true) {
                alert(resp.message);
                loadShop();
            }
        },
        error: function (resp) {
            alert("Something went wrong" + resp.message)
        }

    })
}
// ── 8. CANCEL / RESET ────────────────────────────────────────
function cancelEdit() {
    $('#shopForm')[0].reset();
    $('#Id').val(0);
    $('.form-control, .select-control').removeClass('is-invalid');
    $('#formCardTitle').text('Add / Update Shop');
    $('#btnSaveText').text('Save Shop');
    $('#editBanner').removeClass('show');
}

// ── 9. TOAST ─────────────────────────────────────────────────
function showToast(msg, type) {
    type = type || 'success';
    var icon = type === 'success' ? '✓' : '✕';
    var el = $('<div class="shop-toast ' + type + '">' + icon + ' ' + msg + '</div>');
    // Create container if missing
    if (!$('#toastWrap').length) {
        $('body').append('<div class="toast-wrap" id="toastWrap"></div>');
    }
    $('#toastWrap').append(el);
    setTimeout(function () {
        el.fadeOut(300, function () { el.remove(); });
    }, 3000);
}

// ── 10. VALIDATION HELPER ────────────────────────────────────
function highlightError(selector, message) {
    $(selector).addClass('is-invalid').focus();
    showToast(message, 'error');
}

// ── 11. HTML ESCAPE ──────────────────────────────────────────
function escHtml(str) {
    if (!str && str !== 0) return '';
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}
