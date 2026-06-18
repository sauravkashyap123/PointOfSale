

// ── BARCODE SCANNER LOGIC ──────────────────────
let html5QrcodeScanner = null;
const scannerContainer = document.getElementById('scanner-container');
const toggleScannerBtn = document.getElementById('toggle-scanner-btn');
const productCodeInput = document.getElementById('product-code-input');
const productNameInput = document.getElementById('product-name-input'); // Product Name input field

toggleScannerBtn.addEventListener('click', function () {
    if (html5QrcodeScanner && scannerContainer.style.display !== 'none') {
        stopScanner();
        return;
    }

    scannerContainer.style.display = 'block';
    toggleScannerBtn.innerHTML = '✕ Stop';
    toggleScannerBtn.style.backgroundColor = '#dc2626';

    const formatsToSupport = [
        Html5QrcodeSupportedFormats.EAN_13,
        Html5QrcodeSupportedFormats.EAN_8,
        Html5QrcodeSupportedFormats.UPC_A,
        Html5QrcodeSupportedFormats.UPC_E,
        Html5QrcodeSupportedFormats.CODE_128,
        Html5QrcodeSupportedFormats.QR_CODE
    ];

    html5QrcodeScanner = new Html5QrcodeScanner(
        "scanner-container",
        {
            fps: 15,
            qrbox: { width: 280, height: 150 },
            aspectRatio: 1.0,
            formatsToSupport: formatsToSupport
        },
    /* verbose= */ false
    );

    async function onScanSuccess(decodedText, decodedResult) {
        productCodeInput.value = decodedText; // 1. Product Code fill ho gaya
        stopScanner(); // Camera band kiya

        showToast('success', '⏳ Fetching product details...');

        // ── 2. Usi Barcode se International Database (Open Food Facts) se Name nikalna ──
        try {
            // Yeh free API hai jo barcode se product ka naam dhoondti hai
            const url = `https://world.openfoodfacts.org/api/v0/product/${decodedText}.json`;
            const response = await fetch(url);

            if (response.ok) {
                const data = await response.json();

                if (data.status === 1 && data.product) {
                    // Agar product mil gaya toh uska naam (English ya Generic name) set karein
                    const pName = data.product.product_name || data.product.product_name_en || data.product.generic_name || "";

                    if (pName) {
                        productNameInput.value = pName; // Product Name fill ho gaya!
                        showToast('success', '✅ Barcode & Product Name autofilled!');
                    } else {
                        showToast('error', '⚠ Barcode scanned, but product name not found in public database.');
                    }
                } else {
                    showToast('error', '⚠ New Barcode! Product details not found online. Please enter manually.');
                }
            } else {
                showToast('error', '❌ Could not connect to barcode lookup service.');
            }
        } catch (err) {
            console.error("API Error: ", err);
            showToast('error', '❌ Error fetching product name online.');
        }
    }

    function onScanFailure(error) {
        // Console clean rakhne ke liye
    }

    html5QrcodeScanner.render(onScanSuccess, onScanFailure);
});

function stopScanner() {
    if (html5QrcodeScanner) {
        html5QrcodeScanner.clear().then(() => {
            scannerContainer.style.display = 'none';
            toggleScannerBtn.innerHTML = '📷 Scan';
            toggleScannerBtn.style.backgroundColor = '#4f46e5';
        }).catch(err => console.error("Failed to clear scanner", err));
    }
}

// ── FORM SUBMIT AJAX LOGIC ──────────────────────
document.getElementById('productForm').addEventListener('submit', async function (e) {
    e.preventDefault();
    const saveBtn = document.querySelector('.btn-primary');
    const originalText = saveBtn.innerHTML;
    saveBtn.disabled = true;
    saveBtn.innerHTML = '⏳ &nbsp;Saving...';

    try {
        const formData = new FormData(this);
        const response = await fetch('/Home/Addproduct', {
            method: 'POST',
            body: formData
        });

        if (!response.ok) throw new Error('Server error: ' + response.status);
        const result = await response.json();

        if (result.result) {
            showToast('success', '✅ Product saved successfully!');
            setTimeout(() => {
                window.location.href = '/Home/ProductList';
            }, 1500);
        } else {
            showToast('error', '❌ ' + result.message);
            saveBtn.disabled = false;
            saveBtn.innerHTML = originalText;
        }
    } catch (err) {
        showToast('error', '❌ Something went wrong: ' + err.message);
        saveBtn.disabled = false;
        saveBtn.innerHTML = originalText;
    }
});

function showToast(type, message) {
    const existing = document.getElementById('ajaxToast');
    if (existing) existing.remove();

    const toast = document.createElement('div');
    toast.id = 'ajaxToast';
    toast.style.cssText = `
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 9999;
    padding: 14px 20px;
    border-radius: 10px;
    font-size: 14px;
    font-weight: 500;
    box-shadow: 0 4px 16px rgba(0,0,0,0.12);
    animation: toastIn 0.3s ease;
    max-width: 320px;
    ${type === 'success'
            ? 'background:#d1fae5; color:#065f46; border:1px solid #6ee7b7;'
            : 'background:#fee2e2; color:#991b1b; border:1px solid #fca5a5;'}
    `;
    toast.innerHTML = message;
    document.body.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transition = 'opacity 0.3s ease';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

