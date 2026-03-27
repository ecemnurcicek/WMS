// Town Management JavaScript

let currentTownId = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    initializeTownManagement();
});

function initializeTownManagement() {
    setupEventListeners();
}

// Setup event listeners
function setupEventListeners() {
    // Add new town button
    const btnAddTown = document.getElementById('btnAddTown');
    if (btnAddTown) {
        btnAddTown.addEventListener('click', function () {
            openCreateModal();
        });
    }
}

// Open Create Modal
function openCreateModal() {
    fetch('/Town/CreateForm')
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
            
            // Attach region change listener
            attachRegionChangeListener();
            
            // Re-attach form submission listener
            const form = document.getElementById('townForm');
            if (form) {
                form.removeEventListener('submit', submitTownForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitTownForm();
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

// Edit town
function editTown(townId) {
    fetch(`/Town/EditForm?id=${townId}`)
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
            
            // Attach region change listener
            attachRegionChangeListener();
            
            // Re-attach form submission listener
            const form = document.getElementById('townForm');
            if (form) {
                form.removeEventListener('submit', submitTownForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitTownForm();
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

// Attach region change listener for cascading dropdown
function attachRegionChangeListener() {
    const regionSelect = document.getElementById('townRegion');
    if (regionSelect) {
        regionSelect.addEventListener('change', function () {
            const regionId = this.value;
            const citySelect = document.getElementById('townCity');
            
            // Clear city dropdown
            citySelect.innerHTML = '<option value="">-- Şehir Seçiniz --</option>';
            
            if (regionId) {
                // Fetch cities by region
                fetch(`/Town/GetCitiesByRegion?regionId=${regionId}`)
                    .then(response => response.json())
                    .then(cities => {
                        cities.forEach(city => {
                            const option = document.createElement('option');
                            option.value = city.id;
                            option.textContent = city.name;
                            citySelect.appendChild(option);
                        });
                    })
                    .catch(error => {
                        console.error('Error fetching cities:', error);
                    });
            }
        });
    }
}

// Submit town form
function submitTownForm() {
    const townId = document.getElementById('townId').value;
    const regionId = document.getElementById('townRegion').value;
    const cityId = document.getElementById('townCity').value;
    const name = document.getElementById('townName').value.trim();
    const isActive = document.getElementById('townIsActive').checked;

    // Validation
    if (!regionId) {
        showAlert('Lütfen bir bölge seçin', 'error');
        document.getElementById('townRegion').focus();
        return;
    }

    if (!cityId) {
        showAlert('Lütfen bir şehir seçin', 'error');
        document.getElementById('townCity').focus();
        return;
    }

    if (!name) {
        showAlert('İlçe adı boş olamaz', 'error');
        document.getElementById('townName').focus();
        return;
    }

    const url = townId && parseInt(townId) > 0 ? `/Town/Update/${townId}` : '/Town/CreateJson';
    const formData = new FormData();
    formData.append('id', townId || '0');
    formData.append('regionId', regionId);
    formData.append('cityId', cityId);
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

// Delete town (show confirmation modal)
function deleteTown(townId) {
    fetch(`/Town/DeleteConfirm?id=${townId}`)
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
            
            currentTownId = townId;
            const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm delete town
function confirmDeleteTown() {
    const deleteTownIdInput = document.getElementById('deleteTownId');
    const townId = deleteTownIdInput ? deleteTownIdInput.value : currentTownId;
    
    if (!townId) return;

    fetch(`/Town/DeleteJson/${townId}`, {
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
            
            showAlert(data.message || 'İlçe başarıyla silindi', 'success');
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

    currentTownId = null;
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
