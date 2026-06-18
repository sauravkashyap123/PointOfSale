// $(document).ready(function () {
//     getAllProducts();
// });

//     // ── CLOCK ──
//     function updateClock() {
//         var n = new Date();
//         var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
//         var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
//         var h = n.getHours(), m = n.getMinutes(), s = n.getSeconds(), ap = h >= 12 ? 'PM' : 'AM';
//         h = h % 12 || 12;
//         document.getElementById('clock').textContent = String(h).padStart(2, '0') + ':' + String(m).padStart(2, '0') + ':' + String(s).padStart(2, '0') + ' ' + ap;
//         document.getElementById('dateDisplay').textContent = days[n.getDay()] + ', ' + n.getDate() + ' ' + months[n.getMonth()] + ' ' + n.getFullYear();
//     }
// updateClock();
// setInterval(updateClock, 1000);

// var products = [];

// async function getAllProducts() {

//     try {

//         const response = await fetch(
//             '/Billing/GetAllProducts',
//             {
//                 method: 'GET'
//             }
//         );

//         if (!response.ok) {

//             throw new Error(
//                 "Failed to fetch products."
//             );
//         }

//         const aproducts = await response.json();

//         products = [];

//         aproducts.forEach(p => {

//             products.push({

//                 id: p.id,

//                 name: p.productName ?? "",

//                 price: p.saleRate,

//                 stock: p.stockQuantity,

//                 cat: p.categoryId
//                     ? p.categoryId.toString()
//                     : "",

//                 sku: p.productCode ?? "",
//                 imageurl:p.imageUrl
//             });

//         });

//         console.log(products);

//         renderProducts();

//     }
//     catch (error) {

//         console.error(error);

//         alert(
//             "Something went wrong while loading products."
//         );
//     }
// }


// // ── PRODUCTS — single clean line ──

// var currentCat = 'all', currentSearch = '', order = {}, discType = 'pct';

// // ── RENDER PRODUCTS ──
// function renderProducts() {
//     var grid = document.getElementById('productGrid');
//     if (!grid) return;
//     var list = products.slice();

//     if (currentCat !== 'all') {
//         var f = [];
//         for (var i = 0; i < list.length; i++) {
//             if (String(list[i].cat) === String(currentCat)) f.push(list[i]);
//         }
//         list = f;
//     }

//     if (currentSearch) {
//         var s = [];
//         for (var i = 0; i < list.length; i++) {
//             if (list[i].name.toLowerCase().indexOf(currentSearch) > -1 || list[i].sku.toLowerCase().indexOf(currentSearch) > -1) s.push(list[i]);
//         }
//         list = s;
//     }

//     if (!list.length) {
//         grid.innerHTML = '<div style="grid-column:1/-1;text-align:center;padding:60px 20px;color:var(--muted)"><i class="ti ti-search-off" style="font-size:48px;display:block;margin-bottom:12px;color:var(--border)"></i>No products found</div>';
//         return;
//     }

//     var html = '';
//     for (var i = 0; i < list.length; i++) {
//         var p = list[i];
//         var inCart = order[p.id] ? true : false;
//         var sc = 'ok', sl = p.stock + ' in stock';
//         if (p.stock === 0) { sc = 'out'; sl = 'Out of stock'; }
//         else if (p.stock <= 5) { sc = 'low'; sl = 'Only ' + p.stock + ' left'; }
//         var iconName = sc === 'ok' ? 'circle-check' : sc === 'low' ? 'alert-triangle' : 'circle-x';
//         html += '<div class="prod-card' + (inCart ? ' in-cart' : '') + '" onclick="addToOrder(' + p.id + ')">'
//             + '<div class="img-wrap">'
//             + '<span style="font-size:36px"><img src="' + p.imageurl +'" style="height: 100px;" ></img></span>'
//             + '<div class="stock-badge ' + sc + '"><i class="ti ti-' + iconName + '" style="font-size:10px;margin-right:2px"></i>' + sl + '</div>'
//             + '</div>'
//             + '<div class="card-body">'
//             + '<div class="prod-name">' + p.name + '</div>'
//             + '<div class="prod-price">&#8377;' + parseFloat(p.price).toFixed(2) + '</div>'
//             + '<div class="prod-sku"><i class="ti ti-barcode" style="font-size:11px"></i> ' + p.sku + '</div>'
//             + '<button class="add-btn" onclick="event.stopPropagation();addToOrder(' + p.id + ')">'
//             + '<i class="ti ti-' + (inCart ? 'check' : 'plus') + '"></i>' + (inCart ? ' Added' : ' Add to Order')
//             + '</button>'
//             + '</div>'
//             + '</div>';
//     }
//     grid.innerHTML = html;
// }

// // ── ORDER ACTIONS ──
// function addToOrder(id) {
//     debugger;
//     var p = null;
//     for (var i = 0; i < products.length; i++) {
//         if (products[i].id === id) { p = products[i]; break; }
//     }
//     if (!p || p.stock === 0) return;
//     if (order[id]) order[id].qty++;
//     else order[id] = { id: p.id, name: p.name, price: p.price, stock: p.stock, cat: p.cat, sku: p.sku, qty: 1 };

