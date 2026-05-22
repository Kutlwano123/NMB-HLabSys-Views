(function () {
    'use strict';

    // ====================================================================
    // 1. APPLICATION SIDEBAR & INTERFACE RESPONSIVENESS LAYER
    // ====================================================================
    function initSidebar() {
        const sidebar = document.getElementById('doctorSidebar');
        const overlay = document.getElementById('sidebarOverlay');
        const toggle = document.getElementById('sidebarToggle');
        if (!sidebar) return;

        function close() {
            sidebar.classList.remove('open');
            overlay?.classList.remove('show');
        }
        toggle?.addEventListener('click', () => {
            sidebar.classList.toggle('open');
            overlay?.classList.toggle('show');
        });
        overlay?.addEventListener('click', close);
    }

    // Runs visual cascade card pop-in load transitions cleanly
    function initStatReveal() {
        document.querySelectorAll('.stat-card').forEach((card, i) => {
            setTimeout(() => card.classList.add('revealed'), 80 + i * 90);
        });
    }

    // ====================================================================
    // 2. LIVE CLOCK ENGINE (Using South African Locales)
    // ====================================================================
    function initClock() {
        const dateEl = document.getElementById('clockDate');
        const timeEl = document.getElementById('clockTime');
        if (!dateEl || !timeEl) return;

        function tick() {
            const now = new Date();
            dateEl.textContent = now.toLocaleDateString('en-ZA', {
                weekday: 'short', day: 'numeric', month: 'short', year: 'numeric'
            });
            timeEl.textContent = now.toLocaleTimeString('en-ZA', {
                hour: '2-digit', minute: '2-digit', second: '2-digit'
            });
        }
        tick();
        setInterval(tick, 1000);
    }

    // ====================================================================
    // 3. SECURE PROFILE RENDERING VIA SERVER-SIDE META DATA
    // ====================================================================
    function initServerProfile() {
        const name = document.body.dataset.doctorName;
        const initials = document.body.dataset.doctorInitials;
        const role = document.body.dataset.doctorRole;
        if (!name && !initials) return;

        if (name) {
            ['sidebarProfileName', 'topbarName'].forEach(id => {
                const el = document.getElementById(id);
                if (el) el.textContent = name;
            });
        }
        if (role) {
            ['sidebarProfileRole', 'topbarRole'].forEach(id => {
                const el = document.getElementById(id);
                if (el) el.textContent = role;
            });
        }
        if (initials) {
            ['sidebarAvatar', 'topbarAvatar'].forEach(id => {
                const el = document.getElementById(id);
                if (el) el.textContent = initials;
            });
        }
    }

    // ====================================================================
    // 4. INTERACTIVE RECORD FILTERING OPERATIONS
    // ====================================================================
    function initStatusFilters() {
        document.querySelectorAll('[data-status-filter]').forEach(bar => {
            const tableId = bar.dataset.statusFilter;
            const table = document.getElementById(tableId);
            if (!table) return;

            bar.querySelectorAll('.filter-btn').forEach(btn => {
                btn.addEventListener('click', () => {
                    bar.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
                    btn.classList.add('active');
                    const status = btn.dataset.status;
                    table.querySelectorAll('tbody tr').forEach(row => {
                        if (status === 'all') {
                            row.style.display = '';
                        } else {
                            row.style.display = row.dataset.status === status ? '' : 'none';
                        }
                    });
                });
            });
        });
    }

    function initAlertFilters() {
        const bar = document.getElementById('alertFilterBar');
        const grid = document.getElementById('alertCardGrid');
        if (!bar || !grid) return;

        const customInputs = document.getElementById('alertCustomRange');
        const fromInput = document.getElementById('alertFromDate');
        const toInput = document.getElementById('alertToDate');

        function daysAgo(n) {
            const d = new Date();
            d.setDate(d.getDate() - n);
            return d;
        }

        bar.querySelectorAll('.filter-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                bar.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
                btn.classList.add('active');

                const range = btn.dataset.range;
                if (range === 'custom') {
                    customInputs?.classList.add('show');
                    return;
                }
                customInputs?.classList.remove('show');

                let from = daysAgo(5);
                if (range === '10') from = daysAgo(10);
                if (range === '30') from = daysAgo(30);

                grid.querySelectorAll('.alert-card').forEach(card => {
                    const cardDate = new Date(card.dataset.date);
                    card.style.display = cardDate >= from ? '' : 'none';
                });
            });
        });

        function applyCustom() {
            if (!fromInput?.value || !toInput?.value) return;
            const from = new Date(fromInput.value);
            const to = new Date(toInput.value);
            to.setHours(23, 59, 59);
            grid.querySelectorAll('.alert-card').forEach(card => {
                const cardDate = new Date(card.dataset.date);
                card.style.display = cardDate >= from && cardDate <= to ? '' : 'none';
            });
        }
        fromInput?.addEventListener('change', applyCustom);
        toInput?.addEventListener('change', applyCustom);
    }

    // ====================================================================
    // 5. TOAST NOTIFICATION UTILITY LAYERS
    // ====================================================================
    function showToast(msg) {
        let toast = document.getElementById('doctorToast');
        if (!toast) {
            toast = document.createElement('div');
            toast.id = 'doctorToast';
            toast.className = 'toast';
            document.body.appendChild(toast);
        }
        toast.textContent = msg;
        toast.classList.add('show');
        clearTimeout(showToast._t);
        showToast._t = setTimeout(() => toast.classList.remove('show'), 2800);
    }

    // Flash backend TempData success strings cleanly directly to user layout context
    function handleTempDataMessages() {
        const successMessage = document.body.dataset.toastSuccess;
        if (successMessage) {
            showToast(successMessage);
        }
    }

    // ====================================================================
    // INITIALIZATION PIPELINE OVER THE WIRE
    // ====================================================================
    document.addEventListener('DOMContentLoaded', () => {
        initServerProfile();
        initClock();
        initSidebar();
        initStatReveal();
        initStatusFilters();
        initAlertFilters();
        handleTempDataMessages();
    });
})();