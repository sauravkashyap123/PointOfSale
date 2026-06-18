// ============================================================
//  warehouse.js  –  MithaiShop POS · Warehouse Management
// ============================================================

var passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#^()_\-+=]).{8,}$/;

$(document).ready(function () {

    // ── 1. INITIAL DATA LOAD ──────────────────────────────────
    loadWarehouse();

    // ── 2. FORM SUBMIT ────────────────────────────────────────
    $("#warehouseForm").submit(function (e) {
        e.preventDefault();

        var name = $.trim($("#WarehouseName").val());
        var code = $.trim($("#WarehouseCode").val());
        var psw = $("#Password").val();

        if (!name) {
            highlightError("#WarehouseName", "Warehouse name is required.");
            return false;
        }
        if (!code) {
            highlightError("#WarehouseCode", "Warehouse code is required.");
            return false;
        }
        if (!passwordRegex.test(psw)) {
            Swal.fire({
                icon: "warning",
                title: "Invalid Password",
                text: "Password must contain at least 1 uppercase, 1 lowercase, 1 number and 1 special character (min 8 characters).",
                confirmButtonColor: "#0C447C"
            });
            $("#Password").addClass("is-invalid").focus();
            return false;
        }

        var model = {
            Id: $("#Id").val(),
            WarehouseName: name,
            WarehouseCode: code,
            Address: $("#Address").val(),
            NoOfShop: $("#NoOfShop").val(),
            City: $("#City").val(),
            State: $("#State").val(),
            ContactNumber: $("#ContactNumber").val(),
            Email: $("#Email").val(),
            Pincode: $("#Pincode").val(),
            Description: $("#Description").val(),
            Password: psw
        };

        $("#btnSave").prop("disabled", true);
        $("#btnSaveText").text("Saving…");

        $.ajax({
            url: '/Home/AddWarehouse',
            type: 'POST',
            data: model,
            success: function (res) {
                if (res.status === true) {
                    Swal.fire({
                        icon: "success",
                        title: "Success!",
                        text: res.message,
                        confirmButtonColor: "#0C447C",
                        timer: 2500,
                        timerProgressBar: true
                    });
                    cancelEdit();
                    loadWarehouse();
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Failed",
                        text: res.message,
                        confirmButtonColor: "#0C447C"
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: "error",
                    title: "Server Error",
                    text: "Something went wrong. Please try again.",
                    confirmButtonColor: "#0C447C"
                });
            },
            complete: function () {
                $("#btnSave").prop("disabled", false);
                $("#btnSaveText").text(
                    $("#Id").val() ? "Update Warehouse" : "Save Warehouse"
                );
            }
        });
    });

    // Remove inline error highlights on input
    $(".form-control").on("input focus", function () {
        $(this).removeClass("is-invalid");
    });
});

// ── 3. VALIDATION HELPER ─────────────────────────────────────
function highlightError(selector, message) {
    $(selector).addClass("is-invalid").focus();
    Swal.fire({
        icon: "warning",
        title: "Validation Error",
        text: message,
        confirmButtonColor: "#0C447C",
        timer: 2500
    });
}

