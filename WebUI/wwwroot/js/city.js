// City Management JavaScript

let currentCityId = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    initializeCityManagement();
});

function initializeCityManagement() {
    setupEventListeners();
}

// Setup event listeners
function setupEventListeners() {
    // Add new city button
    const btnAddCity = document.getElementById('btnAddCity');
    if (btnAddCity) {
        btnAddCity.addEventListener('click', function () {
            openCreateModal();
        });
    }
}

// Open Create Modal
function openCreateModal() {
    fetch('/City/CreateForm')
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('formModalContainer');
            if (container) {
                container.innerHTML = html;
            } else {
                // If container doesn't exist, create it
                const div = document.createElement('div');
                div.id = 'formModalContainer';
                div.innerHTML = html;
                document.body.appendChild(div);
            }
            
            // Re-attach form submission listener
            const form = document.getElementById('cityForm');
            if (form) {
                form.removeEventListener('submit', submitCityForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitCityForm();
                });
            }
            
            const modal = new bootstrap.Modal(document.getElementById('formModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Edit city
function editCity(cityId) {
    fetch(`/City/EditForm?id=${cityId}`)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('formModalContainer');
            if (container) {
                container.innerHTML = html;
            } else {
                const div = document.createElement('div');
                div.id = 'formModalContainer';
                div.innerHTML = html;
                document.body.appendChild(div);
            }
            
            // Re-attach form submission listener
            const form = document.getElementById('cityForm');
            if (form) {
                form.removeEventListener('submit', submitCityForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitCityForm();
                });
            }
            
            const modal = new bootstrap.Modal(document.getElementById('formModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Submit city form
function submitCityForm() {
    const cityId = document.getElementById('cityId').value;
    const regionId = document.getElementById('cityRegion').value;
    const name = document.getElementById('cityName').value.trim();
    const isActive = document.getElementById('cityIsActive').checked;

    // Validation
    if (!regionId) {
        showAlert('Lütfen bir bölge seçin', 'error');
        document.getElementById('cityRegion').focus();
        return;
    }

    if (!name) {
        showAlert('Şehir adı boş olamaz', 'error');
        document.getElementById('cityName').focus();
        return;
    }

    const url = cityId && parseInt(cityId) > 0 ? `/City/Update/${cityId}` : '/City/CreateJson';
    const formData = new FormData();
    formData.append('id', cityId || '0');
    formData.append('regionId', regionId);
    formData.append('name', name);
    formData.append('isActive', isActive);

    fetch(url, {
        method: 'POST',
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Close modal
            const modal = bootstrap.Modal.getInstance(document.getElementById('formModal'));
            if (modal) modal.hide();
            
            showAlert(data.message || 'İşlem başarılı', 'success');
            // Reload page to reflect changes
            setTimeout(() => window.location.reload(), 1000);
        } else {
            showAlert(data.message || 'Bir hata oluştu', 'error');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('İşlem sırasında bir hata oluştu', 'error');
    });
}

// Delete city (show confirmation modal)
function deleteCity(cityId) {
    fetch(`/City/DeleteConfirm?id=${cityId}`)
        .then(response => response.text())
        .then(html => {
            const container = document.getElementById('deleteModalContainer');
            if (container) {
                container.innerHTML = html;
            } else {
                const div = document.createElement('div');
                div.id = 'deleteModalContainer';
                div.innerHTML = html;
                document.body.appendChild(div);
            }
            
            currentCityId = cityId;
            const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm delete city
function confirmDeleteCity() {
    const deleteCityIdInput = document.getElementById('deleteCityId');
    const cityId = deleteCityIdInput ? deleteCityIdInput.value : currentCityId;
    
    if (!cityId) return;

    fetch(`/City/DeleteJson/${cityId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            const modal = bootstrap.Modal.getInstance(document.getElementById('deleteModal'));
            if (modal) modal.hide();
            
            showAlert(data.message || 'Şehir başarıyla silindi', 'success');
            // Reload page to reflect changes
            setTimeout(() => window.location.reload(), 1000);
        } else {
            showAlert(data.message || 'Bir hata oluştu', 'error');
        }
    })
    .catch(error => {
        console.error('Error:', error);
        showAlert('Silme işlemi sırasında bir hata oluştu', 'error');
    });

    currentCityId = null;
}

// Show alert
function showAlert(message, type) {
    if (typeof AlertModal !== 'undefined' && AlertModal.bootstrapModal) {
        if (type === 'success') {
            AlertModal.success(message);
        } else {
            AlertModal.error(message);
        }
    } else {
        alert(message);
    }
}
