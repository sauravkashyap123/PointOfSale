// ── GLOBALS & CONFIGURATION ──
const ROLE_CFG = {
    staff: {
        title: 'Point of Sale',
        sub: 'Staff login — POS billing & sales',
        color: 'var(--orange)'
    },
    admin: {
        title: 'Admin Dashboard',
        sub: 'Full system access & management',
        color: '#DC2626'
    },
    warehouse: {
        title: 'Warehouse Portal',
        sub: 'Inventory & stock management',
        color: 'var(--blue-light)'
    }
};

let currentRole = 'staff';
let authMode = 'pwd';
let pinBuf = '';

// ── INITIALIZATION ──
document.addEventListener('DOMContentLoaded', () => {
    // Start Clock
    updateClock();
    setInterval(updateClock, 1000);

    // Bind Keyboard events for PIN entry
    document.addEventListener('keydown', handleKeyboardInput);

    // Initial Focus
    const userIdInput = document.getElementById('userId');
    if (userIdInput) userIdInput.focus();
});

// ── CLOCK ──
function updateClock() {
    const clockEl = document.getElementById('statusClock');
    if (!clockEl) return;

    const n = new Date();
    let h = n.getHours(), m = n.getMinutes(), s = n.getSeconds(), ap = h >= 12 ? 'PM' : 'AM';
    h = h % 12 || 12;

    clockEl.textContent = `${String(h).padStart(2, '0')}:${String(m).padStart(2, '0')}:${String(s).padStart(2, '0')} ${ap}`;
}

// ── SELECT ROLE ──
function selectRole(role) {
    currentRole = role;

    // Manage tab active classes
    document.querySelectorAll('.role-tab').forEach(t => t.classList.remove('active'));
    const targetedTab = document.getElementById('tab-' + role);
    if (targetedTab) targetedTab.classList.add('active');

    // Update structural text branding
    const cfg = ROLE_CFG[role];
    if (cfg) {
        document.getElementById('cardTitle').textContent = cfg.title;
        document.getElementById('cardSub').textContent = cfg.sub;
    }

    clearAlert();
    pinBuf = '';
    updatePinDots();
}

// ── AUTH MODE SWITCH ──
function switchAuth(mode) {
    authMode = mode;

    document.getElementById('btnPwd').classList.toggle('active', mode === 'pwd');
    document.getElementById('btnPin').classList.toggle('active', mode === 'pin');

    document.getElementById('pwdForm').style.display = mode === 'pwd' ? 'block' : 'none';
    document.getElementById('pinForm').style.display = mode === 'pin' ? 'block' : 'none';
    document.getElementById('signinBtn').style.display = mode === 'pwd' ? 'flex' : 'none';

    clearAlert();
    pinBuf = '';
    updatePinDots();
}

// ── TOGGLE PASSWORD VISIBILITY ──
function togglePwd() {
    const inp = document.getElementById('userPwd');
    const icon = document.getElementById('eyeIcon');
    if (!inp || !icon) return;

    if (inp.type === 'password') {
        inp.type = 'text';
        icon.innerHTML = '<i class="ti ti-eye-off"></i>';
    } else {
        inp.type = 'password';
        icon.innerHTML = '<i class="ti ti-eye"></i>';
    }
}

// ── UI ALERTS ──
function showAlert(type, msg) {
    const box = document.getElementById('alertBox');
    if (!box) return;

    box.className = `alert-box alert-${type} show`;
    document.getElementById('alertIcon').className = type === 'error' ? 'ti ti-alert-circle' : 'ti ti-circle-check';
    document.getElementById('alertText').textContent = msg;
}

function clearAlert() {
    const box = document.getElementById('alertBox');
    if (box) box.className = 'alert-box';
}

// ── INTERACTIVE PIN PADS ──
function pinPress(k) {
    if (k === 'del') {
        if (pinBuf.length > 0) pinBuf = pinBuf.slice(0, -1);
    } else if (pinBuf.length < 4) {
        pinBuf += k;
    }
    updatePinDots();
}

function updatePinDots() {
    for (let i = 0; i < 4; i++) {
        const d = document.getElementById('pd' + i);
        if (d) d.classList.toggle('filled', i < pinBuf.length);
    }
}

// ── KEYBOARD INTERCEPTOR ──
function handleKeyboardInput(e) {
    if (authMode !== 'pin') return;
    if (e.key >= '0' && e.key <= '9') pinPress(e.key);
    else if (e.key === 'Backspace') pinPress('del');
    else if (e.key === 'Enter') doPinLogin();
}

// ── BACKEND API INTEGRATIONS ──

// Standard Password Login
async function doLogin() {
    debugger;
    const id = document.getElementById('userId').value.trim();
    const pwd = document.getElementById('userPwd').value;
    const rememberMe = document.getElementById('rememberMe').checked;

    if (!id) { showAlert('error', 'Please enter your User ID.'); document.getElementById('userId').focus(); return; }
    if (!pwd) { showAlert('error', 'Please enter your password.'); document.getElementById('userPwd').focus(); return; }

    clearAlert();

    const btn = document.getElementById('signinBtn');
    const nativeBtnHtml = btn.innerHTML;
    btn.innerHTML = '<div class="spinner"></div> Signing in...';
    btn.classList.add('loading');

    try {
        // Formulating the LoginModel object matching C# signature
        const response = await fetch('/Account/Login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Username: id, Password: pwd, Role: currentRole, RememberMe: rememberMe })
        });

        if (!response.ok) throw new Error('Server connection lost.');

        const data = await response.json();

        if (data.result) {
            showAlert('success', data.message || 'Welcome! Redirecting...');
            btn.innerHTML = '<i class="ti ti-circle-check"></i> Success!';
            setTimeout(() => {
                window.location.href = data.redirect || '/Home/Index';
            }, 1000);
        } else {
            showAlert('error', data.message || 'Invalid User ID or Password. Please try again.');
            resetPasswordBtn(btn, nativeBtnHtml);
        }
    } catch (err) {
        showAlert('error', 'An internal runtime exception occurred. Please try again.');
        resetPasswordBtn(btn, nativeBtnHtml);
    }
}

function resetPasswordBtn(btn, defaultHtml) {
    btn.innerHTML = defaultHtml;
    btn.classList.remove('loading');
    const pwdInput = document.getElementById('userPwd');
    if (pwdInput) {
        pwdInput.value = '';
        pwdInput.focus();
    }
}

// Quick PIN Login
async function doPinLogin() {
    const id = document.getElementById('pinUserId').value.trim();
    if (!id) { showAlert('error', 'Please enter your User ID.'); return; }
    if (pinBuf.length < 4) { showAlert('error', 'Please enter your 4-digit PIN.'); return; }

    clearAlert();

    try {
        const response = await fetch('/Account/PinLogin', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ Username: id, Pin: pinBuf, Role: currentRole })
        });

        if (!response.ok) throw new Error('Server connection lost.');

        const data = await response.json();

        if (data.result) {
            showAlert('success', data.message || 'Access Authorized! Redirecting...');
            setTimeout(() => {
                window.location.href = data.redirect || '/Home/Index';
            }, 1000);
        } else {
            showAlert('error', data.message || 'Invalid Quick Login PIN. Please try again.');
            pinBuf = '';
            updatePinDots();
        }
    } catch (err) {
        showAlert('error', 'Could not establish connection with authentication service.');
        pinBuf = '';
        updatePinDots();
    }
}

// Mock Password Recovery Functionality
function doForgot() {
    showAlert('success', 'Password reset context dispatched successfully to your registered layout.');
}