// ── 4. LOAD & RENDER TABLE ───────────────────────────────────
function loadWarehouse() {

    // Destroy previous DataTable instance safely
    if ($.fn.dataTable && $.fn.dataTable.isDataTable('#warehouseTable')) {
        $('#warehouseTable').DataTable().destroy();
    }

    $.ajax({
        url: '/Home/AllWarehouseList',
        type: 'GET',
        success: function (res) {
            var data = res.data || [];
            var html = '';

            if (data.length === 0) {
                html = '<tr><td colspan="12">' +
                    '<div class="empty-state">' +
                    '<svg xmlns="http://www.w3.org/2000/svg" width="40" height="40" fill="none" viewBox="0 0 24 24" stroke="#d1d5db" stroke-width="1.5">' +
                    '<path stroke-linecap="round" stroke-linejoin="round" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"/>' +
                    '</svg>' +
                    '<p>No warehouses found. Add your first warehouse above.</p>' +
                    '</div></td></tr>';
            } else {
                $.each(data, function (i, item) {
                    html += `
                        <tr>
                            <td></td>
                            <td class="cell-num">${i + 1}</td>
                            <td>
                                <span class="status-dot"></span>
                                <span class="cell-name">${escHtml(item.warehouseName)}</span>
                            </td>
                            <td><span class="cell-code">${escHtml(item.warehouseCode)}</span></td>
                            <td>${escHtml(item.address) || '—'}</td>
                            <td>${escHtml(item.city) || '—'}</td>
                            <td>${escHtml(item.noOfShop) || '—'}</td>
                            <td>${escHtml(item.contactNumber) || '—'}</td>
                            <td class="cell-email">${escHtml(item.email) || '—'}</td>
                            <td>${escHtml(item.pincode) || '—'}</td>
                            <td class="cell-pw">${escHtml(item.password)}</td>
                            <td>
                                <button type="button" class="btn-tbl-edit"
                                    onclick='editWarehouse(${JSON.stringify(item)})'>
                                    <svg xmlns="http://www.w3.org/2000/svg" width="13" height="13" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                                        <path stroke-linecap="round" stroke-linejoin="round" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                                    </svg>
                                    Edit
                                </button>
                            </td>
                        </tr>`;
                });
            }

            $("#warehouseTable tbody").html(html);

            // ── DATATABLE INIT WITH RESPONSIVE CHILD-ROW ──
            $('#warehouseTable').DataTable({
                responsive: {
                    details: {
                        type: 'column',   // first <td> becomes the toggle
                        target: 'tr'        // click anywhere on row to expand
                    }
                },
                pageLength: 10,
                lengthMenu: [5, 10, 25, 50],
                columnDefs: [
                    // Col 0 → dtr-control (+/- toggle), never sortable, never hidden
                    {
                        orderable: false,
                        className: 'dtr-control',
                        responsivePriority: 1,
                        targets: 0
                    },
                    // Col 1 → Sr No
                    { responsivePriority: 8, targets: 1 },
                    // Col 2 → Warehouse Name — always visible
                    { responsivePriority: 2, targets: 2 },
                    // Col 3 → Code
                    { responsivePriority: 3, targets: 3 },
                    // Col 11 → Actions — always visible
                    { responsivePriority: 4, targets: 11 },
                    // Col 7 → Contact
                    { responsivePriority: 5, targets: 7 },
                    // Col 5 → City
                    { responsivePriority: 6, targets: 5 },
                    // Col 8 → Email
                    { responsivePriority: 7, targets: 8 },
                    // Remaining → collapse first
                    { responsivePriority: 10, targets: '_all' }
                ],
                language: {
                    search: "",
                    searchPlaceholder: "Search…",
                    lengthMenu: "Show _MENU_ rows",
                    info: "Showing _START_ – _END_ of _TOTAL_ warehouses",
                    infoEmpty: "No warehouses found",
                    paginate: {
                        previous: "‹",
                        next: "›"
                    }
                },
                dom: 'lrtip'  // custom search bar used — DataTables search box hidden
            });

            // Update badge count
            $("#rowCountBadge").text(data.length + " record" + (data.length !== 1 ? "s" : ""));

            // Show custom search bar
            if (data.length > 0) {
                $("#customSearch").show();
            }
        },
        error: function () {
            Swal.fire({
                icon: "error",
                title: "Load Error",
                text: "Could not load warehouse list.",
                confirmButtonColor: "#0C447C"
            });
        }
    });
}

// ── 5. POPULATE FORM FOR EDIT ────────────────────────────────
function editWarehouse(item) {
    $("#Id").val(item.id);
    $("#WarehouseName").val(item.warehouseName);
    $("#WarehouseCode").val(item.warehouseCode);
    $("#Address").val(item.address);
    $("#NoOfShop").val(item.noOfShop);
    $("#City").val(item.city);
    $("#State").val(item.state);
    $("#ContactNumber").val(item.contactNumber);
    $("#Email").val(item.email);
    $("#Pincode").val(item.pincode);
    $("#Description").val(item.description);
    $("#Password").val(item.password);

    $("#formCardTitle").text("Update Warehouse");
    $("#btnSaveText").text("Update Warehouse");
    $("#editBanner").addClass("show");

    window.scrollTo({ top: 0, behavior: 'smooth' });
}

// ── 6. RESET FORM ────────────────────────────────────────────
function cancelEdit() {
    $("#warehouseForm")[0].reset();
    $("#Id").val('');
    $(".form-control").removeClass("is-invalid");
    $("#formCardTitle").text("Add / Update Warehouse");
    $("#btnSaveText").text("Save Warehouse");
    $("#editBanner").removeClass("show");
}

// ── 7. HTML ESCAPE ───────────────────────────────────────────
function escHtml(str) {
    if (!str && str !== 0) return '';
    return String(str)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
