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
        const response = await fetch('/Billing/GetShopWiseAllProducts', { method: 'GET' });
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

// ── ADD TO ORDER BY SKU (barcode scan ke baad) ──
function addToOrderBySku(sku) {
    var p = null;
    for (var i = 0; i < products.length; i++) {
        if (products[i].sku && products[i].sku.toLowerCase() === sku.toLowerCase()) {
            p = products[i];
            break;
        }
    }
    if (!p) {
        showScanFeedback('notfound', 'Product nahi mila: ' + sku);
        return;
    }
    if (p.stock === 0) {
        showScanFeedback('notfound', p.name + ' — Out of Stock');
        return;
    }
    showScanFeedback('found', p.name + ' — Added!');
    addToOrder(p.id);
}

// ── CHANGE QTY (+/-) ──
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

// ── SET QTY DIRECTLY FROM INPUT ──
function setQtyDirect(id, val) {
    var newQty = parseInt(val);
    if (!order[id]) return;

    if (isNaN(newQty) || newQty < 1) {
        var inp = document.getElementById('qtyInput_' + id);
        if (inp) inp.value = order[id].qty;
        return;
    }

    if (order[id].stock && order[id].stock !== 999 && newQty > order[id].stock) {
        alert('Stock mein sirf ' + order[id].stock + ' available hai.');
        var inp = document.getElementById('qtyInput_' + id);
        if (inp) inp.value = order[id].qty;
        return;
    }

    var type = newQty > order[id].qty ? 'increase' : 'decrease';

    $.ajax({
        url: '/Billing/UpdateQty',
        method: 'POST',
        data: { productId: id, qty: newQty, type: type },
        success: function (response) {
            if (response.status) {
                order[id].qty = newQty;
                syncCartFromResponse(response.cart);
            } else {
                alert(response.message || 'Update failed.');
                var inp = document.getElementById('qtyInput_' + id);
                if (inp) inp.value = order[id].qty;
            }
        },
        error: function () {
            alert('Something went wrong.');
            var inp = document.getElementById('qtyInput_' + id);
            if (inp) inp.value = order[id].qty;
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
                + '<input type="number" '
                + 'id="qtyInput_' + it.id + '" '
                + 'value="' + it.qty + '" '
                + 'min="1" '
                + (it.stock && it.stock !== 999 ? 'max="' + it.stock + '" ' : '')
                + 'style="width:52px;text-align:center;border:1px solid var(--border);border-radius:6px;padding:3px 4px;font-size:13px;font-weight:600;background:var(--bg);color:var(--text);-moz-appearance:textfield;" '
                + 'onchange="setQtyDirect(' + it.id + ', this.value)" '
                + 'onkeydown="if(event.key===\'Enter\') this.blur()" '
                + '>'
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

// ════════════════════════════════════════════════════
// ── BARCODE SCANNER (Camera) ──
// ════════════════════════════════════════════════════

var scannerModal = null;
var scannerStream = null;
var scannerAnimFrame = null;
var scannerActive = false;
var lastScannedCode = '';
var lastScannedTime = 0;

// Scan button click — modal inject karke camera start karo
function openScanner() {
    // Pehle se modal hai toh remove karo
    var existing = document.getElementById('scannerModal');
    if (existing) existing.remove();

    // Modal HTML inject
    var modal = document.createElement('div');
    modal.id = 'scannerModal';
    modal.style.cssText = 'position:fixed;inset:0;z-index:9999;background:rgba(0,0,0,0.85);display:flex;align-items:center;justify-content:center;flex-direction:column;';
    modal.innerHTML = ''
        + '<div style="position:relative;width:320px;max-width:95vw;">'
        + '  <div style="display:flex;align-items:center;justify-content:space-between;margin-bottom:10px;">'
        + '    <span style="color:#fff;font-size:15px;font-weight:600;"><i class="ti ti-barcode" style="margin-right:6px"></i>Barcode Scan Karo</span>'
        + '    <button onclick="closeScanner()" style="background:rgba(255,255,255,0.15);border:none;color:#fff;border-radius:50%;width:32px;height:32px;cursor:pointer;font-size:18px;display:flex;align-items:center;justify-content:center;">&times;</button>'
        + '  </div>'
        + '  <div style="position:relative;border-radius:12px;overflow:hidden;background:#000;">'
        + '    <video id="scannerVideo" autoplay playsinline muted style="width:100%;display:block;border-radius:12px;"></video>'
        + '    <canvas id="scannerCanvas" style="display:none;"></canvas>'
        + '    <!-- scanner line animation -->'
        + '    <div style="position:absolute;inset:0;pointer-events:none;">'
        + '      <div id="scanLine" style="position:absolute;left:10%;right:10%;height:2px;background:linear-gradient(90deg,transparent,#22c55e,transparent);top:40%;animation:scanAnim 1.8s ease-in-out infinite;"></div>'
        + '      <div style="position:absolute;top:14px;left:14px;width:24px;height:24px;border-top:3px solid #22c55e;border-left:3px solid #22c55e;border-radius:3px 0 0 0;"></div>'
        + '      <div style="position:absolute;top:14px;right:14px;width:24px;height:24px;border-top:3px solid #22c55e;border-right:3px solid #22c55e;border-radius:0 3px 0 0;"></div>'
        + '      <div style="position:absolute;bottom:14px;left:14px;width:24px;height:24px;border-bottom:3px solid #22c55e;border-left:3px solid #22c55e;border-radius:0 0 0 3px;"></div>'
        + '      <div style="position:absolute;bottom:14px;right:14px;width:24px;height:24px;border-bottom:3px solid #22c55e;border-right:3px solid #22c55e;border-radius:0 0 3px 0;"></div>'
        + '    </div>'
        + '  </div>'
        + '  <div id="scanStatus" style="text-align:center;color:#94a3b8;font-size:13px;margin-top:10px;min-height:22px;">Camera shuru ho rahi hai...</div>'
        + '  <div id="scanFeedback" style="margin-top:8px;min-height:36px;"></div>'
        + '</div>'
        + '<style>'
        + '@keyframes scanAnim{0%{top:15%}50%{top:75%}100%{top:15%}}'
        + '</style>';

    document.body.appendChild(modal);
    scannerModal = modal;

    // ZXing library CDN se load karo (agar pehle load nahi hua)
    if (typeof ZXing === 'undefined') {
        var script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/@zxing/library@0.19.1/umd/index.min.js';
        script.onload = function () { startCamera(); };
        script.onerror = function () {
            document.getElementById('scanStatus').textContent = 'Scanner library load nahi hui.';
        };
        document.head.appendChild(script);
    } else {
        startCamera();
    }
}

function startCamera() {
    var video = document.getElementById('scannerVideo');
    if (!video) return;

    scannerActive = true;
    lastScannedCode = '';
    lastScannedTime = 0;

    navigator.mediaDevices.getUserMedia({
        video: { facingMode: { ideal: 'environment' }, width: { ideal: 1280 }, height: { ideal: 720 } }
    }).then(function (stream) {
        scannerStream = stream;
        video.srcObject = stream;
        video.onloadedmetadata = function () {
            video.play();
            document.getElementById('scanStatus').textContent = 'Barcode camera ke saamne rakho...';
            startDecoding();
        };
    }).catch(function (err) {
        console.error(err);
        var msg = 'Camera access nahi mila.';
        if (err.name === 'NotAllowedError') msg = 'Camera permission deny ki hai. Browser settings se allow karo.';
        else if (err.name === 'NotFoundError') msg = 'Koi camera nahi mila device mein.';
        document.getElementById('scanStatus').textContent = msg;
    });
}

function startDecoding() {
    var video = document.getElementById('scannerVideo');
    var canvas = document.getElementById('scannerCanvas');
    if (!video || !canvas) return;

    var hints = new Map();
    var formats = [
        ZXing.BarcodeFormat.EAN_13,
        ZXing.BarcodeFormat.EAN_8,
        ZXing.BarcodeFormat.CODE_128,
        ZXing.BarcodeFormat.CODE_39,
        ZXing.BarcodeFormat.QR_CODE,
        ZXing.BarcodeFormat.UPC_A,
        ZXing.BarcodeFormat.UPC_E,
        ZXing.BarcodeFormat.DATA_MATRIX
    ];
    hints.set(ZXing.DecodeHintType.POSSIBLE_FORMATS, formats);
    hints.set(ZXing.DecodeHintType.TRY_HARDER, true);

    var reader = new ZXing.MultiFormatReader();
    reader.setHints(hints);

    var ctx = canvas.getContext('2d');

    function tick() {
        if (!scannerActive) return;
        if (video.readyState === video.HAVE_ENOUGH_DATA) {
            canvas.width = video.videoWidth;
            canvas.height = video.videoHeight;
            ctx.drawImage(video, 0, 0, canvas.width, canvas.height);

            try {
                var imgData = ctx.getImageData(0, 0, canvas.width, canvas.height);
                var luminanceSource = new ZXing.HTMLCanvasElementLuminanceSource(canvas);
                var binaryBitmap = new ZXing.BinaryBitmap(new ZXing.HybridBinarizer(luminanceSource));
                var result = reader.decode(binaryBitmap);

                if (result) {
                    var code = result.getText();
                    var now = Date.now();
                    // Same code 2 sec mein dobara mat process karo
                    if (code !== lastScannedCode || (now - lastScannedTime) > 2000) {
                        lastScannedCode = code;
                        lastScannedTime = now;
                        onBarcodeScanned(code);
                    }
                }
            } catch (e) {
                // decode nahi hua — next frame try karo
            }
        }
        scannerAnimFrame = requestAnimationFrame(tick);
    }

    scannerAnimFrame = requestAnimationFrame(tick);
}

function onBarcodeScanned(code) {
    document.getElementById('scanStatus').textContent = 'Scanned: ' + code;
    // SKU match karo products mein
    addToOrderBySku(code);
}

function showScanFeedback(type, msg) {
    var el = document.getElementById('scanFeedback');
    if (!el) return;
    var color = type === 'found' ? '#22c55e' : '#ef4444';
    var icon = type === 'found' ? 'ti-circle-check' : 'ti-circle-x';
    el.innerHTML = '<div style="background:' + color + '22;border:1px solid ' + color + ';border-radius:8px;padding:8px 12px;color:' + color + ';font-size:13px;font-weight:600;display:flex;align-items:center;gap:8px;">'
        + '<i class="ti ' + icon + '" style="font-size:16px"></i>' + msg + '</div>';

    // 2.5 sec baad feedback clear karo
    setTimeout(function () {
        var el2 = document.getElementById('scanFeedback');
        if (el2) el2.innerHTML = '';
    }, 2500);
}

function closeScanner() {
    scannerActive = false;

    if (scannerAnimFrame) {
        cancelAnimationFrame(scannerAnimFrame);
        scannerAnimFrame = null;
    }
    if (scannerStream) {
        scannerStream.getTracks().forEach(function (t) { t.stop(); });
        scannerStream = null;
    }
    var modal = document.getElementById('scannerModal');
    if (modal) modal.remove();
    scannerModal = null;
}

// ESC se bhi scanner band ho
document.addEventListener('keydown', function (e) {
    if (e.key === 'Escape' && scannerActive) closeScanner();
});

// ════════════════════════════════════════════════════
// ── OPEN BILL ──
// ════════════════════════════════════════════════════

let billNo = '';
async function openBill() {
    var items = Object.values(order);
    if (!items.length) { alert('Please add items to the order first.'); return; }
    closeCartDrawer();

    var now = new Date();
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
    var days = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

    try {
        let response = await $.ajax({ url: "/Billing/GenerateBillNo", method: "POST" });
        billNo = response.billno || response;
    } catch (error) {
        alert('Failed to generate bill no.');
        return;
    }

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
}

function closeBill() { document.getElementById('billModal').style.display = 'none'; }
function closeBillOutside(e) { if (e.target === document.getElementById('billModal')) closeBill(); }

// ── PRINT BILL ──
async function printBill() {
    var billNo1 = billNo;
    var customerName = "";
    var mobileNo = "";

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
                paymentMethod: "Cash"
            }
        });

        if (response.status === false) {
            alert("Error: " + response.message);
            return;
        }

        alert(response.message);

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
                window.location.reload();
            };
        }

    } catch (error) {
        console.error(error);
        alert('Server se connect karne mein galti hui ya transaction fail ho gaya.');
    }
}

