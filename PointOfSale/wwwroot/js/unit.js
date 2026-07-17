
var dt;

$(document).ready(function () {
    loadUnit();

    // ── Save / Update ──
    $("#unitForm").submit(function (e) {
        e.preventDefault();

        var id = parseInt($("#Id").val()) || 0;
        var unitName = $("#UnitName").val().trim();
        var shortName = $("#ShortName").val().trim();

        if (!unitName || !shortName) {
            showAlert('Please fill in all fields.', false);
            return;
        }

        var formData = { Id: id, UnitName: unitName, ShortName: shortName };
        var url = id > 0 ? '/Home/AddUnit' : '/Home/AddUnit';

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            success: function (res) {
                showAlert(res.message || 'Unit saved successfully!', true);
                cancelEdit();
                 loadUnit(); 
                updateBadge();
            },
            error: function () {
                showAlert('Something went wrong. Please try again.', false);
            }
        });
    });

    // ── Live Search ──
    document.getElementById('liveSearch').addEventListener('input', function () {
        if (dt) dt.search(this.value).draw();
    });

    // ── Entries Per Page ──
    document.getElementById('entriesSelect').addEventListener('change', function () {
        if (dt) dt.page.len(parseInt(this.value)).draw();
    });
});

function loadUnit() {
    if ($.fn.dataTable && $.fn.dataTable.isDataTable('#tblUnit')) {
        $('#tblUnit').DataTable().destroy();
    }

    $.ajax({
        url: '/Home/AllUnitList',
        type: 'GET',
        success: function (res) {
            var html = '';
            $.each(res.data, function (i, item) {
                html += `
    <tr>
    <td><span class="sno-badge">${i + 1}</span></td>
    <td class="unit-name-cell">${item.unitName}</td>
    <td><span class="short-pill">${item.shortName}</span></td>
    <td>
    <button type="button" class="btn-edit-s"
    onclick='editUnit(${JSON.stringify(item)})'>
    <i class="ti ti-edit" style="font-size:13px"></i> Edit
    </button>
    <button type="button" class="btn-del-s"
    onclick='deleteUnit(${item.id})'>
    <i class="ti ti-trash" style="font-size:13px"></i> Delete
    </button>

    </td>
    </tr>`;
            });

            $('#tblUnit tbody').html(html);

            dt = $('#tblUnit').DataTable({
                paging: true,
                searching: true,
                ordering: true,
                pageLength: 10
            });

            document.getElementById('unitCountBadge').textContent = res.data.length + ' Units';
        },
        error: function () {
            showAlert('Error loading units. Please refresh.', false);
        }
    });
}

function editUnit(item) {
    document.getElementById('Id').value = item.id;
    document.getElementById('UnitName').value = item.unitName;
    document.getElementById('ShortName').value = item.shortName;
    document.getElementById('saveBtnText').textContent = 'Update Unit';
    document.getElementById('formHeading').textContent = 'Edit Unit';
    document.getElementById('editBar').classList.add('show');
    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function cancelEdit() {
    document.getElementById('Id').value = 0;
    document.getElementById('UnitName').value = '';
    document.getElementById('ShortName').value = '';
    document.getElementById('saveBtnText').textContent = 'Save Unit';
    document.getElementById('formHeading').textContent = 'Add New Unit';
    document.getElementById('editBar').classList.remove('show');
}

function deleteUnit(id) {
    if (!confirm('Are you sure you want to delete this unit?')) return;
    $.ajax({
        url: '/Home/DeleteUnit/' + id,
        type: 'DELETE',
        data: { id: id },
        success: function (res) {
            if (res.status == true) {
                showAlert(res.message || 'Unit deleted.', true);
                loadUnit();
            }
            
            // if (dt) { dt.ajax.reload(); } else { loadUnit(); }
            updateBadge();
        },
        error: function () {
            showAlert('Delete failed. Please try again.', false);
        }
    });
}
function ChangeUnitStatus(id) {
    if (!confirm('Are you sure you want to delete this unit?')) return;
    $.ajax({
        url: '/Home/ChangeUnitStatus/' + id,
        type: 'GET',
        data: { id: id },
        success: function (res) {
            showAlert(res.message || 'Unit .', true);
            if (dt) { dt.ajax.reload(); } else { loadUnit(); }
            updateBadge();
        },
        error: function () {
            showAlert('Delete failed. Please try again.', false);
        }
    });
}

function showAlert(msg, success) {
    var a = document.getElementById('formAlert');
    var icon = document.getElementById('alertIcon');
    document.getElementById('alertText').textContent = msg;
    a.className = 'form-alert show ' + (success ? 'success' : 'error');
    icon.className = 'ti ' + (success ? 'ti-circle-check' : 'ti-alert-circle');
    setTimeout(function () { a.classList.remove('show'); }, 3000);
}

function updateBadge() {
    var rows = document.querySelectorAll('#tblUnit tbody tr').length;
    document.getElementById('unitCountBadge').textContent = rows + ' Units';
}