//     // $.ajax({
//     //     url: '/Billing/AddToCart',
//     //     method: 'POST',
//     //     data: { productId: id },
//     //     success: function (response) {
//     //         if (response.status) {
//     //             var p = null;
//     //             for (var i = 0; i < products.length; i++) {
//     //                 if (products[i].id === id) { p = products[i]; break; }
//     //             }
//     //             if (!p || p.stock === 0) return;
//     //             if (order[id]) order[id].qty++;
//     //             else order[id] = { id: p.id, name: p.name, price: p.price, stock: p.stock, cat: p.cat, sku: p.sku, qty: 1 };
//     //         }

//     //     }
//     // });
//     renderOrder();
//     renderProducts();
// }

// function changeQty(id, d) {
//     if (!order[id]) return;
//     order[id].qty += d;
//     if (order[id].qty <= 0) delete order[id];
//     renderOrder();
//     renderProducts();
// }

// function removeItem(id) { delete order[id]; renderOrder(); renderProducts(); }
// function clearOrder() { order = {}; renderOrder(); renderProducts(); }

// // ── RENDER ORDER ──
// function renderOrder() {
//     var items = Object.values(order);
//     var cnt = items.length;
//     document.getElementById('orderCount').textContent = cnt;
//     document.getElementById('orderCountMob').textContent = cnt;
//     document.getElementById('mobCartCount').textContent = cnt;

//     var html = '';
//     if (items.length) {
//         for (var i = 0; i < items.length; i++) {
//             var it = items[i];
//             html += '<div class="order-item">'
//                 + '<span class="oi-emoji"><i class="ti ti-shopping-bag" style="font-size:22px"></i></span>'
//                 + '<div class="oi-info">'
//                 + '<div class="oi-name">' + it.name + '</div>'
//                 + '<div class="oi-line">&#8377;' + (it.price * it.qty).toFixed(2) + '</div>'
//                 + '<div class="qty-ctrl">'
//                 + '<button class="q-btn" onclick="changeQty(' + it.id + ',-1)"><i class="ti ti-minus" style="font-size:12px"></i></button>'
//                 + '<span class="q-num">' + it.qty + '</span>'
//                 + '<button class="q-btn" onclick="changeQty(' + it.id + ',1)"><i class="ti ti-plus" style="font-size:12px"></i></button>'
//                 + '<span class="q-unit">x &#8377;' + parseFloat(it.price).toFixed(2) + '</span>'
//                 + '</div>'
//                 + '</div>'
//                 + '<button class="oi-remove" onclick="removeItem(' + it.id + ')"><i class="ti ti-x"></i></button>'
//                 + '</div>';
//         }
//     } else {
//         html = '<div class="empty-order"><i class="ti ti-shopping-cart-plus"></i><p>No items added yet</p><small>Click a product to add</small></div>';
//     }
//     document.getElementById('orderItems').innerHTML = html;
//     document.getElementById('orderItemsMob').innerHTML = html;
//     recalc();
// }

// // ── RECALC ──
// function recalc() {
//     var items = Object.values(order);
//     var sub = 0;
//     for (var i = 0; i < items.length; i++) sub += items[i].price * items[i].qty;
//     var dv = parseFloat(document.getElementById('discountVal').value) || 0;
//     var disc = discType === 'pct' ? sub * (dv / 100) : Math.min(dv, sub);
//     var grand = Math.max(0, sub - disc);
//     var rs = '&#8377;';
//     document.getElementById('subtotalVal').innerHTML = rs + sub.toFixed(2);
//     document.getElementById('grandTotal').innerHTML = rs + grand.toFixed(2);
//     document.getElementById('payAmt').innerHTML = rs + grand.toFixed(2);
//     document.getElementById('subtotalMob').innerHTML = rs + sub.toFixed(2);
//     document.getElementById('grandMob').innerHTML = rs + grand.toFixed(2);
//     document.getElementById('payAmtMob').innerHTML = rs + grand.toFixed(2);
//     document.getElementById('discSaved').innerHTML = disc > 0 ? '-' + rs + disc.toFixed(2) : '';
// }

// function setDiscType(t) {
//     discType = t;
//     document.getElementById('discPct').classList.toggle('active', t === 'pct');
//     document.getElementById('discFlat').classList.toggle('active', t === 'flat');
//     recalc();
// }

// // ── FILTERS ──
// function selectCat(el, cat) {
//     var pills = document.querySelectorAll('.cat-pill');
//     for (var i = 0; i < pills.length; i++) pills[i].classList.remove('active');
//     el.classList.add('active');
//     currentCat = cat;
//     renderProducts();
// }

// function filterProducts() {
//     currentSearch = document.getElementById('searchInput').value.toLowerCase();
//     renderProducts();
// }

// function clearSearch() {
//     document.getElementById('searchInput').value = '';
//     currentSearch = '';
//     renderProducts();
// }