// ── HARDWARE BARCODE SCANNER GLOBAL LISTENER ──
var barcodeBuffer = '';
var lastKeyTime = 0;

$(document).on('keydown', function (e) {
    var target = e.target;
    if (target.tagName === 'INPUT' || target.tagName === 'TEXTAREA' || target.tagName === 'SELECT') {
        return;
    }

    var now = Date.now();
    if (now - lastKeyTime > 80) {
        barcodeBuffer = '';
    }
    lastKeyTime = now;

    if (e.key === 'Enter') {
        if (barcodeBuffer.length >= 3) {
            addToOrderBySku(barcodeBuffer.trim());
            barcodeBuffer = '';
            e.preventDefault();
        }
    } else if (e.key.length === 1) {
        barcodeBuffer += e.key;
    }
});

// ── BULK ORDER MODAL FUNCTIONS ──
function openBulkOrderModal() {
    var items = Object.values(order);
    if (items.length === 0) {
        alert("Pehle cart mein items add karein bulk order ke liye.");
        return;
    }
    
    var sub = 0;
    for (var i = 0; i < items.length; i++) {
        sub += items[i].price * items[i].qty;
    }
    
    var dv = parseFloat(document.getElementById('discountVal').value) || 0;
    var disc = discType === 'pct' ? sub * (dv / 100) : Math.min(dv, sub);
    var grand = Math.max(0, sub - disc);
    
    document.getElementById('boTotalAmount').value = grand.toFixed(2);
    document.getElementById('boAdvanceAmount').value = '0';
    document.getElementById('boCustomerName').value = '';
    document.getElementById('boContactNumber').value = '';
    document.getElementById('boRemarks').value = '';
    
    var tom = new Date();
    tom.setDate(tom.getDate() + 1);
    tom.setHours(12, 0, 0, 0);
    var localTom = tom.toISOString().slice(0, 16);
    document.getElementById('boDeliveryDate').value = localTom;
    
    document.getElementById('bulkOrderModal').style.display = 'flex';
}

