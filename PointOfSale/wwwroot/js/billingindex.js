
let cart = [];

$(document).ready(function () {

    loadCart();

    tick();
    setInterval(tick, 30000);

    document.getElementById('searchInput')
        ?.addEventListener('keyup', searchProducts);

    document.getElementById('discountInput')
        ?.addEventListener('input', function () {

            let subtotal = cart.reduce(
                (sum, item) => sum + Number(item.amount || 0),
                0
            );

            updateTotals(subtotal);
        });

    document.querySelectorAll('.cat-btn')
        .forEach(btn => {

            btn.addEventListener('click', function () {

                document.querySelectorAll('.cat-btn')
                    .forEach(x => x.classList.remove('active'));

                this.classList.add('active');

                searchProducts();
            });
        });
});

// Clock
function tick() {

    const now = new Date();

    let h = now.getHours();
    let m = now.getMinutes();

    const ap = h >= 12 ? 'PM' : 'AM';

    h = h % 12 || 12;

    let clock = document.getElementById('liveClock');

    if (clock) {

        clock.innerText =
            `${h}:${String(m).padStart(2, '0')} ${ap}`;
    }
}

// Toast
function showToast(msg, isError = false) {

    let toast = document.getElementById('toast');

    if (!toast) {
        alert(msg);
        return;
    }

    document.getElementById('toastMsg').innerText = msg;

    toast.classList.remove('error');

    if (isError) {
        toast.classList.add('error');
    }

    toast.classList.add('show');

    setTimeout(() => {
        toast.classList.remove('show');
    }, 3000);
}

// Load Cart
async function loadCart() {

    try {

        let response = await fetch('/Billing/GetCartData');

        if (!response.ok) {
            console.error('Failed to load cart');
            return;
        }

        let data = await response.json();

        cart = data.cart || [];

        console.log("Cart Loaded");
        console.table(cart);

        renderCart();
    }
    catch (err) {

        console.error(err);
    }
}

// Add To Cart
async function addToCart(productId) {

    try {

        let response = await fetch('/Billing/AddToCart', {

            method: 'POST',

            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },

            body: `productId=${productId}&qty=1`
        });

        let data = await response.json();

        if (data.status) {

            await loadCart();

            showToast(data.message);
        }
        else {

            showToast(data.message, true);
        }
    }
    catch (err) {

        console.error(err);

        showToast('Error adding item', true);
    }
}

// Update Qty
async function updateQty(productId, type) {

    try {

        let response = await fetch('/Billing/UpdateQty', {

            method: 'POST',

            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },

            body: `productId=${productId}&qty=1&type=${type}`
        });

        let data = await response.json();

        if (data.status) {

            await loadCart();
        }
        else {

            showToast(data.message, true);
        }
    }
    catch (err) {

        console.error(err);
    }
}

// Remove Item
async function removeItem(productId) {

    if (!confirm('Item remove karna hai?'))
        return;

    try {

        let response = await fetch('/Billing/RemoveItem', {

            method: 'POST',

            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },

            body: `productId=${productId}`
        });

        let data = await response.json();

        if (data.status) {

            await loadCart();

            showToast(data.message);
        }
        else {

            showToast(data.message, true);
        }
    }
    catch (err) {

        console.error(err);
    }
}

// Clear Cart
async function clearCart() {

    if (cart.length === 0)
        return;

    if (!confirm('Cart clear karna hai?'))
        return;

    try {

        let response = await fetch('/Billing/ClearCart', {
            method: 'POST'
        });

        let data = await response.json();

        if (data.status) {

            cart = [];

            renderCart();

            showToast('Cart cleared successfully');
        }
    }
    catch (err) {

        console.error(err);
    }
}

// Render Cart
function renderCart() {

    let container = document.getElementById('cartItems');
    let empty = document.getElementById('cartEmpty');

    if (!container)
        return;

    if (!cart || cart.length === 0) {

        container.innerHTML = '';

        if (empty) {
            empty.style.display = 'flex';
        }

        document.getElementById('cartBadge').innerText = '0';

        updateTotals(0);

        return;
    }

    if (empty) {
        empty.style.display = 'none';
    }

    let html = '';
    let subtotal = 0;
    let count = 0;

    cart.forEach(item => {

        subtotal += Number(item.amount || 0);

        count += Number(item.quantity || 0);

        html += `
    <div class="cart-item">

    <div class="cart-item-info">

    <div class="cart-item-name">
    ${item.productName}
    </div>

    <div class="cart-item-price">
    ₹${Number(item.rate || 0).toFixed(2)}
    </div>

    </div>

    <div class="qty-control">

    <button class="qty-btn"
    onclick="updateQty(${item.productId},'decrease')">
    -
    </button>

    <span class="qty-val">
    ${item.quantity}
    </span>

    <button class="qty-btn"
    onclick="updateQty(${item.productId},'increase')">
    +
    </button>

    </div>

    <span class="item-total">
    ₹${Number(item.amount || 0).toFixed(2)}
    </span>

    <button class="btn-remove"
    onclick="removeItem(${item.productId})">

    <i class="ti ti-trash"></i>

    </button>

    </div>
    `;
    });

    container.innerHTML = html;

    document.getElementById('cartBadge').innerText = count;

    updateTotals(subtotal);
}