// function toggleFilter(t) {
//     document.getElementById('catBtn').classList.toggle('active', t === 'cat');
//     document.getElementById('brandBtn').classList.toggle('active', t === 'brand');
// }

// // ── MOBILE DRAWER ──
// function openCartDrawer() {
//     document.getElementById('cartOverlay').classList.add('open');
//     document.getElementById('cartDrawer').classList.add('open');
// }
// function closeCartDrawer() {
//     document.getElementById('cartOverlay').classList.remove('open');
//     document.getElementById('cartDrawer').classList.remove('open');
// }

// // ── BILL ──
// function openBill() {
//     var items = Object.values(order);
//     if (!items.length) { alert('Please add items to the order first.'); return; }
//     closeCartDrawer();
//     var now = new Date();
//     var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
//     var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
//     var billNo = 'INV-' + Math.floor(Math.random() * 90000 + 10000);
//     var sub = 0;
//     for (var i = 0; i < items.length; i++) sub += items[i].price * items[i].qty;
//     var dv = parseFloat(document.getElementById('discountVal').value) || 0;
//     var disc = discType === 'pct' ? sub * (dv / 100) : Math.min(dv, sub);
//     var grand = Math.max(0, sub - disc);
//     var h = now.getHours(), ap = h >= 12 ? 'PM' : 'AM';
//     h = h % 12 || 12;
//     var ts = String(h).padStart(2, '0') + ':' + String(now.getMinutes()).padStart(2, '0') + ' ' + ap;

//     document.getElementById('billMeta').innerHTML = '<strong>Bill No:</strong> ' + billNo + '<br>' + days[now.getDay()] + ', ' + now.getDate() + ' ' + months[now.getMonth()] + ' ' + now.getFullYear() + ' &middot; ' + ts + '<br>Cashier: Arjun Kumar';

//     var billRows = '';
//     for (var i = 0; i < items.length; i++) {
//         var it = items[i];
//         billRows += '<div class="bill-item-row">'
//             + '<span class="bi-name">' + it.name + '</span>'
//             + '<span class="bi-qty">' + it.qty + '</span>'
//             + '<span class="bi-amt">&#8377;' + (it.price * it.qty).toFixed(2) + '</span>'
//             + '</div>';
//     }
//     document.getElementById('billItemsBody').innerHTML = billRows;

//     var tots = '<div class="bill-tot-row"><span>Subtotal</span><span>&#8377;' + sub.toFixed(2) + '</span></div>';
//     if (disc > 0) tots += '<div class="bill-tot-row" style="color:var(--green)"><span>Discount</span><span>-&#8377;' + disc.toFixed(2) + '</span></div>';
//     tots += '<div class="bill-tot-row"><span>Tax (0%)</span><span>&#8377;0.00</span></div>';
//     tots += '<div class="bill-tot-row grand"><span>TOTAL AMOUNT</span><span>&#8377;' + grand.toFixed(2) + '</span></div>';
//     document.getElementById('billTots').innerHTML = tots;
//     document.getElementById('billModal').style.display = 'flex';
// }

// function closeBill() { document.getElementById('billModal').style.display = 'none'; }
// function closeBillOutside(e) { if (e.target === document.getElementById('billModal')) closeBill(); }

// // ── PRINT ──
// function printBill() {
//     var content = document.getElementById('billPrint').innerHTML;
//     var css = '*{box-sizing:border-box;margin:0;padding:0}'
//         + 'body{font-family:monospace;padding:20px;font-size:12px;color:#111;max-width:380px;margin:0 auto}'
//         + '.bill-top{text-align:center;padding-bottom:12px;border-bottom:2px dashed #ccc;margin-bottom:12px}'
//         + '.bill-logo{font-size:20px;font-weight:700}'
//         + '.bill-store,.bill-meta{font-size:10px;color:#666;margin-top:3px}'
//         + '.bill-items-head{display:flex;font-size:10px;font-weight:700;color:#999;padding:4px 0;border-bottom:1px solid #ddd;margin-bottom:5px}'
//         + '.bh-item{flex:1}.bh-qty{width:34px;text-align:center}.bh-amt{width:72px;text-align:right}'
//         + '.bill-item-row{display:flex;font-size:11px;padding:4px 0;border-bottom:1px dotted #eee;align-items:center}'
//         + '.bi-name{flex:1;line-height:1.3}.bi-qty{width:34px;text-align:center;color:#888}.bi-amt{width:72px;text-align:right;font-weight:700}'
//         + '.bill-tots{padding-top:9px;margin-top:3px;border-top:1px dashed #ccc}'
//         + '.bill-tot-row{display:flex;justify-content:space-between;font-size:11px;padding:3px 0}'
//         + '.bill-tot-row.grand{font-size:14px;font-weight:700;border-top:2px dashed #ccc;margin-top:5px;padding-top:7px}'
//         + '.bill-footer{text-align:center;margin-top:12px;padding-top:10px;border-top:2px dashed #ccc;font-size:10px;color:#999;line-height:1.7}';
//     var html = '<!DOCTYPE html><html><head><title>Bill</title><style>' + css + '</style></head><body>' + content + '</body></html>';
//     var blob = new Blob([html], { type: 'text/html' });
//     var url = URL.createObjectURL(blob);
//     var w = window.open(url, '_blank', 'width=420,height=650');
//     if (w) {
//         w.onload = function () { w.print(); URL.revokeObjectURL(url); };
//     }
// }

