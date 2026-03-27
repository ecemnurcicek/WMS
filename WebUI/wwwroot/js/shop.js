// Shop Management JavaScript

let currentShopId = null;

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    initializeShopManagement();
});

function initializeShopManagement() {
    setupEventListeners();
}

// Setup event listeners
function setupEventListeners() {
    // Add new shop button
    const btnAddShop = document.getElementById('btnAddShop');
    if (btnAddShop) {
        btnAddShop.addEventListener('click', function () {
            openCreateModal();
        });
    }
}

// Attach Region change listener for cascading dropdown
function attachRegionChangeListener() {
    const regionSelect = document.getElementById('shopRegion');
    if (regionSelect) {
        regionSelect.addEventListener('change', function () {
            const regionId = this.value;
            const citySelect = document.getElementById('shopCity');
            const townSelect = document.getElementById('shopTown');
            
            // Reset city and town dropdowns
            citySelect.innerHTML = '<option value="">-- Şehir Yükleniyor... --</option>';
            townSelect.innerHTML = '<option value="">-- Önce Şehir Seçiniz --</option>';
            
            if (regionId) {
                fetch(`/Shop/GetCitiesByRegion?regionId=${regionId}`)
                    .then(response => response.json())
                    .then(data => {
                        citySelect.innerHTML = '<option value="">-- Şehir Seçiniz --</option>';
                        data.forEach(city => {
                            const option = document.createElement('option');
                            option.value = city.id;
                            option.textContent = city.name;
                            citySelect.appendChild(option);
                        });
                    })
                    .catch(error => {
                        console.error('Error loading cities:', error);
                        citySelect.innerHTML = '<option value="">-- Şehir Yüklenemedi --</option>';
                    });
            } else {
                citySelect.innerHTML = '<option value="">-- Önce Bölge Seçiniz --</option>';
            }
        });
    }
}

// Attach City change listener for cascading dropdown
function attachCityChangeListener() {
    const citySelect = document.getElementById('shopCity');
    if (citySelect) {
        citySelect.addEventListener('change', function () {
            const cityId = this.value;
            const townSelect = document.getElementById('shopTown');
            
            // Reset town dropdown
            townSelect.innerHTML = '<option value="">-- İlçe Yükleniyor... --</option>';
            
            if (cityId) {
                fetch(`/Shop/GetTownsByCity?cityId=${cityId}`)
                    .then(response => response.json())
                    .then(data => {
                        townSelect.innerHTML = '<option value="">-- İlçe Seçiniz --</option>';
                        data.forEach(town => {
                            const option = document.createElement('option');
                            option.value = town.id;
                            option.textContent = town.name;
                            townSelect.appendChild(option);
                        });
                    })
                    .catch(error => {
                        console.error('Error loading towns:', error);
                        townSelect.innerHTML = '<option value="">-- İlçe Yüklenemedi --</option>';
                    });
            } else {
                townSelect.innerHTML = '<option value="">-- Önce Şehir Seçiniz --</option>';
            }
        });
    }
}

// Open Create Modal
function openCreateModal() {
    fetch('/Shop/CreateForm')
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
            
            // Attach cascading dropdown listeners
            attachRegionChangeListener();
            attachCityChangeListener();
            
            // Re-attach form submission listener
            const form = document.getElementById('shopForm');
            if (form) {
                form.removeEventListener('submit', submitShopForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitShopForm();
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

// Edit shop
function editShop(shopId) {
    fetch(`/Shop/EditForm?id=${shopId}`)
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
            
            // Attach cascading dropdown listeners
            attachRegionChangeListener();
            attachCityChangeListener();
            
            // Re-attach form submission listener
            const form = document.getElementById('shopForm');
            if (form) {
                form.removeEventListener('submit', submitShopForm);
                form.addEventListener('submit', function (e) {
                    e.preventDefault();
                    submitShopForm();
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

// Submit shop form
function submitShopForm() {
    const shopId = document.getElementById('shopId').value;
    const regionId = document.getElementById('shopRegion').value;
    const cityId = document.getElementById('shopCity').value;
    const townId = document.getElementById('shopTown').value;
    const brandId = document.getElementById('shopBrand').value;
    const name = document.getElementById('shopName').value.trim();
    const isActive = document.getElementById('shopIsActive').checked;

    // Validation
    if (!regionId) {
        showAlert('Lütfen bir bölge seçin', 'error');
        document.getElementById('shopRegion').focus();
        return;
    }

    if (!cityId) {
        showAlert('Lütfen bir şehir seçin', 'error');
        document.getElementById('shopCity').focus();
        return;
    }

    if (!townId) {
        showAlert('Lütfen bir ilçe seçin', 'error');
        document.getElementById('shopTown').focus();
        return;
    }

    if (!brandId) {
        showAlert('Lütfen bir marka seçin', 'error');
        document.getElementById('shopBrand').focus();
        return;
    }

    if (!name) {
        showAlert('Mağaza adı boş olamaz', 'error');
        document.getElementById('shopName').focus();
        return;
    }

    const url = shopId && parseInt(shopId) > 0 ? `/Shop/Update/${shopId}` : '/Shop/CreateJson';
    const formData = new FormData();
    formData.append('id', shopId || '0');
    formData.append('name', name);
    formData.append('brandId', brandId);
    formData.append('townId', townId);
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

// Delete shop (show confirmation modal)
function deleteShop(shopId) {
    fetch(`/Shop/DeleteConfirm?id=${shopId}`)
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
            
            currentShopId = shopId;
            const modal = new bootstrap.Modal(document.getElementById('deleteModal'));
            modal.show();
        })
        .catch(error => {
            console.error('Error:', error);
            showAlert('Modal yüklenirken bir hata oluştu', 'error');
        });
}

// Confirm delete shop
function confirmDeleteShop() {
    const deleteShopIdInput = document.getElementById('deleteShopId');
    const shopId = deleteShopIdInput ? deleteShopIdInput.value : currentShopId;
    
    if (!shopId) return;

    fetch(`/Shop/DeleteJson/${shopId}`, {
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
            
            showAlert(data.message || 'Mağaza başarıyla silindi', 'success');
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

    currentShopId = null;
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
