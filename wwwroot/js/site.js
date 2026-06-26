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