// // ── INIT ──
// renderProducts();
// renderOrder();
$(document).ready(function () {
    getAllProducts();
    loadCartData();
});

// ── CLOCK ──
function updateClock() {
    var n = new Date();
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
    var h = n.getHours(), m = n.getMinutes(), s = n.getSeconds(), ap = h >= 12 ? 'PM' : 'AM';
    h = h % 12 || 12;
    document.getElementById('clock').textContent = String(h).padStart(2, '0') + ':' + String(m).padStart(2, '0') + ':' + String(s).padStart(2, '0') + ' ' + ap;
    document.getElementById('dateDisplay').textContent = days[n.getDay()] + ', ' + n.getDate() + ' ' + months[n.getMonth()] + ' ' + n.getFullYear();
}
updateClock();
setInterval(updateClock, 1000);

var products = [];
var currentCat = 'all', currentSearch = '', order = {}, discType = 'pct';

// ── FETCH PRODUCTS ──
async function getAllProducts() {
    try {
        const response = await fetch('/Billing/GetAllProducts', { method: 'GET' });
        if (!response.ok) throw new Error("Failed to fetch products.");
        const aproducts = await response.json();
        products = [];
        aproducts.forEach(p => {
            products.push({
                id: p.id,
                name: p.productName || "",
                price: p.saleRate,
                stock: p.stockQuantity,
                cat: p.categoryId ? p.categoryId.toString() : "",
                sku: p.productCode || "",
                imageurl: p.imageUrl
            });
        });
        renderProducts();
    } catch (error) {
        console.error(error);
        var grid = document.getElementById('productGrid');
        if (grid) grid.innerHTML = '<div style="grid-column:1/-1;text-align:center;padding:40px;color:var(--muted)"><i class="ti ti-wifi-off" style="font-size:40px;display:block;margin-bottom:10px"></i>Products load nahi hue. Refresh karo.</div>';
    }
}

// ── LOAD CART FROM SERVER ──
function loadCartData() {
    $.ajax({
        url: '/Billing/GetCartData',
        method: 'GET',
        success: function (response) {
            order = {};
            if (response.cart && response.cart.length > 0) {
                response.cart.forEach(function (item) {
                    order[item.productId] = {
                        id: item.productId,
                        name: item.productName || "",
                        price: item.rate || item.saleRate || 0,
                        qty: item.quantity,
                        stock: 999,
                        cat: "",
                        sku: ""
                    };
                });
            }
            renderOrder();
            renderProducts();
        },
        error: function () {
            console.error('Cart load failed');
        }
    });
}

// ── RENDER PRODUCTS ──
function renderProducts() {
    var grid = document.getElementById('productGrid');
    if (!grid) return;
    var list = products.slice();

    if (currentCat !== 'all') {
        var f = [];
        for (var i = 0; i < list.length; i++) {
            if (String(list[i].cat) === String(currentCat)) f.push(list[i]);
        }
        list = f;
    }

    if (currentSearch) {
        var s = [];
        for (var i = 0; i < list.length; i++) {
            if (list[i].name.toLowerCase().indexOf(currentSearch) > -1 || list[i].sku.toLowerCase().indexOf(currentSearch) > -1) s.push(list[i]);
        }
        list = s;
    }

    if (!list.length) {
        grid.innerHTML = '<div style="grid-column:1/-1;text-align:center;padding:60px 20px;color:var(--muted)"><i class="ti ti-search-off" style="font-size:48px;display:block;margin-bottom:12px;color:var(--border)"></i>No products found</div>';
        return;
    }

    var html = '';
    for (var i = 0; i < list.length; i++) {
        var p = list[i];
        var inCart = order[p.id] ? true : false;
        var sc = 'ok', sl = p.stock + ' in stock';
        if (p.stock === 0) { sc = 'out'; sl = 'Out of stock'; }
        else if (p.stock <= 5) { sc = 'low'; sl = 'Only ' + p.stock + ' left'; }
        var iconName = sc === 'ok' ? 'circle-check' : sc === 'low' ? 'alert-triangle' : 'circle-x';
        var imgHtml = p.imageurl
            ? '<img src="' + p.imageurl + '" style="height:100px;object-fit:cover;border-radius:8px" onerror="this.outerHTML=\'<i class=ti\\ ti-shopping-bag style=font-size:40px></i>\'">'
            : '<i class="ti ti-shopping-bag" style="font-size:40px"></i>';
        html += '<div class="prod-card' + (inCart ? ' in-cart' : '') + '" onclick="addToOrder(' + p.id + ')">'
            + '<div class="img-wrap">'
            + imgHtml
            + '<div class="stock-badge ' + sc + '"><i class="ti ti-' + iconName + '" style="font-size:10px;margin-right:2px"></i>' + sl + '</div>'
            + '</div>'
            + '<div class="card-body">'
            + '<div class="prod-name">' + p.name + '</div>'
            + '<div class="prod-price">&#8377;' + parseFloat(p.price).toFixed(2) + '</div>'
            + '<div class="prod-sku"><i class="ti ti-barcode" style="font-size:11px"></i> ' + p.sku + '</div>'
            + '<button class="add-btn" onclick="event.stopPropagation();addToOrder(' + p.id + ')">'
            + '<i class="ti ti-' + (inCart ? 'check' : 'plus') + '"></i>' + (inCart ? ' Added' : ' Add to Order')
            + '</button>'
            + '</div>'
            + '</div>';
    }
    grid.innerHTML = html;
}

