// Scripts de base pour le site

// Confirmation avant suppression
function confirmDelete(message) {
    return confirm(message || "Êtes-vous sûr de vouloir supprimer cet élément ?");
}

// Afficher/cacher les mots de passe
function togglePasswordVisibility(inputId) {
    var input = document.getElementById(inputId);
    if (input.type === "password") {
        input.type = "text";
    } else {
        input.type = "password";
    }
}

// Message automatique temporaire
function showTempMessage(message, type, duration = 3000) {
    var alertDiv = document.createElement('div');
    alertDiv.className = 'alert alert-' + type + ' alert-dismissible fade show position-fixed';
    alertDiv.style.top = '20px';
    alertDiv.style.right = '20px';
    alertDiv.style.zIndex = '9999';
    alertDiv.innerHTML = message + '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>';

    document.body.appendChild(alertDiv);

    setTimeout(function () {
        alertDiv.remove();
    }, duration);
}

// Initialisation au chargement
document.addEventListener('DOMContentLoaded', function () {
    // Activer les tooltips Bootstrap
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Masquer les messages après 5 secondes
    setTimeout(function () {
        var alerts = document.querySelectorAll('.alert:not(.alert-permanent)');
        alerts.forEach(function (alert) {
            var bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        });
    }, 5000);
});