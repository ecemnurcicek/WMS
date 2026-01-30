// Brand Management JavaScript

let currentBrandId = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    initializeBrandManagement();
});

function initializeBrandManagement() {
    setupEventListeners();
}

// Setup event listeners
function setupEventListeners() {
    // Add new brand button
    const btnAddBrand = document.getElementById('btnAddBrand');
    if (btnAddBrand) {
        btnAddBrand.addEventListener('click', function () {
            openCreateModal();
        });
    }
}

// Open Create Modal
function openCreateModal() {
    fetch('/Brand/CreateForm')
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
            const form = document.getElementById('brandForm');
            if (form) {
                form.removeEventListener('submit', submitBrandForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitBrandForm();
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

// Edit brand
function editBrand(brandId) {
    fetch(`/Brand/EditForm?id=${brandId}`)
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
            const form = document.getElementById('brandForm');
            if (form) {
                form.removeEventListener('submit', submitBrandForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitBrandForm();
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

// Submit brand form
function submitBrandForm() {
    const brandId = document.getElementById('brandId').value;
    const name = document.getElementById('brandName').value.trim();
    const isActive = document.getElementById('brandIsActive').checked;

    // Validation
    if (!name) {
        showAlert('Marka adı boş olamaz', 'error');
        document.getElementById('brandName').focus();
        return;
    }

    const url = brandId && parseInt(brandId) > 0 ? `/Brand/Update/${brandId}` : '/Brand/CreateJson';
    const formData = new FormData();
    formData.append('id', brandId || '0');
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

// Delete brand (show confirmation modal)
function deleteBrand(brandId) {
    fetch(`/Brand/DeleteConfirm?id=${brandId}`)
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
            
            currentBrandId = brandId;
            const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm delete brand
function confirmDeleteBrand() {
    const deleteBrandIdInput = document.getElementById('deleteBrandId');
    const brandId = deleteBrandIdInput ? deleteBrandIdInput.value : currentBrandId;
    
    if (!brandId) return;

    fetch(`/Brand/DeleteJson/${brandId}`, {
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
            
            showAlert(data.message || 'Marka başarıyla silindi', 'success');
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

    currentBrandId = null;
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
