// Region Management JavaScript

let currentRegionId = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    initializeRegionManagement();
});

function initializeRegionManagement() {
    setupEventListeners();
}

// Setup event listeners
function setupEventListeners() {
    // Add new region button
    document.getElementById('btnAddRegion').addEventListener('click', function () {
        openCreateModal();
    });
}

// Open Create Modal
function openCreateModal() {
    fetch('/Region/CreateForm')
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
            const form = document.getElementById('regionForm');
            if (form) {
                form.removeEventListener('submit', submitRegionForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitRegionForm();
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

// Edit region
function editRegion(regionId) {
    fetch(`/Region/EditForm/${regionId}`)
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
            const form = document.getElementById('regionForm');
            if (form) {
                form.removeEventListener('submit', submitRegionForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitRegionForm();
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

// Submit region form
function submitRegionForm() {
    const regionId = document.getElementById('regionId').value;
    const name = document.getElementById('regionName').value.trim();
    const isActive = document.getElementById('regionIsActive').checked;

    // Validation
    if (!name) {
        showAlert('Bölge adı boş olamaz', 'error');
        document.getElementById('regionName').focus();
        return;
    }

    const url = regionId && parseInt(regionId) > 0 ? `/Region/Update/${regionId}` : '/Region/CreateJson';
    const formData = new FormData();
    formData.append('id', regionId || '0');
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

// Delete region (show confirmation modal)
function deleteRegion(regionId) {
    fetch(`/Region/DeleteConfirm/${regionId}`)
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
            
            currentRegionId = regionId;
            const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm delete region
function confirmDeleteRegion() {
    const deleteRegionIdInput = document.getElementById('deleteRegionId');
    const regionId = deleteRegionIdInput ? deleteRegionIdInput.value : currentRegionId;
    
    if (!regionId) return;

    fetch(`/Region/DeleteJson/${regionId}`, {
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
            
            showAlert(data.message || 'Bölge başarıyla silindi', 'success');
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

    currentRegionId = null;
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