// ── ADD TO ORDER ──
function addToOrder(id) {
    var p = null;
    for (var i = 0; i < products.length; i++) {
        if (products[i].id === id) { p = products[i]; break; }
    }
    if (!p || p.stock === 0) return;

    // pehli baar cart me add — AddToCart call karo
    if (!order[id]) {
        $.ajax({
            url: '/Billing/AddToCart',
            method: 'POST',
            data: { productId: id },
            success: function (response) {
                if (response.status) {
                    order[id] = { id: p.id, name: p.name, price: p.price, stock: p.stock, cat: p.cat, sku: p.sku, qty: 1 };
                    syncCartFromResponse(response.cart);
                } else {
                    alert(response.message || 'Add to cart failed.');
                }
            },
            error: function () {
                alert('Something went wrong.');
            }
        });
    } else {
        // already cart me hai — qty badhao via UpdateQty
        var newQty = order[id].qty + 1;
        $.ajax({
            url: '/Billing/UpdateQty',
            method: 'POST',
            data: { productId: id, qty: newQty, type: 'increase' },
            success: function (response) {
                if (response.status) {
                    order[id].qty = newQty;
                    syncCartFromResponse(response.cart);
                } else {
                    alert(response.message || 'Update failed.');
                }
            },
            error: function () {
                alert('Something went wrong.');
            }
        });
    }
}

// ── CHANGE QTY ──
function changeQty(id, d) {
    if (!order[id]) return;
    var newQty = order[id].qty + d;

    if (newQty <= 0) {
        removeItem(id);
        return;
    }

    $.ajax({
        url: '/Billing/UpdateQty',
        method: 'POST',
        data: { productId: id, qty: newQty, type: d > 0 ? 'increase' : 'decrease' },
        success: function (response) {
            if (response.status) {
                if (order[id]) order[id].qty = newQty;
                syncCartFromResponse(response.cart);
            } else {
                alert(response.message || 'Update failed.');
            }
        },
        error: function () {
            alert('Something went wrong.');
        }
    });
}

// ── REMOVE ITEM ──
function removeItem(id) {
    $.ajax({
        url: '/Billing/RemoveItem',
        method: 'POST',
        data: { productId: id },
        success: function (response) {
            if (response.status) {
                delete order[id];
                syncCartFromResponse(response.cart);
            } else {
                alert(response.message || 'Remove failed.');
            }
        },
        error: function () {
            alert('Something went wrong.');
        }
    });
}

// ── CLEAR CART ──
function clearOrder() {
    $.ajax({
        url: '/Billing/ClearCart',
        method: 'POST',
        success: function (response) {
            if (response.status) {
                order = {};
                renderOrder();
                renderProducts();
            } else {
                alert('Clear cart failed.');
            }
        },
        error: function () {
            alert('Something went wrong.');
        }
    });
}

// ── SYNC CART FROM SERVER RESPONSE ──
function syncCartFromResponse(cart) {
    if (cart && cart.length > 0) {
        cart.forEach(function (item) {
            if (order[item.productId]) {
                order[item.productId].qty = item.quantity;
                order[item.productId].price = item.rate || item.saleRate || order[item.productId].price;
            }
        });
    }
    renderOrder();
    renderProducts();
}

// ── RENDER ORDER ──
function renderOrder() {
    var items = Object.values(order);
    var cnt = items.length;
    document.getElementById('orderCount').textContent = cnt;
    document.getElementById('orderCountMob').textContent = cnt;
    document.getElementById('mobCartCount').textContent = cnt;

    var html = '';
    if (items.length) {
        for (var i = 0; i < items.length; i++) {
            var it = items[i];
            html += '<div class="order-item">'
                + '<span class="oi-emoji"><i class="ti ti-shopping-bag" style="font-size:22px"></i></span>'
                + '<div class="oi-info">'
                + '<div class="oi-name">' + it.name + '</div>'
                + '<div class="oi-line">&#8377;' + (it.price * it.qty).toFixed(2) + '</div>'
                + '<div class="qty-ctrl">'
                + '<button class="q-btn" onclick="changeQty(' + it.id + ',-1)"><i class="ti ti-minus" style="font-size:12px"></i></button>'
                + '<span class="q-num">' + it.qty + '</span>'
                + '<button class="q-btn" onclick="changeQty(' + it.id + ',1)"><i class="ti ti-plus" style="font-size:12px"></i></button>'
                + '<span class="q-unit">x &#8377;' + parseFloat(it.price).toFixed(2) + '</span>'
                + '</div>'
                + '</div>'
                + '<button class="oi-remove" onclick="removeItem(' + it.id + ')"><i class="ti ti-x"></i></button>'
                + '</div>';
        }
    } else {
        html = '<div class="empty-order"><i class="ti ti-shopping-cart-plus"></i><p>No items added yet</p><small>Click a product to add</small></div>';
    }
    document.getElementById('orderItems').innerHTML = html;
    document.getElementById('orderItemsMob').innerHTML = html;
    recalc();
}