function closeBulkOrderModal() {
    document.getElementById('bulkOrderModal').style.display = 'none';
}

function closeBulkOrderOutside(e) {
    if (e.target.id === 'bulkOrderModal') {
        closeBulkOrderModal();
    }
}

function submitBulkOrder() {
    var custName = document.getElementById('boCustomerName').value.trim();
    var custContact = document.getElementById('boContactNumber').value.trim();
    var deliveryDate = document.getElementById('boDeliveryDate').value;
    var advance = parseFloat(document.getElementById('boAdvanceAmount').value) || 0;
    var total = parseFloat(document.getElementById('boTotalAmount').value) || 0;
    var remarks = document.getElementById('boRemarks').value.trim();

    if (!custName || !custContact || !deliveryDate) {
        alert("Customer Name, Contact, aur Delivery Date bharna zaroori hai!");
        return;
    }

    var orderItems = Object.values(order).map(it => {
        return {
            ProductId: it.id,
            Quantity: it.qty,
            Rate: it.price,
            Amount: it.price * it.qty
        };
    });

    var payload = {
        CustomerName: custName,
        ContactNumber: custContact,
        DeliveryDate: new Date(deliveryDate).toISOString(),
        AdvanceAmount: advance,
        TotalAmount: total,
        Remarks: remarks,
        Items: orderItems
    };

    $.ajax({
        url: '/Billing/CreateBulkOrder',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (response) {
            if (response.success) {
                alert(response.message);
                closeBulkOrderModal();
                clearOrder();
            } else {
                alert(response.message || 'Bulk Order fail ho gaya.');
            }
        },
        error: function () {
            alert('Something went wrong while saving bulk order.');
        }
    });
}