// Totals
function updateTotals(subtotal) {

    let discountPercent =
        parseFloat(document.getElementById('discountInput')?.value) || 0;

    let discountAmount =
        (subtotal * discountPercent) / 100;

    let total =
        subtotal - discountAmount;

    document.getElementById('subtotalVal').innerText =
        '₹' + subtotal.toFixed(2);

    document.getElementById('totalVal').innerText =
        '₹' + total.toFixed(2);

    if (discountAmount > 0) {

        document.getElementById('discountRow').style.display = 'flex';

        document.getElementById('discountVal').innerText =
            '-₹' + discountAmount.toFixed(2);
    }
    else {

        document.getElementById('discountRow').style.display = 'none';
    }
}



// Process Bill
async function processBill() {

    if (!cart || cart.length === 0) {

        showToast('Cart khaali hai', true);
        return;
    }

    let btn = document.getElementById('processBtn');

    btn.disabled = true;

    btn.innerHTML =
        '<i class="ti ti-loader ti-spin"></i> Processing...';

    let customername =
        document.getElementById('customerName').value.trim();

    let mobileno =
        document.getElementById('mobileNo').value.trim();

    // Default Customer Name
    if (customername === '') {
        customername = 'Walk In';
    }

    // Mobile blank hai to blank hi bhejo
    if (mobileno === '') {
        mobileno = ' ';
    }

    let discountPercent =
        document.getElementById('discountInput').value || 0;

    let paymentMethod =
        document.getElementById('paymentSelect').value;

    let response = await fetch('/Billing/ProcessBill', {

        method: 'POST',

        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },

        body:
            `customername=${encodeURIComponent(customername)}
                &mobileno=${encodeURIComponent(mobileno)}
                &discountPercent=${discountPercent}
                &paymentMethod=${paymentMethod}`
    });

    let data = await response.json();

    btn.disabled = false;

    btn.innerHTML =
        '<i class="ti ti-receipt"></i> Generate Bill';
    // console.log(data);
    // console.log(data.status);
    // console.log(data.invoiceId);
    if (data.status) {

        cart = [];

        // renderCart();

        document.getElementById('customerName').value = '';
        document.getElementById('mobileNo').value = '';
        document.getElementById('discountInput').value = 0;

        showToast(data.message);


        setTimeout(() => {

            window.open(
                '/Billing/PrintBill?invoiceId=' +
                data.invoiceId +
                '&invoiceNumber=' +
                encodeURIComponent(data.invoiceNumber),
                '_blank'
            );

        }, 2000);
    }
    else {

        showToast(data.message, true);
    }
}

// Search Product
async function searchProducts() {

    let q =
        document.getElementById('searchInput').value;

    let activeCategory =
        document.querySelector('.cat-btn.active');

    let category =
        activeCategory ? activeCategory.innerText : 'Sab kuch';

    let response =
        await fetch(`/Billing/SearchProducts?q=${encodeURIComponent(q)}&category=${encodeURIComponent(category)}`);

    let products = await response.json();

    let html = '';

    products.forEach(p => {

        html += `
                <div class="product-card"
                     onclick="addToCart(${p.id})">

                    <span class="p-icon">
                        <img src="${p.imageUrl}"
                             style="height:83px;width:87px;" />
                    </span>

                    <div class="p-name">
                        ${p.productName}
                    </div>

                    <div class="p-code">
                        ${p.productCode}
                    </div>

                    <div class="p-price">
                        ₹${p.saleRate.toFixed(2)}
                    </div>

                    <div class="p-stock">
                        Stock : ${p.stockQuantity}
                    </div>

                    <span class="add-overlay">
                        <i class="ti ti-plus"></i>
                    </span>

                </div>
            `;
    });

    document.getElementById('productGrid').innerHTML = html;
}

document.getElementById('searchInput')
    .addEventListener('keyup', searchProducts);

document.querySelectorAll('.cat-btn')
    .forEach(btn => {

        btn.addEventListener('click', function () {

            document.querySelectorAll('.cat-btn')
                .forEach(x => x.classList.remove('active'));

            this.classList.add('active');

            searchProducts();
        });
    });