// ── RECALC ──
function recalc() {
    var items = Object.values(order);
    var sub = 0;
    for (var i = 0; i < items.length; i++) sub += items[i].price * items[i].qty;
    var dv = parseFloat(document.getElementById('discountVal').value) || 0;
    var disc = discType === 'pct' ? sub * (dv / 100) : Math.min(dv, sub);
    var grand = Math.max(0, sub - disc);
    var rs = '&#8377;';
    document.getElementById('subtotalVal').innerHTML = rs + sub.toFixed(2);
    document.getElementById('grandTotal').innerHTML = rs + grand.toFixed(2);
    document.getElementById('payAmt').innerHTML = rs + grand.toFixed(2);
    document.getElementById('subtotalMob').innerHTML = rs + sub.toFixed(2);
    document.getElementById('grandMob').innerHTML = rs + grand.toFixed(2);
    document.getElementById('payAmtMob').innerHTML = rs + grand.toFixed(2);
    document.getElementById('discSaved').innerHTML = disc > 0 ? '-' + rs + disc.toFixed(2) : '';
}

function setDiscType(t) {
    discType = t;
    document.getElementById('discPct').classList.toggle('active', t === 'pct');
    document.getElementById('discFlat').classList.toggle('active', t === 'flat');
    recalc();
}

// ── FILTERS ──
function selectCat(el, cat) {
    var pills = document.querySelectorAll('.cat-pill');
    for (var i = 0; i < pills.length; i++) pills[i].classList.remove('active');
    el.classList.add('active');
    currentCat = cat;
    renderProducts();
}

function filterProducts() {
    currentSearch = document.getElementById('searchInput').value.toLowerCase();
    renderProducts();
}

function clearSearch() {
    document.getElementById('searchInput').value = '';
    currentSearch = '';
    renderProducts();
}

function toggleFilter(t) {
    document.getElementById('catBtn').classList.toggle('active', t === 'cat');
    document.getElementById('brandBtn').classList.toggle('active', t === 'brand');
}

// ── MOBILE DRAWER ──
function openCartDrawer() {
    document.getElementById('cartOverlay').classList.add('open');
    document.getElementById('cartDrawer').classList.add('open');
}
function closeCartDrawer() {
    document.getElementById('cartOverlay').classList.remove('open');
    document.getElementById('cartDrawer').classList.remove('open');
}

// ── BILL ──
// function openBill() {
//     debugger;
//     var items = Object.values(order);
//     if (!items.length) { alert('Please add items to the order first.'); return; }
//     closeCartDrawer();
//     var now = new Date();
//     var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
//     var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
//     let billNo = 'INV-' + Math.floor(Math.random() * 90000 + 10000);

//     $.ajax({
//         url: "/Billing/GenerateBillNo",
//         method: "POST",
//         success: function (response) {
//             alert(response.billno);
//             billNo = response.billno;
//             alert(billNo);
//         },
//         error: function () {
//             alert('Failed to generate bill no.');
//         }
//     })

//     var sub = 0;
//     for (var i = 0; i < items.length; i++) sub += items[i].price * items[i].qty;
//     var dv = parseFloat(document.getElementById('discountVal').value) || 0;
//     var disc = discType === 'pct' ? sub * (dv / 100) : Math.min(dv, sub);
//     var grand = Math.max(0, sub - disc);
//     var h = now.getHours(), ap = h >= 12 ? 'PM' : 'AM';
//     h = h % 12 || 12;
//     var ts = String(h).padStart(2, '0') + ':' + String(now.getMinutes()).padStart(2, '0') + ' ' + ap;

//     document.getElementById('billMeta').innerHTML = '<strong>Bill No:</strong> ' + billNo + '<br>' + days[now.getDay()] + ', ' + now.getDate() + ' ' + months[now.getMonth()] + ' ' + now.getFullYear() + ' &middot; ' + ts + '<br>Cashier: Arjun Kumar';

//     var billRows = '';
//     for (var i = 0; i < items.length; i++) {
//         var it = items[i];
//         billRows += '<div class="bill-item-row">'
//             + '<span class="bi-name">' + it.name + '</span>'
//             + '<span class="bi-qty">' + it.qty + '</span>'
//             + '<span class="bi-amt">&#8377;' + (it.price * it.qty).toFixed(2) + '</span>'
//             + '</div>';
//     }
//     document.getElementById('billItemsBody').innerHTML = billRows;

