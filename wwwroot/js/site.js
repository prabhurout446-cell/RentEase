// RentEase - site.js

// Unread message badge
async function updateUnreadBadge() {
    try {
        const resp = await fetch('/Chat/UnreadCount');
        if (resp.ok) {
            const count = await resp.json();
            const badge = document.getElementById('unreadBadge');
            if (badge) {
                if (count > 0) {
                    badge.textContent = count;
                    badge.classList.remove('d-none');
                } else {
                    badge.classList.add('d-none');
                }
            }
        }
    } catch (e) { /* ignore */ }
}

document.addEventListener('DOMContentLoaded', function () {
    // Update unread badge if logged in
    if (document.getElementById('unreadBadge')) {
        updateUnreadBadge();
        setInterval(updateUnreadBadge, 30000);
    }

    // Auto-dismiss alerts
    setTimeout(() => {
        document.querySelectorAll('.alert-dismissible').forEach(el => {
            const bsAlert = bootstrap.Alert.getOrCreateInstance(el);
            if (bsAlert) bsAlert.close();
        });
    }, 5000);
});
