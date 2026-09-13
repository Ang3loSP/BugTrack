document.addEventListener('DOMContentLoaded', function () {
    const alerts = document.querySelectorAll('.alert:not(.alert-danger)');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity 0.5s ease';
            alert.style.opacity = '0';
            setTimeout(() => alert.remove(), 500);
        }, 5000);
    });

    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    const nav = document.querySelector('.app-nav');
    if (nav) {
        const onScroll = () => {
            nav.classList.toggle('is-scrolled', window.scrollY > 8);
        };
        window.addEventListener('scroll', onScroll, { passive: true });
        onScroll();
    }

    if (window.matchMedia('(pointer: fine)').matches) {
        document.querySelectorAll('.card, .stat-tile, .glass-panel').forEach(el => {
            el.addEventListener('pointermove', e => {
                const rect = el.getBoundingClientRect();
                el.style.setProperty('--lx', ((e.clientX - rect.left) / rect.width * 100).toFixed(1) + '%');
                el.style.setProperty('--ly', ((e.clientY - rect.top) / rect.height * 100).toFixed(1) + '%');
            });
        });
    }
});

function confirmDelete(message) {
    return confirm(message || 'Are you sure you want to delete this item?');
}

function getStatusBadgeClass(status) {
    const classes = {
        'New': 'bg-primary',
        'InProgress': 'bg-warning',
        'Fixed': 'bg-info',
        'Retest': 'bg-warning',
        'Closed': 'bg-success',
        'Reopened': 'bg-danger'
    };
    return classes[status] || 'bg-secondary';
}

function getPriorityBadgeClass(priority) {
    const classes = {
        'Low': 'bg-success',
        'Medium': 'bg-info',
        'High': 'bg-warning',
        'Critical': 'bg-danger'
    };
    return classes[priority] || 'bg-secondary';
}

function getResultBadgeClass(result) {
    const classes = {
        'Pass': 'bg-success',
        'Fail': 'bg-danger',
        'Blocked': 'bg-warning',
        'NotRun': 'bg-secondary'
    };
    return classes[result] || 'bg-secondary';
}