//     var tots = '<div class="bill-tot-row"><span>Subtotal</span><span>&#8377;' + sub.toFixed(2) + '</span></div>';
//     if (disc > 0) tots += '<div class="bill-tot-row" style="color:var(--green)"><span>Discount</span><span>-&#8377;' + disc.toFixed(2) + '</span></div>';
//     tots += '<div class="bill-tot-row"><span>Tax (0%)</span><span>&#8377;0.00</span></div>';
//     tots += '<div class="bill-tot-row grand"><span>TOTAL AMOUNT</span><span>&#8377;' + grand.toFixed(2) + '</span></div>';
//     document.getElementById('billTots').innerHTML = tots;
//     document.getElementById('billModal').style.display = 'flex';
// }
let billNo = '';
async function openBill() {
    debugger;
    var items = Object.values(order);
    if (!items.length) { alert('Please add items to the order first.'); return; }
    closeCartDrawer();

    var now = new Date();
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

    // Yahan hum local variable hi use kar rahe hain
    

    try {
        // 'await' use karne se JS rukega jab tak backend se response nahi aata
        let response = await $.ajax({
            url: "/Billing/GenerateBillNo",
            method: "POST"
        });

        billNo = response.billno || response; // Backend se bill no mil gaya
    } catch (error) {
        alert('Failed to generate bill no.');
        return; // Agar error aaya toh aage ka code nahi chalega
    }

    // --- Ab niche ka code normal chalega aur billNo mein value hogi ---
    var sub = 0;
    for (var i = 0; i < items.length; i++) sub += items[i].price * items[i].qty;
    var dv = parseFloat(document.getElementById('discountVal').value) || 0;
    var disc = discType === 'pct' ? sub * (dv / 100) : Math.min(dv, sub);
    var grand = Math.max(0, sub - disc);

    var h = now.getHours(), ap = h >= 12 ? 'PM' : 'AM';
    h = h % 12 || 12;
    var ts = String(h).padStart(2, '0') + ':' + String(now.getMinutes()).padStart(2, '0') + ' ' + ap;

    document.getElementById('billMeta').innerHTML = '<strong>Bill No:</strong> ' + billNo + '<br>' + days[now.getDay()] + ', ' + now.getDate() + ' ' + months[now.getMonth()] + ' ' + now.getFullYear() + ' &middot; ' + ts + '<br>Cashier: Arjun Kumar';

    var billRows = '';
    for (var i = 0; i < items.length; i++) {
        var it = items[i];
        billRows += '<div class="bill-item-row">'
            + '<span class="bi-name">' + it.name + '</span>'
            + '<span class="bi-qty">' + it.qty + '</span>'
            + '<span class="bi-amt">&#8377;' + (it.price * it.qty).toFixed(2) + '</span>'
            + '</div>';
    }
    document.getElementById('billItemsBody').innerHTML = billRows;

    var tots = '<div class="bill-tot-row"><span>Subtotal</span><span>&#8377;' + sub.toFixed(2) + '</span></div>';
    if (disc > 0) tots += '<div class="bill-tot-row" style="color:var(--green)"><span>Discount</span><span>-&#8377;' + disc.toFixed(2) + '</span></div>';
    tots += '<div class="bill-tot-row"><span>Tax (0%)</span><span>&#8377;0.00</span></div>';
    tots += '<div class="bill-tot-row grand"><span>TOTAL AMOUNT</span><span>&#8377;' + grand.toFixed(2) + '</span></div>';
    document.getElementById('billTots').innerHTML = tots;
    document.getElementById('billModal').style.display = 'flex';

    // Ab aap is billNo ko doosre function mein as a Parameter bhej sakte hain:
    //mySecondFunction(billNo);
    // printBill(billNo);
}

function closeBill() { document.getElementById('billModal').style.display = 'none'; }
function closeBillOutside(e) { if (e.target === document.getElementById('billModal')) closeBill(); }

// ── PRINT ──
// function printBill() {
//     var content = document.getElementById('billPrint').innerHTML;
//     var css = '*{box-sizing:border-box;margin:0;padding:0}'
//         + 'body{font-family:monospace;padding:20px;font-size:12px;color:#111;max-width:380px;margin:0 auto}'
//         + '.bill-top{text-align:center;padding-bottom:12px;border-bottom:2px dashed #ccc;margin-bottom:12px}'
//         + '.bill-logo{font-size:20px;font-weight:700}'
//         + '.bill-store,.bill-meta{font-size:10px;color:#666;margin-top:3px}'
//         + '.bill-items-head{display:flex;font-size:10px;font-weight:700;color:#999;padding:4px 0;border-bottom:1px solid #ddd;margin-bottom:5px}'
//         + '.bh-item{flex:1}.bh-qty{width:34px;text-align:center}.bh-amt{width:72px;text-align:right}'
//         + '.bill-item-row{display:flex;font-size:11px;padding:4px 0;border-bottom:1px dotted #eee;align-items:center}'
//         + '.bi-name{flex:1;line-height:1.3}.bi-qty{width:34px;text-align:center;color:#888}.bi-amt{width:72px;text-align:right;font-weight:700}'
//         + '.bill-tots{padding-top:9px;margin-top:3px;border-top:1px dashed #ccc}'
//         + '.bill-tot-row{display:flex;justify-content:space-between;font-size:11px;padding:3px 0}'
//         + '.bill-tot-row.grand{font-size:14px;font-weight:700;border-top:2px dashed #ccc;margin-top:5px;padding-top:7px}'
//         + '.bill-footer{text-align:center;margin-top:12px;padding-top:10px;border-top:2px dashed #ccc;font-size:10px;color:#999;line-height:1.7}';
//     var html = '<!DOCTYPE html><html><head><title>Bill</title><style>' + css + '</style></head><body>' + content + '</body></html>';
//     var blob = new Blob([html], { type: 'text/html' });
//     var url = URL.createObjectURL(blob);
//     var w = window.open(url, '_blank', 'width=420,height=650');
//     if (w) {
//         w.onload = function () { w.print(); URL.revokeObjectURL(url); };
//     }
// }

async function printBill() {
    
    var billNo1 = billNo; 
    var customerName = "";
    var mobileNo = "";

    // Agar discount value percentage mein hai toh wo nikalen, nahi toh 0
    // var discountPercent = (typeof discType !== 'undefined' && discType === 'pct') ?
    //     (parseFloat(document.getElementById('discountVal').value) || 0) : 0;

    // var paymentMethod = document.getElementById('payMethod')?.value || "Cash";

    // Validation: Agar bill number hi nahi hai toh aage mat badho
    if (!billNo1) {
        alert("Bill Number nahi mila. Pehle bill generate karein!");
        return;
    }

    try {
        
        let response = await $.ajax({
            url: "/Billing/ProcessBill",
            method: "POST",
            data: {
                billno: billNo1,
                customername: customerName,
                mobileno: mobileNo,
                discountPercent: 0,
                paymentMethod: "paymentMethod"
            }
        });

        // 3. Backend response check karenge
        // Kyunki aapne wahan 'return Ok(...)' ya 'return Json(new { status = false })' likha hai
        if (response.status === false) {
            alert("Error: " + response.message);
            return; // Agar cart khali hai ya koi error hai, toh print nahi hoga
        }

        // 4. AGAR BACKEND SE SUCCESS AAYA, TOH PRINT WALA LOGIC CHALEGA
        alert(response.message); // "Bill INVxxxx successfully bana!" dikahega

        var content = document.getElementById('billPrint').innerHTML;
        var css = '*{box-sizing:border-box;margin:0;padding:0}'
            + 'body{font-family:monospace;padding:20px;font-size:12px;color:#111;max-width:380px;margin:0 auto}'
            + '.bill-top{text-align:center;padding-bottom:12px;border-bottom:2px dashed #ccc;margin-bottom:12px}'
            + '.bill-logo{font-size:20px;font-weight:700}'
            + '.bill-store,.bill-meta{font-size:10px;color:#666;margin-top:3px}'
            + '.bill-items-head{display:flex;font-size:10px;font-weight:700;color:#999;padding:4px 0;border-bottom:1px solid #ddd;margin-bottom:5px}'
            + '.bh-item{flex:1}.bh-qty{width:34px;text-align:center}.bh-amt{width:72px;text-align:right}'
            + '.bill-item-row{display:flex;font-size:11px;padding:4px 0;border-bottom:1px dotted #eee;align-items:center}'
            + '.bi-name{flex:1;line-height:1.3}.bi-qty{width:34px;text-align:center;color:#888}.bi-amt{width:72px;text-align:right;font-weight:700}'
            + '.bill-tots{padding-top:9px;margin-top:3px;border-top:1px dashed #ccc}'
            + '.bill-tot-row{display:flex;justify-content:space-between;font-size:11px;padding:3px 0}'
            + '.bill-tot-row.grand{font-size:14px;font-weight:700;border-top:2px dashed #ccc;margin-top:5px;padding-top:7px}'
            + '.bill-footer{text-align:center;margin-top:12px;padding-top:10px;border-top:2px dashed #ccc;font-size:10px;color:#999;line-height:1.7}';

        var html = '<!DOCTYPE html><html><head><title>Bill</title><style>' + css + '</style></head><body>' + content + '</body></html>';
        var blob = new Blob([html], { type: 'text/html' });
        var url = URL.createObjectURL(blob);
        var w = window.open(url, '_blank', 'width=420,height=650');

        if (w) {
            w.onload = function () {
                w.print();
                URL.revokeObjectURL(url);

                // Print hone ke baad aap chahein toh page reload kar sakte hain taaki cart khali dikhe
                 window.location.reload(); 
            };
        }

    } catch (error) {
        console.error(error);
        alert('Server se connect karne mein galti hui ya transaction fail ho gaya.');
    }
}